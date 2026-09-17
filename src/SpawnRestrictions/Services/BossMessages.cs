namespace SpawnRestrictions.Services;

internal static class BossMessages
{
    // explains the host's current restriction to the summoning player
    public static void Show(Humanoid player, BossRestrictionState state)
    {
        if (player == null)
        {
            return;
        }

        var text = state.IsReady
            ? $"Bosses require {state.RequiredPlayers} players online ({state.OnlinePlayers} online)."
            : "Waiting for the server's boss restrictions.";
        player.Message(MessageHud.MessageType.Center, text);
    }
}
