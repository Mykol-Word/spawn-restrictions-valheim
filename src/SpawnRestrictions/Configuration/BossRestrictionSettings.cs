using BepInEx.Configuration;

namespace SpawnRestrictions.Configuration;

internal sealed class BossRestrictionSettings
{
    public ConfigEntry<int> RequiredOnlinePlayers { get; }
    public ConfigEntry<bool> AllowDefeatedBosses { get; }

    // binds the host's boss summoning rules
    public BossRestrictionSettings(ConfigFile config)
    {
        RequiredOnlinePlayers = config.Bind(
            "Boss restrictions", "Required online players", 2,
            new ConfigDescription(
                "Minimum online players to summon bosses at altars. Only the host/server value is used. " +
                "0 disables the restriction. Existing bosses are unaffected.",
                new AcceptableValueRange<int>(0, int.MaxValue)));
        AllowDefeatedBosses = config.Bind(
            "Boss restrictions", "Allow defeated bosses", true,
            "Allow bosses already defeated in the current world to be summoned regardless of player count. " +
            "Only the host/server value is used. False applies the player requirement to every boss.");
    }
}
