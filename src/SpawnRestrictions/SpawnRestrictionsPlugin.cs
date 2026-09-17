using BepInEx;
using HarmonyLib;
using SpawnRestrictions.Configuration;
using SpawnRestrictions.Patches;
using SpawnRestrictions.Services;
using UnityEngine;

namespace SpawnRestrictions;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class SpawnRestrictionsPlugin : BaseUnityPlugin
{
    public const string PluginGuid = "spawnrestrictions.valheim";
    public const string PluginName = "Spawn Restrictions";
    public const string PluginVersion = "0.1.0";

    private Harmony _harmony;
    private ServerRuleSync _sync;
    private float _nextUpdate;

    // initializes the host rule and the patches required on every peer
    private void Awake()
    {
        var state = new BossRestrictionState();
        _sync = new ServerRuleSync(new BossRestrictionSettings(Config), state);
        BossPatches.Initialize(state, _sync);
        _harmony = new Harmony(PluginGuid);
        _harmony.CreateClassProcessor(typeof(BossPatches)).Patch();
        Logger.LogInfo("Spawn Restrictions loaded. Install this version on the host/server and every client.");
    }

    // synchronizes the server summoning rule twice per second
    private void Update()
    {
        if (ZNet.instance == null || ZNet.instance.HaveStopped || Time.unscaledTime < _nextUpdate)
        {
            return;
        }

        _nextUpdate = Time.unscaledTime + 0.5f;
        _sync.Broadcast();
    }

    // removes this plugin's patches and releases their services
    private void OnDestroy()
    {
        _harmony?.UnpatchSelf();
        BossPatches.Clear();
    }
}
