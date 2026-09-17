# spawn-restrictions-valheim

A Valheim mod that adds configurable restrictions for bosses, events, and traveling. The code should be modular, readable, and easy to extend as new restrictions are added.

## Architecture

- Keep restriction logic separate from Valheim hooks, configuration, and player-facing messages.
- Keep hooks thin: gather the relevant game context, call a focused service, and apply the result.
- Organize code by responsibility, such as boss restrictions, event restrictions, traveling restrictions, configuration, and shared helpers.
- Prefer small classes and methods with one clear responsibility over large plugin or patch classes.
- Keep shared logic in reusable services instead of duplicating it across patches.
- Keep dependencies and state explicit. Avoid hidden global state.
- Preserve vanilla behavior when a restriction is disabled or does not apply.
- Make changes modular so one restriction can change without affecting unrelated features.

## Coding Rules

- Use clear C# naming and conventions already established by the project.
- Write concise, clean code with straightforward control flow.
- Prefer descriptive names over clever abstractions.
- Keep configuration definitions centralized and clearly named.
- Remove unused code, usings, fields, and temporary debug output.
- Handle errors deliberately; do not hide broken behavior with broad exception handling.
- Use minimal comments only for extremely obscure code.
- Add one concise, lowercase, one-line comment directly above every function describing its purpose.
- Keep all comments lowercase and do not use comments to restate straightforward code.

```csharp
// checks whether the event may start
public static bool CanStartEvent(EventContext context)
{
    return eventRestrictions.Allows(context, config);
}
```

## Git Rules

- Keep commits focused on one logical change.
- Use concise, imperative commit messages.
- Do not include unrelated formatting or generated files.
- Do not commit build outputs, binaries, logs, caches, or local game assemblies.
- Do not add AI crediting or emojis to commits.

## Decompiled Valheim reference

The local Valheim installation used for this reference is:

- Game root: `C:\Program Files (x86)\Steam\steamapps\common\Valheim`
- Managed assemblies: `C:\Program Files (x86)\Steam\steamapps\common\Valheim\valheim_Data\Managed`
- Main gameplay DLL: `valheim_Data\Managed\assembly_valheim.dll`
- Unity and supporting DLLs: `valheim_Data\Managed\*.dll`

ILSpy command-line decompiler (`ilspycmd` 10.1.1.8388) produced compilable C# projects under [`reference/valheim-decompiled`](reference/valheim-decompiled):

- [`assembly_valheim`](reference/valheim-decompiled/assembly_valheim) — main gameplay code, 692 C# files; project file: [`assembly_valheim.csproj`](reference/valheim-decompiled/assembly_valheim/assembly_valheim.csproj)
- [`assembly_utils`](reference/valheim-decompiled/assembly_utils)
- [`assembly_guiutils`](reference/valheim-decompiled/assembly_guiutils)
- [`assembly_postprocessing`](reference/valheim-decompiled/assembly_postprocessing)
- [`assembly_simplemeshcombine`](reference/valheim-decompiled/assembly_simplemeshcombine)
- [`assembly_sunshafts`](reference/valheim-decompiled/assembly_sunshafts)
- [`assembly_lux`](reference/valheim-decompiled/assembly_lux)
- [`assembly_googleanalytics`](reference/valheim-decompiled/assembly_googleanalytics)
- [`gui_framework`](reference/valheim-decompiled/gui_framework)
- [`Assembly-CSharp`](reference/valheim-decompiled/Assembly-CSharp)

Useful starting points for this mod are [`OfferingBowl.cs`](reference/valheim-decompiled/assembly_valheim/OfferingBowl.cs) for boss spawning, [`RandEventSystem.cs`](reference/valheim-decompiled/assembly_valheim/RandEventSystem.cs) for random events (`StartRandomEvent`), [`TeleportWorld.cs`](reference/valheim-decompiled/assembly_valheim/TeleportWorld.cs) for portal travel (`Teleport`), [`Player.cs`](reference/valheim-decompiled/assembly_valheim/Player.cs) for player travel checks, and [`BossStone.cs`](reference/valheim-decompiled/assembly_valheim/BossStone.cs) for boss interaction.

The installed BepInEx/Harmony references are in the Valheim mod profile:

- `C:\Users\purpl\AppData\Roaming\r2modmanPlus-local\Valheim\profiles\ValheimModded\BepInEx\core\BepInEx.dll` (5.4.23.3)
- `C:\Users\purpl\AppData\Roaming\r2modmanPlus-local\Valheim\profiles\ValheimModded\BepInEx\core\0Harmony.dll` (2.9.0.0)
- `C:\Users\purpl\AppData\Roaming\r2modmanPlus-local\Valheim\profiles\ValheimModded\BepInEx\core\BepInEx.Harmony.dll`

These decompiled files are a development reference and are tied to the installed game build. Do not copy them into the shipped mod or edit them as game source. Use the original DLLs as compiler references and Harmony patch targets. If Valheim updates, regenerate the reference before relying on method names or signatures.

### Refreshing the reference

Run from the repository root in PowerShell after a game update (requires `ilspycmd` on `PATH`):

```powershell
$managed = 'C:\Program Files (x86)\Steam\steamapps\common\Valheim\valheim_Data\Managed'
$reference = Join-Path (Get-Location) 'reference\valheim-decompiled'
$assemblies = @(
  'assembly_valheim.dll', 'assembly_utils.dll', 'assembly_guiutils.dll',
  'assembly_postprocessing.dll', 'assembly_simplemeshcombine.dll',
  'assembly_sunshafts.dll', 'assembly_lux.dll', 'assembly_googleanalytics.dll',
  'gui_framework.dll', 'Assembly-CSharp.dll'
)
foreach ($assembly in $assemblies) {
  $output = Join-Path $reference ([IO.Path]::GetFileNameWithoutExtension($assembly))
  Remove-Item -LiteralPath $output -Recurse -Force -ErrorAction SilentlyContinue
  New-Item -ItemType Directory -Force -Path $output | Out-Null
  ilspycmd -p --nested-directories -o $output -r $managed --disable-updatecheck (Join-Path $managed $assembly)
}
```

The exact DLL build captured on September 16, 2026 is recorded by SHA-256 for the two gameplay assemblies:

- `assembly_valheim.dll`: `27A766A8D23A7BD8B6A54FB9AD0452A96C305FB3629B39C40527C09A1C393A84`
- `Assembly-CSharp.dll`: `67807C30783BA84EA3157C20475BE4FECB28A54E15A33172853854EF8535CD6F`
