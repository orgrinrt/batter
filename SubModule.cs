using System.Reflection;
using System.Xml;
using HarmonyLib;
using SafeWarLogPatch.Behaviours;
using SafeWarLogPatch.Models;
using SafeWarLogPatch.Patches;
using SafeWarLogPatch.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace SafeWarLogPatch;

public class SubModule : MBSubModuleBase
{
    private static bool _logInitialized;
    
    private const string ModName = "Batter";
    private static readonly string ModNameLower = ModName.ToLower();

    protected override void OnSubModuleLoad()
    {
        BatterLog.Hr(nameof(OnSubModuleLoad) + " START");

        base.OnSubModuleLoad();

        if (!_logInitialized)
        {
            BatterLog.Info("Started OnSubmoduleLoad");
            _logInitialized = true;
        }

        var harmony = new Harmony($"mod.{ModNameLower}");

        TryPatch(harmony, typeof(EventDispatcherOnWarDeclaredPatch));
        TryPatch(harmony, typeof(DiplomacyModHasMarriedClanLeaderRelationPatch));
        TryPatch(harmony, typeof(HeroIsHumanPlayerCharacterPatch));
        TryPatch(harmony, typeof(HeroIsFriendPatch));
        TryPatch(harmony, typeof(LogEntryHistoryGetRelevantCommentPatch));
        TryPatch(harmony, typeof(DeclareWarLogEntryGetConversationScoreAndCommentPatch));
        TryPatch(harmony, typeof(DefaultDiplomacyModelGetScoreOfDeclaringWarPatch));
        TryPatch(harmony, typeof(DefaultDiplomacyModelGetScoreOfDeclaringWarInternalPatch));
        TryPatch(harmony, typeof(ClanGetRelationWithClanPatch));
        TryPatch(harmony, typeof(VillagerCampaignBehaviourOnSettlementEnteredPatch));
        TryPatch(harmony, typeof(CampaignEventsOnSettlementEnteredPatch));
        TryPatch(harmony, typeof(VillagerCampaignBehvaiourHourlyTickPartyPatch));
        TryPatch(harmony, typeof(CampaignEventsHourlyTickPartyPatch));
        TryPatch(harmony, typeof(CampaignEventsDailyTickClanPatch));
        TryPatch(harmony, typeof(CampaignEventsDailyTickSettlementPatch));
        TryPatch(harmony, typeof(TownMarketDataGetPricePatch));
        TryPatch(harmony, typeof(WorkshopsCampaignBehaviorProduceAnOutputToTownPatch));
        TryPatch(harmony, typeof(TournamentCachePossibleEliteRewardItemsPatch));


        Action<string, Action> patchRunner = (name, apply)
            => TryPatch(harmony, name, apply);

        // patch all roster getters, logging each one uniformly:
        ItemRosterGetterPatches.PatchAll(harmony, patchRunner);

        BatterLog.Hr(nameof(OnSubModuleLoad) + " END");
    }

    protected override void OnGameStart(Game game, IGameStarter starterObject)
    {
        BatterLog.Hr(nameof(OnGameStart) + " START");

        base.OnGameStart(game, starterObject);
        //HeroDiagnostics.DumpBrokenHeroes();
        try
        {
            if (game.GameType is Campaign)
            {
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
        catch (Exception ex)
        {
            BatterLog.Error("     >> failed to initialize submodule:\n {ex}");
        }

        BatterLog.Hr(nameof(OnGameStart) + " END");
    }

    protected override void OnSubModuleUnloaded()
    {
        BatterLog.Hr(nameof(OnSubModuleUnloaded) + " START");

        BatterLog.Shutdown();

        BatterLog.Hr(nameof(OnSubModuleUnloaded) + " END");
    }

    private void DumpMergedXmlCache()
    {
        var mgr = MBObjectManager.Instance;
        foreach (var f in typeof(MBObjectManager)
                     .GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            if (typeof(IDictionary<string, XmlDocument>).IsAssignableFrom(f.FieldType))
            {
                var dict = f.GetValue(mgr) as IDictionary<string, XmlDocument>;
                if (dict != null)
                    foreach (var key in dict.Keys)
                        InformationManager.DisplayMessage(
                            new InformationMessage($"[XML Cache] key: {key}"));
            }
    }


    private void TryPatch(Harmony harmony, Type patchType)
    {
        try
        {
            harmony.CreateClassProcessor(patchType).Patch();
            BatterLog.Info($"✅ Patched: {patchType.Name}");
        }
        catch (Exception ex)
        {
            BatterLog.Info($"❌ Failed to patch {patchType.Name}: {ex}");
        }
    }

    private void TryPatch(Harmony harmony, string patchName, Action apply)
    {
        try
        {
            apply();
            BatterLog.Info($"✅ Patched: {patchName}");
        }
        catch (Exception ex)
        {
            BatterLog.Info($"❌ Failed to patch {patchName}: {ex}");
        }
    }
}

public class ClanNameGenerator
{
    public static TextObject GenerateClanNameForHero(Hero hero)
    {
        var culture = hero.Culture;
        var originSettlement = hero.CurrentSettlement;

        // Generate a clan name based on the hero's culture and current settlement
        var clanName = NameGenerator.Current.GenerateClanName(culture, originSettlement);

        return clanName;
    }
}