using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Localization;
using TaleWorlds.Core;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using System.IO;

namespace SafeWarLogPatch
{
    public static class RecruitLogFilterHelper
    {
        public static bool IsRecruitSpamText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            string lower = text.ToLowerInvariant();

            return lower.Contains(" recruited ");
        }
    }

    [HarmonyPatch(typeof(CampaignInformationManager))]
    [HarmonyPatch("NewLogEntryAdded")]
    [HarmonyPatch(new Type[] { typeof(LogEntry) })]
    [HarmonyPriority(Priority.Low)]
    public static class RecruitLogSuppressorPatch
    {

        private static readonly HashSet<string> SeenLogEntryTypes = new();

        static bool Prefix(LogEntry log)
        {
            if (log is not IChatNotification chatEntry)
                return true;

            TextObject text = chatEntry.GetNotificationText();

            if (text == null)
                return true; // or false?

            string message = text.ToString();
            string typeName = log.GetType().FullName ?? "null";

            // Log only when discovering a new log entry type
            if (SeenLogEntryTypes.Add(typeName))
            {
                SafeLog.Info($"🆕 Discovered LogEntry type: {typeName}");
                SafeLog.Info("📜 All known LogEntry types:\n  - " + string.Join("\n  - ", SeenLogEntryTypes));
            }


            if (RecruitLogFilterHelper.IsRecruitSpamText(message))
            {
                //SafeLog.Info($"[RecruitLogSuppressor] Suppressed: {text}");
                return false;
            }

            if (log.GetType().Name.ToLowerInvariant().Contains("recruited"))
                return false;

            return true;
        }
    }

    [HarmonyPatch(typeof(MBInformationManager), nameof(MBInformationManager.AddQuickInformation))]
    [HarmonyPriority(Priority.Low)]
    public static class RecruitLogFilterPatch
    {
        static bool Prefix(TextObject message)
        {
            if (message == null)
                return true;

            if (RecruitLogFilterHelper.IsRecruitSpamText(message.ToString()))
            {
                //SafeLog.Info($"[RecruitLogFilterPatch] Suppressed: {message}");
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch]
    public static class RecruitLogFilterPatchAdd
    {
        /// <summary>
        /// Tell Harmony: “Patch these two overloads of LogEntry.AddLogEntry(...)”
        /// </summary>
        static IEnumerable<MethodBase> TargetMethods()
        {
            // Overload #1: public static void AddLogEntry(LogEntry logEntry)
            yield return AccessTools.Method(
                typeof(LogEntry),
                                            nameof(LogEntry.AddLogEntry),
                                            new Type[] { typeof(LogEntry) }
            );

            // Overload #2: public static void AddLogEntry(LogEntry logEntry, CampaignTime gameTime)
            yield return AccessTools.Method(
                typeof(LogEntry),
                                            nameof(LogEntry.AddLogEntry),
                                            new Type[] { typeof(LogEntry), typeof(CampaignTime) }
            );
        }

        //
        // Both of the following Prefix‐methods will be applied:
        //   • Prefix(LogEntry logEntry) for the single‐parameter overload
        //   • Prefix(LogEntry logEntry, CampaignTime gameTime) for the two‐parameter overload
        //
        // In each case, we simply call a shared helper that contains the actual suppression logic.
        //

        static bool Prefix(LogEntry logEntry)
        {
            return !ShouldSuppress(logEntry);
        }

        // static bool Prefix(LogEntry logEntry, CampaignTime gameTime)
        // {
        //     return !ShouldSuppress(logEntry);
        // }

        /// <summary>
        /// Returns true if this LogEntry’s text reads like “... recruited ...”,
        /// in which case we want to suppress it (i.e. return false from Prefix).
        /// </summary>
        private static bool ShouldSuppress(LogEntry logEntry)
        {
            string text = (logEntry is IChatNotification chat)
            ? chat.GetNotificationText().ToString()
            : logEntry.ToString();
            if (text.Contains("recruited")) {
                // skip adding the log entry (filter it out)
                return false;
            }

            if (text == null)
                return false;

            string message = text.ToString().ToLowerInvariant();
            return !message.Contains(" recruited ");
        }
    }
}
