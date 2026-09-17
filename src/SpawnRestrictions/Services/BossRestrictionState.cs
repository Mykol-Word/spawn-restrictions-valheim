namespace SpawnRestrictions.Services;

internal sealed class BossRestrictionState
{
    public bool IsReady { get; private set; }
    public int RequiredPlayers { get; private set; }
    public int OnlinePlayers { get; private set; }
    public bool AllowsBosses => IsReady && (RequiredPlayers == 0 || OnlinePlayers >= RequiredPlayers);

    // stores a validated snapshot from the host
    public void Set(int requiredPlayers, int onlinePlayers)
    {
        if (requiredPlayers < 0 || onlinePlayers < 0)
        {
            return;
        }

        RequiredPlayers = requiredPlayers;
        OnlinePlayers = onlinePlayers;
        IsReady = true;
    }

    // discards the previous world's rule before another connection
    public void Reset()
    {
        IsReady = false;
        RequiredPlayers = 0;
        OnlinePlayers = 0;
    }
}
