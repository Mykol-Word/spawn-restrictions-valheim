# Spawn Restrictions

Spawn Restrictions adds a configurable player-count requirement to boss summoning at altars in Valheim. Summoning is blocked until enough players are online. Existing bosses and ongoing fights are unaffected.

Install the same version on the host or dedicated server and every client. The host or server controls the active settings.

## Configuration

Start the game or server once, then edit `BepInEx/config/spawnrestrictions.valheim.cfg`:

```ini
[Boss restrictions]
Required online players = 2
Allow defeated bosses = true
```

- `Required online players` sets the minimum number of online players needed to summon a boss.
- `Allow defeated bosses` lets a boss already defeated in the current world be summoned below the player requirement.

Use `0` to disable restrictions. A host player counts as an online player, but a dedicated server process does not. Client configuration is ignored while connected to a server.

## Use

1. Install this package with r2modman/Thunderstore, or copy its contents into the Valheim game directory.
2. Launch once to generate the configuration file.
3. Edit the host or server settings and restart.
4. Summon bosses at altars as usual.

When the requirement is met, summoning works normally. When it is not met, the summon is blocked and a message explains the requirement.

## Source

[Source code and issue tracker](https://github.com/Mykol-Word/spawn-restrictions-valheim)
