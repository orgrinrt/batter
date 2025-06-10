using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Batter.Core.Behaviours;
using Batter.Core.Models;
using Batter.Core.Patches;
using Batter.Core.Utils;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace Batter.Core;

public class SubModule : MBSubModuleBase {
    private const string MOD_NAME = "Batter";
    private static bool _logInitialized;
    private static readonly string MOD_NAME_LOWER = SubModule.MOD_NAME.ToLower();

    protected override void OnSubModuleLoad() {
        BatterLog.Hr(nameof(SubModule.OnSubModuleLoad) + " START");

        base.OnSubModuleLoad();

        if (!SubModule._logInitialized) {
            BatterLog.Info("Started OnSubmoduleLoad");
            SubModule._logInitialized = true;
        }

        var harmony = new Harmony($"mod.{SubModule.MOD_NAME_LOWER}");

        this.TryPatch(harmony, typeof(EventDispatcherOnWarDeclaredPatch));
        this.TryPatch(harmony, typeof(DiplomacyModHasMarriedClanLeaderRelationPatch));
        this.TryPatch(harmony, typeof(HeroIsHumanPlayerCharacterPatch));
        this.TryPatch(harmony, typeof(HeroIsFriendPatch));
        this.TryPatch(harmony, typeof(LogEntryHistoryGetRelevantCommentPatch));
        this.TryPatch(harmony, typeof(DeclareWarLogEntryGetConversationScoreAndCommentPatch));
        this.TryPatch(harmony, typeof(DefaultDiplomacyModelGetScoreOfDeclaringWarPatch));
        this.TryPatch(harmony, typeof(DefaultDiplomacyModelGetScoreOfDeclaringWarInternalPatch));
        this.TryPatch(harmony, typeof(ClanGetRelationWithClanPatch));
        this.TryPatch(harmony, typeof(VillagerCampaignBehaviourOnSettlementEnteredPatch));
        this.TryPatch(harmony, typeof(CampaignEventsOnSettlementEnteredPatch));
        this.TryPatch(harmony, typeof(VillagerCampaignBehvaiourHourlyTickPartyPatch));
        this.TryPatch(harmony, typeof(CampaignEventsHourlyTickPartyPatch));
        this.TryPatch(harmony, typeof(CampaignEventsDailyTickClanPatch));
        this.TryPatch(harmony, typeof(CampaignEventsDailyTickSettlementPatch));
        this.TryPatch(harmony, typeof(TownMarketDataGetPricePatch));
        this.TryPatch(harmony, typeof(WorkshopsCampaignBehaviorProduceAnOutputToTownPatch));
        this.TryPatch(harmony, typeof(TournamentCachePossibleEliteRewardItemsPatch));


        Action<string, Action> patchRunner = (name, apply)
            => this.TryPatch(harmony, name, apply);

        // patch all roster getters, logging each one uniformly:
        ItemRosterGetterPatches.PatchAll(harmony, patchRunner);

        BatterLog.Hr(nameof(SubModule.OnSubModuleLoad) + " END");
    }

    protected override void OnGameStart(Game game, IGameStarter starterObject) {
        BatterLog.Hr(nameof(SubModule.OnGameStart) + " START");

        base.OnGameStart(game, starterObject);
        //HeroDiagnostics.DumpBrokenHeroes();
        try {
            if (game.GameType is Campaign) {
                var campaignStarter = (CampaignGameStarter)starterObject;
                BatterLog.Info(" >> Adding ComputedPriceRegistry behaviour...");
                campaignStarter.AddBehavior(ComputedPriceRegistry.Instance);
                BatterLog.Info("      >> success!");

                BatterLog.Info(" >> Adding HeroClanSanitizerBehavior behaviour...");
                campaignStarter.AddBehavior(new HeroClanSanitizerBehavior());
                BatterLog.Info("      >> success!");

                BatterLog.Info(" >> Adding OrgrinsItemValueModel model...");
                campaignStarter.AddModel(new OrgrinsItemValueModel());
                BatterLog.Info("      >> success!");

                // DumpMergedXmlCache();
            }
        }
        catch (Exception ex) {
            BatterLog.Error("     >> failed to initialize submodule:\n {ex}");
        }

        BatterLog.Hr(nameof(SubModule.OnGameStart) + " END");
    }

    protected override void OnSubModuleUnloaded() {
        BatterLog.Hr(nameof(SubModule.OnSubModuleUnloaded) + " START");

        BatterLog.Shutdown();

        BatterLog.Hr(nameof(SubModule.OnSubModuleUnloaded) + " END");
    }

    private void DumpMergedXmlCache() {
        var mgr = MBObjectManager.Instance;
        foreach (var f in typeof(MBObjectManager)
                     .GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            if (typeof(IDictionary<string, XmlDocument>).IsAssignableFrom(f.FieldType)) {
                if (f.GetValue(mgr) is not IDictionary<string, XmlDocument> dict) continue;
                foreach (var key in dict.Keys)
                    InformationManager.DisplayMessage(
                        new($"[XML Cache] key: {key}"));
            }
    }


    private void TryPatch(Harmony harmony, Type patchType) {
        try {
            harmony.CreateClassProcessor(patchType).Patch();
            BatterLog.Info($"✅ Patched: {patchType.Name}");
        }
        catch (Exception ex) {
            BatterLog.Info($"❌ Failed to patch {patchType.Name}: {ex}");
        }
    }

    private void TryPatch(Harmony harmony, string patchName, Action apply) {
        try {
            apply();
            BatterLog.Info($"✅ Patched: {patchName}");
        }
        catch (Exception ex) {
            BatterLog.Info($"❌ Failed to patch {patchName}: {ex}");
        }
    }
}