using SpawnRestrictions.Configuration;

namespace SpawnRestrictions.Services;

internal sealed class ServerRuleSync
{
    private const string StateRpc = "SpawnRestrictions.State.v2";
    private readonly BossRestrictionSettings _settings;
    private readonly BossRestrictionState _state;

    // connects the local configuration to the authoritative rule snapshot
    public ServerRuleSync(BossRestrictionSettings settings, BossRestrictionState state)
    {
        _settings = settings;
        _state = state;
    }

    // registers the state message before the peer handshake
    public void RegisterPeer(ZNetPeer peer)
    {
        peer.m_rpc.Register<int, int, bool>(StateRpc, ReceiveState);
    }

    // refreshes host state without consulting client configuration
    public void RefreshHost()
    {
        var network = ZNet.instance;
        if (network != null && network.IsServer())
        {
            _state.Set(_settings.RequiredOnlinePlayers.Value, network.GetNrOfPlayers(),
                _settings.AllowDefeatedBosses.Value);
        }
    }

    // periodically sends the host's rule and player count to connected peers
    public void Broadcast()
    {
        var network = ZNet.instance;
        if (network == null || !network.IsServer())
        {
            return;
        }

        RefreshHost();
        foreach (var peer in network.GetPeers())
        {
            if (peer.IsReady())
            {
                peer.m_rpc.Invoke(StateRpc, _state.RequiredPlayers, _state.OnlinePlayers, _state.AllowDefeatedBosses);
            }
        }
    }

    // accepts rule updates only from the client's actual server connection
    private void ReceiveState(ZRpc rpc, int requiredPlayers, int onlinePlayers, bool allowDefeatedBosses)
    {
        var network = ZNet.instance;
        if (network == null || network.IsServer() || !ReferenceEquals(rpc, network.GetServerRPC()))
        {
            return;
        }

        _state.Set(requiredPlayers, onlinePlayers, allowDefeatedBosses);
    }
}
