# Spawn Restrictions

A BepInEx 5 / Harmony mod that requires enough players online to summon bosses at altars.

**Install the same version on the host/dedicated server and every client.** Only the host/server's configuration is used. This MVP assumes everyone has the mod; it does not kick players who are missing it.

## Install and configure

1. Build with `dotnet build SpawnRestrictions.csproj -c Release`.
2. Copy `bin/Release/net472/SpawnRestrictions.dll` into `BepInEx/plugins/SpawnRestrictions/` on every installation (or each mod-manager profile).
3. Launch once to generate `BepInEx/config/spawnrestrictions.valheim.cfg`.
4. Stop the host/server, edit its config, then restart it:

```ini
[Boss restrictions]
Required online players = 2
Allow defeated bosses = true
```

The default is **2**. Use `0` to disable restrictions. Existing config values from an earlier build are preserved. A listen host counts as a player; a dedicated server process does not. Players can be anywhere in the world. Client config files are ignored while connected to a server.

`Allow defeated bosses` defaults to **true**: a boss whose defeat key is set in the current world can be summoned below the player requirement. Set it to **false** to apply the requirement to every boss, as before. Progress in another world or on a player's character does not grant the exemption. Bosses without a defeat key remain subject to the requirement.

Update the host/server and all clients to version **0.2.0** together for the new synced option.

## Behavior

- At or above the requirement, normal altar summoning is preserved.
- Below it, attempts to summon undefeated bosses show a message and stop before consuming offerings. With `Allow defeated bosses = false`, this applies to every boss. The altar owner checks the rule again when handling the spawn request.
- Existing bosses and ongoing fights are unaffected when players disconnect. Previously saved bosses are also left alone.
- A summon already accepted by the altar finishes normally even if the player count drops during its spawn delay.
- Restrictions apply to altar summoning. Console commands and other spawn paths are not intercepted.
- The host broadcasts its rule and player count every half second. Clients block altar attempts until the first server snapshot arrives, and clear that state when leaving a world.

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
2. Set the host requirement to 3 and the client to 0. With two players, undefeated bosses should be blocked without consuming inventory or item-stand offerings, and a message should explain the requirement.
3. With the host requirement at 2, summon a boss and disconnect one player. The boss should remain and the fight should continue, while new summons of undefeated bosses are blocked.
4. Set the host requirement to 0. All normal summoning should work.
5. Below the requirement, re-summon a boss previously defeated in this world with `Allow defeated bosses = true`. It should work for both inventory and item-stand altars. Set the host option to `false`; the same boss should now be blocked even if the client's option is `true`.
6. Repeat on a dedicated server and at a distant altar, then connect to another world where the boss has not been defeated. It should be restricted again.
