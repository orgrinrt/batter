#region

using Batter.Core.Utils;
using HarmonyLib;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.ObjectSystem;

#endregion

namespace Batter.Core.Patches;

[HarmonyPatch(typeof(FightTournamentGame), "CachePossibleEliteRewardItems")]
internal class TournamentCachePossibleEliteRewardItemsPatch {
    private static void Prefix() {
        var items = MBObjectManager.Instance.GetObjectTypeList<IItemObject>();
        foreach (var item in items)
            if (item == null)
                BatterLog.Error("[Tournament] Null IItemObject found!");
            else if (item.Name == null || item.StringId == null)
                BatterLog.Error($"[Tournament] Incomplete item: {item?.ToString() ?? "null"}");
    }

    private static void Postfix(FightTournamentGame __instance)
    {
    // find the first private instance field of type List<IItemObject>
    var
    listField
    =
    typeof
    (
    FightTournamentGame
    )
    .
    GetFields
    (
    BindingFlags
    .
    Instance
    |
    BindingFlags
    .
    NonPublic
    )
    .
    FirstOrDefault
    (
    f
    =>
    f
    .
    FieldType
    ==
    typeof
    (
    List
    <
    IItemObject
    >
    )
    )
    ;
    if
    (
    listField
    is
    null
    )
    // if you want, log a warning once
    return
    ;
    var
    prizeList
    =
    (
    List
    <
    IItemObject
    >
    )
    listField
    .
    GetValue
    (
    __instance
    )
    ;
    if
    (
    prizeList
    ==
    null
    )
    return
    ;

    // Remove null entries so the comparer never sees them
    prizeList
    .
    RemoveAll
    (
    i
    =>
    i
    ==
    null
    )
    ;

   }
}