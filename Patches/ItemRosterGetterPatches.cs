using System.Diagnostics;
using System.Reflection;
using Batter.Core.Utils;
using HarmonyLib;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;

namespace Batter.Core.Patches;

public static class ItemRosterGetterPatches {
    /// <summary>
    ///     Installs safe guards on every ItemRoster getter/indexer.
    ///     Each patch is invoked via the provided patchRunner so you get
    ///     uniform logging and error‐handling.
    /// </summary>
    public static void PatchAll(
        Harmony harmony,
        Action<String, Action> patchRunner) {
        // all the int properties
        foreach (var prop in new[] {
                     "Count", "VersionNo", "TotalFood", "FoodVariety",
                     "TotalValue", "TradeGoodsTotalValue",
                     "NumberOfPackAnimals", "NumberOfLivestockAnimals", "NumberOfMounts",
                 })
            patchRunner(
                $"ItemRoster.{prop}",
                () => ItemRosterGetterPatches.PatchProperty<Int32>(harmony, prop)
            );

        // float TotalWeight
        patchRunner(
            "ItemRoster.TotalWeight",
            () => ItemRosterGetterPatches.PatchProperty<Single>(harmony, "TotalWeight")
        );

        // indexer this[int]
        patchRunner(
            "ItemRoster.get_Item(int)",
            () => ItemRosterGetterPatches.PatchIndexer<ItemRosterElement>(harmony)
        );
    }

    private static void PatchProperty<T>(Harmony harmony, String propName) {
        var getter = AccessTools.PropertyGetter(typeof(ItemRoster), propName);
        var patchMethod = typeof(GenericFinalizerPatch<>)
            .MakeGenericType(typeof(T))
            .GetMethod(nameof(GenericFinalizerPatch<T>.Finalizer),
                BindingFlags.Public | BindingFlags.Static);

        // prefix = null, postfix = null, transpiler = null, finalizer = patchMethod
        harmony.Patch(
            getter,
            null,
            null,
            null,
            new(patchMethod)
        );
    }

    private static void PatchIndexer<T>(Harmony harmony) {
        var getter = AccessTools.DeclaredMethod(typeof(ItemRoster), "get_Item", new[] { typeof(Int32) });
        var patchMethod = typeof(GenericFinalizerPatch<>)
            .MakeGenericType(typeof(T))
            .GetMethod(nameof(GenericFinalizerPatch<T>.Finalizer),
                BindingFlags.Public | BindingFlags.Static);

        harmony.Patch(
            getter,
            null,
            null,
            null,
            new(patchMethod)
        );
    }

    /// <summary>
    ///     Catches any exception from the original getter, logs it,
    ///     defaults the result, and swallows the exception.
    /// </summary>
    public static class GenericFinalizerPatch<T> {
        public static void Finalizer(Exception __exception) {
            if (__exception != null)
                try {
                    // figure out which getter blew up
                    var frame = new StackTrace().GetFrame(1);
                    var methodName = frame?.GetMethod()?.Name ?? "unknown";
                    BatterLog.Error(
                        $"[ItemRosterSafeGetters] {methodName} threw {__exception.GetType().Name}: {__exception.Message}");
                    //__result = default;
                }
                catch (Exception ex) {
                    BatterLog.Error($"[ItemRosterSafeGetters] failed to default result: {ex}");
                }
            // swallow
        }
    }
}