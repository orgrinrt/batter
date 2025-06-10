using System;
using Batter.Core.Utils;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;

namespace Batter.Core.Patches;

[HarmonyPatch(typeof(DeclareWarLogEntry), nameof(DeclareWarLogEntry.GetConversationScoreAndComment))]
public static class DeclareWarLogEntryGetConversationScoreAndCommentPatch {
    private static bool Prefix(
        DeclareWarLogEntry __instance,
        Hero talkTroop,
        bool findString,
        out string comment,
        out ImportanceEnum score) {
        try {
            if (talkTroop == null || talkTroop.Clan == null || talkTroop.MapFaction == null) {
                BatterLog.Info($"[SafeDeclareWarLogEntryPatch] Skipping invalid talkTroop: {talkTroop?.Name}");
                comment = string.Empty;
                score = ImportanceEnum.Zero;
                return false; // skip original
            }

            // TEMP: Always skip until stabilized
            BatterLog.Info($"[SafeDeclareWarLogEntryPatch] Intercepted with valid talkTroop: {talkTroop.Name}");
            comment = "No comment available.";
            score = ImportanceEnum.Zero;

            return false;
        }
        catch (Exception ex) {
            BatterLog.Info($"[SafeDeclareWarLogEntryPatch] Exception caught in prefix: {ex}");
            comment = string.Empty;
            score = ImportanceEnum.Zero;
            return false;
        }
    }
}