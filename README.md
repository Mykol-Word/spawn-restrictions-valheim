# Spawn Restrictions

A BepInEx 5 / Harmony mod that requires enough players online for bosses.

**Install the same version on the host/dedicated server and every client.** Only the host/server's configuration is used. This MVP assumes everyone has the mod; it does not kick players who are missing it.

## Install and configure

1. Build with `dotnet build SpawnRestrictions.csproj -c Release`.
2. Copy `bin/Release/net472/SpawnRestrictions.dll` into `BepInEx/plugins/SpawnRestrictions/` on every installation (or each mod-manager profile).
3. Launch once to generate `BepInEx/config/spawnrestrictions.valheim.cfg`.
4. Stop the host/server, edit its config, then restart it:

```ini
[Boss restrictions]
Required online players = 2
```

The default is **2**. Use `0` to disable restrictions. Existing config values from an earlier build are preserved. A listen host counts as a player; a dedicated server process does not. Players can be anywhere in the world. Client config files are ignored while connected to a server.

## Behavior

- At or above the requirement, normal boss behavior is preserved.
- Below it, altar attempts show a message and stop before consuming inventory or item-stand offerings.
- Every half second, each peer removes bosses it owns using Valheim's normal network destruction. This covers other spawn paths and ongoing fights, including previously saved bosses once loaded. Despawning awards no death loot or defeat progression.
- If players disconnect during summoning, an already accepted offering is not refunded; its boss will be removed. There are no custom item drops or inventory refunds.
- Server state arrives periodically, so a newly spawned boss can briefly appear before cleanup. This is cooperative multiplayer enforcement, not protection against modified or missing clients.
- Clients block altar attempts until the first server snapshot arrives, but do not despawn bosses while waiting for it. State is cleared when leaving a world.

## Build paths

The project uses the local Valheim and BepInEx paths documented in `AGENTS.md`. Override them on another machine:

```powershell
dotnet build SpawnRestrictions.csproj -c Release `
  -p:ValheimManagedDir='D:\SteamLibrary\steamapps\common\Valheim\valheim_Data\Managed' `
  -p:BepInExCoreDir='D:\Valheim\BepInEx\core'
```

Only the mod DLL is distributed. Game and BepInEx assemblies are external references and are not copied into the output.

## Verification

The test harness uses .NET 10, fake peer connections for synchronization tests, and the installed DLL metadata for patch-signature checks. It does not run Unity or simulate an actual multiplayer session.

```powershell
dotnet build tests/SpawnRestrictions.Tests.csproj -c Release
dotnet tests/bin/Release/net10.0/SpawnRestrictions.Tests.dll `
  bin/Release/net472/SpawnRestrictions.dll `
  'C:\Program Files (x86)\Steam\steamapps\common\Valheim\valheim_Data\Managed' `
  'C:\Users\purpl\AppData\Roaming\r2modmanPlus-local\Valheim\profiles\ValheimModded\BepInEx\core'
```

Before using a real world, test on a disposable world with the mod on both peers:

1. Set the host requirement to 2 and the client to 99. With two players, summoning should work regardless of who owns the altar.
2. Set the host requirement to 3 and the client to 0. With two players, inventory and item-stand offerings should remain untouched and a message should explain the requirement.
3. With the host requirement at 2, summon a boss and disconnect one player. The remaining owner should remove the boss without loot; reconnecting should not restore it.
4. Set the host requirement to 0. All normal summoning should work.
5. Repeat on a dedicated server and at a distant altar, then reconnect to a different host to verify the old rule is discarded.
