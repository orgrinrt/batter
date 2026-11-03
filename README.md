# `batter`

> Crash-guard Harmony patches and campaign fixes for Mount & Blade II: Bannerlord.

Batter is a Bannerlord module that hardens a modded campaign against crashes from null or invalid campaign state. Harmony patches guard war declaration log entries, diplomacy scoring, villager and settlement ticks, item roster access and market price lookups, campaign behaviors sanitize broken hero and clan records, and a custom item value model, `OrgrinsItemValueModel`, replaces the default item valuation.

The solution has four projects. `Batter.Core` is the module itself, with the patches, behaviors, the item value model and the XSLT `ModuleData` transforms, `Batter.Utils.Builders` is a standalone fluent builder library with no Bannerlord dependency, and `Batter.ItemValuation` with its `.Tests` project are empty for now.

## Building

`global.json` pins the .NET SDK to 6.0.x. `Batter.Core` finds the game assemblies through the `BANNERLORD_GAME_DIR` environment variable and references the Bannerlord.Diplomacy module from the game's `Modules` directory.

```sh
BANNERLORD_GAME_DIR="/path/to/Mount & Blade II Bannerlord" dotnet build Batter.sln
```

The module manifest depends on Bannerlord.Harmony, Bannerlord.MBOptionScreen, Native, SandBoxCore, Sandbox and StoryMode.

## Status

Work in progress and mid-rewrite, and the solution doesn't build as shipped. `ComputedPriceRegistry` is called from `SubModule.cs` and the item value model but its source was deleted in the valuation rewrite, a dozen files under `Batter.Core/Patches/` use `System` types without a `using System;`, `SubModule.cs` sits at the repository root where no project includes it, and `SubModule.xml` and `build.sh` still name a `SafeWarLogPatch` assembly that nothing produces. `Batter.Utils.Builders` builds on its own.
