using UnityEngine;

namespace SpawnRestrictions.Services;

internal static class BossProgressionService
{
    // checks the target boss's defeat key in the current world's progression
    public static bool IsDefeated(GameObject bossPrefab)
    {
        if (bossPrefab == null || ZoneSystem.instance == null)
        {
            return false;
        }

        var character = bossPrefab.GetComponent<Character>();
        return character != null && !string.IsNullOrEmpty(character.m_defeatSetGlobalKey) &&
            ZoneSystem.instance.GetGlobalKey(character.m_defeatSetGlobalKey);
    }
}
