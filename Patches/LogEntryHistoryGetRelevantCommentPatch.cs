using Batter.Core.Utils;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;

namespace Batter.Core.Patches;

[HarmonyPatch(typeof(LogEntryHistory), "GetRelevantComment")]
public static class LogEntryHistoryGetRelevantCommentPatch {
    private static Boolean Prefix(Hero conversationHero, ref Int32 bestScore, ref String bestRelatedLogEntryTag) {
        if (conversationHero == null || conversationHero.Clan == null || conversationHero.MapFaction == null) {
            bestScore = 0;
            bestRelatedLogEntryTag = String.Empty;
            BatterLog.Info($"[SafeLogEntryHistoryPatch] Skipped corrupt hero {conversationHero?.Name}");
            return false;
        }

        return true;
    }
}