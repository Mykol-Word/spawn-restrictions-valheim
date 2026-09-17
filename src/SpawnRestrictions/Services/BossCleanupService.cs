using BepInEx.Logging;
using UnityEngine;

namespace SpawnRestrictions.Services;

internal sealed class BossCleanupService
{
    private readonly BossRestrictionState _state;
    private readonly ManualLogSource _logger;

    // connects owner-side cleanup to the server's rule
    public BossCleanupService(BossRestrictionState state, ManualLogSource logger)
    {
        _state = state;
        _logger = logger;
    }

    // removes owned bosses through vanilla network destruction without death rewards
    public void RemoveBlockedBosses()
    {
        if (!_state.ShouldDespawn || ZNetScene.instance == null)
        {
            return;
        }

        foreach (var character in Character.GetAllCharacters().ToArray())
        {
            if (character == null || !character.IsBoss() || !character.IsOwner())
            {
                continue;
            }

            var view = character.GetComponent<ZNetView>();
            if (view == null || !view.IsValid())
            {
                continue;
            }

            var zdo = view.GetZDO();
            if (zdo.GetBool(ZDOVars.s_bossCount) && ZoneSystem.instance != null)
            {
                ZoneSystem.instance.GetGlobalKey(GlobalKeys.activeBosses, out float count);
                ZoneSystem.instance.SetGlobalKey(GlobalKeys.activeBosses, Mathf.Max(0f, count - 1f));
                zdo.Set(ZDOVars.s_bossCount, false);
            }

            _logger.LogInfo($"Despawning {character.name}: {_state.OnlinePlayers}/{_state.RequiredPlayers} players online.");
            ZNetScene.instance.Destroy(character.gameObject);
        }
    }
}
