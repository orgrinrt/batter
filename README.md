# `batter`

<div align="center">

> Crash-guard Harmony patches and campaign fixes for Mount & Blade II: Bannerlord.

</div>

Batter is a Bannerlord module that hardens a modded campaign against crashes caused by null or
invalid campaign state. Harmony patches guard war declaration log entries, diplomacy scoring,
villager and settlement ticks, item roster access, and market price lookups. Campaign behaviors
sanitize broken hero and clan records, and a custom item value model (`OrgrinsItemValueModel`)
replaces the default item valuation. The solution is a work in progress and parts of it are
mid-rewrite.

## Projects

| Project | Target | Purpose |
|---------|--------|---------|
| `Batter.Core` | net472, net6.0 | The module itself: Harmony patches, campaign behaviors, the item value model, and XSLT `ModuleData` transforms (culture name extensions, armor culture autopatch, missing item prices, item variety). |
| `Batter.Utils.Builders` | net6.0 | Standalone fluent builder library: `IBuilder`/`IBuildable` contracts, `DynBuilder` dynamic builders, and predicate combinators. No Bannerlord dependency. |
| `Batter.ItemValuation` | net6.0 | Item valuation library. Currently contains no sources. |
| `Batter.ItemValuation.Tests` | net6.0 | NUnit test project for `Batter.ItemValuation`. Currently contains no sources. |

## Building

`Batter.Core` resolves game assemblies through the `BANNERLORD_GAME_DIR` environment variable and
references the Bannerlord.Diplomacy module DLL from the game's `Modules` directory. Set the
variable to the game install path, then build the solution:

```sh
BANNERLORD_GAME_DIR="/path/to/Mount & Blade II Bannerlord" dotnet build Batter.sln
```

NuGet dependencies: Lib.Harmony, Bannerlord.ButterLib, Bannerlord.UIExtenderEx, Bannerlord.MCM,
and Bannerlord.ReferenceAssemblies. The module manifest (`SubModule.xml`) declares dependencies on
Bannerlord.Harmony, Bannerlord.MBOptionScreen, Native, SandBoxCore, Sandbox, and StoryMode.
