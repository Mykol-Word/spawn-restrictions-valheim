namespace SpawnRestrictions.Services;

internal sealed class BossRestrictionState
{
    public bool IsReady { get; private set; }
    public int RequiredPlayers { get; private set; }
    public int OnlinePlayers { get; private set; }
    public bool AllowDefeatedBosses { get; private set; }

    // checks the player requirement and the optional world-defeat exemption
    public bool AllowsSummoning(bool defeatedInWorld)
    {
        return IsReady && (RequiredPlayers == 0 || OnlinePlayers >= RequiredPlayers ||
            (AllowDefeatedBosses && defeatedInWorld));
    }

    // stores a validated snapshot from the host
    public void Set(int requiredPlayers, int onlinePlayers, bool allowDefeatedBosses)
    {
        if (requiredPlayers < 0 || onlinePlayers < 0)
        {
            return;
        }

        RequiredPlayers = requiredPlayers;
        OnlinePlayers = onlinePlayers;
        AllowDefeatedBosses = allowDefeatedBosses;
        IsReady = true;
    }

    // discards the previous world's rule before another connection
    public void Reset()
    {
        IsReady = false;
        RequiredPlayers = 0;
        OnlinePlayers = 0;
        AllowDefeatedBosses = false;
    }
}
