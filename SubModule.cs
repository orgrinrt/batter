using TaleWorlds.MountAndBlade;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using System.Linq;
using System;
using System.IO;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem.LogEntries;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using System.Linq;
using System;
using System.Reflection;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.CampaignSystem.LogEntries;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TaleWorlds.Core;
using System;
using System.IO;
using System.Xml.Serialization;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;
using System.Collections;
using System.Collections.Generic;
using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.Library;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using System.Linq;
using System;
using System.Reflection;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem.LogEntries;
using System.Collections;
using System.Collections.Generic;



namespace SafeWarLogPatch
{
    public class SubModule : MBSubModuleBase
    {
        private static bool _logInitialized = false;

        protected override void OnSubModuleLoad()
        {
            SafeLog.Hr(nameof(OnSubModuleLoad) + " START");

            base.OnSubModuleLoad();

            if (!_logInitialized)
            {
                SafeLog.Info("Started OnSubmoduleLoad");
                _logInitialized = true;
            }

            var harmony = new Harmony("mod.safewarlogpatch");

            TryPatch(harmony, typeof(WarLogSafePatch));
            TryPatch(harmony, typeof(SafeClanExtensionsPatch));
            TryPatch(harmony, typeof(RecruitLogSuppressorPatch));
            TryPatch(harmony, typeof(RecruitLogFilterPatch));
            TryPatch(harmony, typeof(HeroConvSafePatch));
            TryPatch(harmony, typeof(SafeIsFriendPatch));
            TryPatch(harmony, typeof(SafeLogEntryHistoryPatch));
            TryPatch(harmony, typeof(SafeDeclareWarLogEntryPatch));
            TryPatch(harmony, typeof(SafeScoreOfDeclaringWarPatch));
            TryPatch(harmony, typeof(SafeGetScoreOfWarInternalPatch));
            TryPatch(harmony, typeof(SafeClanRelationPatch));
            TryPatch(harmony, typeof(SafeVillagerSettlementEntryPatch));
            TryPatch(harmony, typeof(SafeVillagerSettlementEntryPatchLate));
            TryPatch(harmony, typeof(SafeVillagerHourlyTickPatch));
            TryPatch(harmony, typeof(SafeVillagerHourlyTickPatchLate));
            TryPatch(harmony, typeof(RecruitLogFilterPatchAdd));
            TryPatch(harmony, typeof(DailyTickClanPatch));
            TryPatch(harmony, typeof(SafeDailyTickSettlementPatch));
            TryPatch(harmony, typeof(TownMarketDataGetPricePatch));
            TryPatch(harmony, typeof(WorkshopsCampaignBehaviorProduce));
            TryPatch(harmony, typeof(TournamentPrizeCrash));


            Action<string, Action> patchRunner = (name, apply)
            => TryPatch(harmony, name, apply);

            // patch all roster getters, logging each one uniformly:
            ItemRosterSafeGetters.PatchAll(harmony, patchRunner);

            SafeLog.Hr(nameof(OnSubModuleLoad) + " END");
        }

        protected override void OnGameStart(Game game, IGameStarter starterObject)
        {
            SafeLog.Hr(nameof(OnGameStart) + " START");

            base.OnGameStart(game, starterObject);
            //HeroDiagnostics.DumpBrokenHeroes();
            try {
                if (game.GameType is Campaign)
                {
                    var campaignStarter = (CampaignGameStarter)starterObject;
                    SafeLog.Info(" >> Adding ComputedPriceRegistry behaviour...");
                    campaignStarter.AddBehavior(ComputedPriceRegistry.Instance);
                    SafeLog.Info("      >> success!");

                    SafeLog.Info(" >> Adding HeroClanSanitizerBehavior behaviour...");
                    campaignStarter.AddBehavior(new HeroClanSanitizerBehavior());
                    SafeLog.Info("      >> success!");

                    SafeLog.Info(" >> Adding OrgrinsItemValueModel model...");
                    campaignStarter.AddModel((GameModel)new OrgrinsItemValueModel());
                    SafeLog.Info("      >> success!");

                    // DumpMergedXmlCache();

                }

            } catch (Exception ex) {
                    SafeLog.Error("     >> failed to initialize submodule:\n {ex}");
                }

                SafeLog.Hr(nameof(OnGameStart) + " END");

        }

        protected override void OnSubModuleUnloaded()
        {
            SafeLog.Hr(nameof(OnSubModuleUnloaded) + " START");

            SafeLog.Shutdown();

            SafeLog.Hr(nameof(OnSubModuleUnloaded) + " END");

        }

        private void DumpMergedXmlCache()
        {
            var mgr = MBObjectManager.Instance;
            foreach (var f in typeof(MBObjectManager)
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (typeof(IDictionary<string, XmlDocument>).IsAssignableFrom(f.FieldType))
                {
                    var dict = f.GetValue(mgr) as IDictionary<string, XmlDocument>;
                    if (dict != null)
                    {
                        foreach (var key in dict.Keys)
                        {
                            InformationManager.DisplayMessage(
                                new InformationMessage($"[XML Cache] key: {key}"));
                        }
                    }
                }
            }
        }


        private void TryPatch(Harmony harmony, Type patchType)
        {
            try
            {
                harmony.CreateClassProcessor(patchType).Patch();
                SafeLog.Info($"✅ Patched: {patchType.Name}");
            }
            catch (Exception ex)
            {
                SafeLog.Info($"❌ Failed to patch {patchType.Name}: {ex}");
            }
        }
        private void TryPatch(Harmony harmony, string patchName, Action apply)
        {
            try
            {
                apply();
                SafeLog.Info($"✅ Patched: {patchName}");
            }
            catch (Exception ex)
            {
                SafeLog.Info($"❌ Failed to patch {patchName}: {ex}");
            }
        }


    }

    public class ClanNameGenerator
    {
        public static TextObject GenerateClanNameForHero(Hero hero)
        {
            CultureObject culture = hero.Culture;
            Settlement originSettlement = hero.CurrentSettlement;

            // Generate a clan name based on the hero's culture and current settlement
            TextObject clanName = NameGenerator.Current.GenerateClanName(culture, originSettlement);

            return clanName;
        }
    }

}



