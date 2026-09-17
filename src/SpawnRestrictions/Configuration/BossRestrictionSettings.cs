using BepInEx.Configuration;

namespace SpawnRestrictions.Configuration;

internal sealed class BossRestrictionSettings
{
    public ConfigEntry<int> RequiredOnlinePlayers { get; }

    // binds the host's minimum player requirement
    public BossRestrictionSettings(ConfigFile config)
    {
        RequiredOnlinePlayers = config.Bind(
            "Boss restrictions", "Required online players", 2,
            new ConfigDescription(
                "Minimum online players to summon bosses at altars. Only the host/server value is used. " +
                "0 disables the restriction. Existing bosses are unaffected.",
                new AcceptableValueRange<int>(0, int.MaxValue)));
    }
}
