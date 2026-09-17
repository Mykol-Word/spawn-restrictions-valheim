using HarmonyLib;
using SpawnRestrictions.Services;

namespace SpawnRestrictions.Patches;

[HarmonyPatch]
internal static class BossPatches
{
    private static BossRestrictionState _state;
    private static ServerRuleSync _sync;

    // supplies the shared services used by harmony's static entry points
    public static void Initialize(BossRestrictionState state, ServerRuleSync sync)
    {
        _state = state;
        _sync = sync;
    }

    // releases the plugin-owned services after unpatching
    public static void Clear()
    {
        _state = null;
        _sync = null;
    }

    // forgets the previous world's rule before a network session starts
    [HarmonyPatch(typeof(ZNet), "Awake"), HarmonyPrefix]
    private static void BeginSession()
    {
        _state.Reset();
    }

    // forgets server state when leaving a world
    [HarmonyPatch(typeof(ZNet), "OnDestroy"), HarmonyPostfix]
    private static void EndSession()
    {
        _state.Reset();
    }

    // installs the custom message handler before the vanilla handshake starts
    [HarmonyPatch(typeof(ZNet), "OnNewConnection"), HarmonyPrefix]
    private static void RegisterPeer(ZNetPeer peer)
    {
        _sync.RegisterPeer(peer);
    }

    // blocks inventory offerings before items or global keys can change
    [HarmonyPatch(typeof(OfferingBowl), nameof(OfferingBowl.UseItem)), HarmonyPrefix]
    private static bool UseItem(OfferingBowl __instance, Humanoid user, ItemDrop.ItemData item, ref bool __result)
    {
        if (__instance.m_useItemStands || __instance.m_bossPrefab == null ||
            __instance.m_bossItem == null || item == null ||
            item.m_shared.m_name != __instance.m_bossItem.m_itemData.m_shared.m_name || AllowsBosses())
        {
            return true;
        }

        BossMessages.Show(user, _state);
        __result = true;
        return false;
    }

    // blocks item-stand summoning while leaving the offerings attached
    [HarmonyPatch(typeof(OfferingBowl), nameof(OfferingBowl.Interact)), HarmonyPrefix]
    private static bool Interact(OfferingBowl __instance, Humanoid user, bool hold, ref bool __result)
    {
        if (hold || !__instance.m_useItemStands || __instance.m_bossPrefab == null || AllowsBosses())
        {
            return true;
        }

        BossMessages.Show(user, _state);
        __result = false;
        return false;
    }

    // rechecks the restriction on the altar's owner before consuming offerings
    [HarmonyPatch(typeof(OfferingBowl), "RPC_SpawnBoss"), HarmonyPrefix]
    private static bool SpawnBoss(OfferingBowl __instance)
    {
        return __instance.m_bossPrefab == null || AllowsBosses();
    }

    // refreshes the host's count before deciding whether an altar may proceed
    private static bool AllowsBosses()
    {
        _sync.RefreshHost();
        return _state.AllowsBosses;
    }
}
