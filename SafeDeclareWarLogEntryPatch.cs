using HarmonyLib;
using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;

namespace SafeWarLogPatch
{




    [HarmonyPatch(typeof(LogEntryHistory), "GetRelevantComment")]
    public static class SafeLogEntryHistoryPatch
    {
        static bool Prefix(Hero conversationHero, ref int bestScore, ref string bestRelatedLogEntryTag)
        {
            if (conversationHero == null || conversationHero.Clan == null || conversationHero.MapFaction == null)
            {
                bestScore = 0;
                bestRelatedLogEntryTag = string.Empty;
                SafeLog.Info($"[SafeLogEntryHistoryPatch] Skipped corrupt hero {conversationHero?.Name}");
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(DeclareWarLogEntry), nameof(DeclareWarLogEntry.GetConversationScoreAndComment))]
    public static class SafeDeclareWarLogEntryPatch
    {
        static bool Prefix(
            DeclareWarLogEntry __instance,
            Hero talkTroop,
            bool findString,
            out string comment,
            out ImportanceEnum score)
        {
            try
            {
                if (talkTroop == null || talkTroop.Clan == null || talkTroop.MapFaction == null)
                {
                    SafeLog.Info($"[SafeDeclareWarLogEntryPatch] Skipping invalid talkTroop: {talkTroop?.Name}");
                    comment = string.Empty;
                    score = ImportanceEnum.Zero;
                    return false; // skip original
                }

                // TEMP: Always skip until stabilized
                SafeLog.Info($"[SafeDeclareWarLogEntryPatch] Intercepted with valid talkTroop: {talkTroop.Name}");
                comment = "No comment available.";
                score = ImportanceEnum.Zero;

                return false;
            }
            catch (Exception ex)
            {
                SafeLog.Info($"[SafeDeclareWarLogEntryPatch] Exception caught in prefix: {ex}");
                comment = string.Empty;
                score = ImportanceEnum.Zero;
                return false;
            }
        }
    }



}
