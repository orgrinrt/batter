using Bannerlord.ButterLib.ObjectSystem.Extensions;
using Batter.Core.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

// ← for SetFlag / HasFlag

namespace Batter.Core.Behaviours;

public class HeroClanSanitizerBehavior : CampaignBehaviorBase {
    private const String SanitizedFlagKey = "SafeWarLogPatch.HeroSanitized";
    private static readonly Random _random = new();

    // Keep track of which heroes we've already sanitized during this session
    private static readonly HashSet<String> _alreadySanitized = new();

    public override void RegisterEvents() {
        CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, this.OnGameLoaded);
    }

    public override void SyncData(IDataStore dataStore) {
        // No persistent data beyond the in‐memory _alreadySanitized set.
    }

    private void OnGameLoaded(CampaignGameStarter starter) {
        BatterLog.Info("[Sanitizer] Running hero data sanitization...");

        var sanitizedCount = 0;
        var skippedCount = 0;

        var allHeroes = Hero.AllAliveHeroes.ToList();

        foreach (var hero in allHeroes) {
            if (hero == null || HeroClanSanitizerBehavior.ShouldSkip(hero)) {
                skippedCount++;
                continue;
            }

            if (HeroClanSanitizerBehavior._alreadySanitized.Contains(hero.StringId)) {
                skippedCount++;
                continue;
            }

            try {
                if (this.SanitizeHero(hero)) {
                    HeroClanSanitizerBehavior._alreadySanitized.Add(hero.StringId);
                    HeroClanSanitizerBehavior.MarkHeroAsSanitized(hero);
                    sanitizedCount++;
                }
                else {
                    skippedCount++;
                }
            }
            catch (Exception ex) {
                BatterLog.Error(
                    $"[Sanitizer] Error processing hero {HeroClanSanitizerBehavior.SafeHeroName(hero)}: {ex}");
            }
        }

        BatterLog.Info($"[Sanitizer] Completed: Sanitized {sanitizedCount} heroes, skipped {skippedCount} heroes.");
    }

    private Boolean SanitizeHero(Hero hero) {
        if (hero.IsWanderer)
            return false;

        var changed = false;

        // 1) Ensure culture
        if (hero.Culture == null || hero.Culture.Name == null || String.IsNullOrWhiteSpace(hero.Culture.Name.Value)) {
            var inferredCulture = this.InferCulture(hero);
            if (inferredCulture != null && !Equals(hero.Culture, inferredCulture)) {
                hero.Culture = inferredCulture;
                BatterLog.Info(
                    $"[Sanitizer] Assigned culture '{inferredCulture.Name}' to hero {HeroClanSanitizerBehavior.SafeHeroName(hero)}");
                changed = true;
            }
        }

        // 2) Generate parents if missing
        changed |= this.GenerateParentsForHero(hero);

        // 3) Assign name if missing/placeholder
        changed |= this.AssignHeroNameIfMissing(hero);

        // 4) Fix clan & faction
        changed |= HeroClanSanitizerBehavior.FixClanAndFaction(hero);

        // 5) Ensure at least one friend
        changed |= this.AssignFriendIfNone(hero);

        return changed;
    }

    #region Inference Methods

    private CultureObject InferCulture(Hero hero) {
        // A) From father or mother
        if (hero.Father?.Culture != null) return hero.Father.Culture;
        if (hero.Mother?.Culture != null) return hero.Mother.Culture;

        // B) From any friendly relation > 30
        foreach (var other in Hero.AllAliveHeroes) {
            if (other == hero) continue;
            var rel = CharacterRelationManager.GetHeroRelation(hero, other);
            if (rel > 30 && other.Culture != null) return other.Culture;
        }

        // C) From hero’s clan
        if (hero.Clan?.Culture != null) return hero.Clan.Culture;

        // D) From hero’s faction (MapFaction)
        if (hero.MapFaction?.Culture != null) return hero.MapFaction.Culture;

        // E) From current settlement’s MapFaction
        var settlement = hero.CurrentSettlement;
        if (settlement != null && settlement.MapFaction?.Culture != null)
            return settlement.MapFaction.Culture;

        // F) From the main hero
        //if (Hero.MainHero?.Culture != null) return Hero.MainHero.Culture;

        // G) Fallback: pick a random culture
        var allCultures = Campaign.Current.ObjectManager.GetObjectTypeList<CultureObject>();
        if (allCultures != null && allCultures.Count > 0)
            return allCultures[HeroClanSanitizerBehavior._random.Next(allCultures.Count)];

        return null;
    }

    #endregion

    #region Parents Generation

    private Boolean GenerateParentsForHero(Hero hero) {
        var changed = false;

        var culture = hero.Culture ?? Hero.MainHero?.Culture;

        if (hero == null || culture == null)
            return false;

        if (hero.Father == null) {
            var newDad = this.CreateNewRandomHero(culture);
            if (newDad != null) {
                // Immediately mark parent as dead so they do not take part in ticks
                newDad.ChangeState(Hero.CharacterStates.Dead);

                hero.Father = newDad;
                BatterLog.Info(
                    $"[Sanitizer] Created father '{HeroClanSanitizerBehavior.SafeHeroName(newDad)}' for {HeroClanSanitizerBehavior.SafeHeroName(hero)}");
                changed = true;
            }
        }

        var newMom = this.CreateNewRandomHero(culture);
        if (newMom != null) {
            newMom.ChangeState(Hero.CharacterStates.Dead);

            hero.Mother = newMom;
            BatterLog.Info(
                $"[Sanitizer] Created mother '{HeroClanSanitizerBehavior.SafeHeroName(newMom)}' for {HeroClanSanitizerBehavior.SafeHeroName(hero)}");
            changed = true;
        }

        return changed;
    }

    #endregion

    #region Friend Assignment

    private Boolean AssignFriendIfNone(Hero hero) {
        // If the hero already has any relation > 30, skip
        foreach (var other in Hero.AllAliveHeroes) {
            if (other == hero) continue;
            if (CharacterRelationManager.GetHeroRelation(hero, other) > 30)
                return false;
        }

        var culture = hero.Culture ?? Hero.MainHero?.Culture;
        if (culture == null) return false;

        var friend = this.CreateNewRandomHero(culture);
        if (friend == null) return false;

        // If they already share a relation, skip
        if (HeroClanSanitizerBehavior.AreAlreadyFriends(hero, friend)) return false;

        // Ensure friend has a valid clan/faction
        HeroClanSanitizerBehavior.FixClanAndFaction(friend);

        // Set mutual +40 relation
        CharacterRelationManager.SetHeroRelation(hero, friend, 40);
        CharacterRelationManager.SetHeroRelation(friend, hero, 40);

        BatterLog.Info(
            $"[Sanitizer] Assigned new friend '{HeroClanSanitizerBehavior.SafeHeroName(friend)}' to hero {HeroClanSanitizerBehavior.SafeHeroName(hero)}");
        return true;
    }

    #endregion

    #region Name Assignment

    private Boolean AssignHeroNameIfMissing(Hero hero) {
        if (hero.Name != null && !this.IsPlaceholderName(hero.Name.ToString()))
            return false;

        // Ensure we have a culture to hand to the generator
        var culture = hero.Culture ?? Hero.MainHero?.Culture;
        if (culture == null) {
            BatterLog.Warning(
                $"[Sanitizer] Cannot assign name: no culture for {HeroClanSanitizerBehavior.SafeHeroName(hero)}");
            return false;
        }

        // This API will fill both 'firstName' and 'fullName' appropriately.
        NameGenerator.Current.GenerateHeroNameAndHeroFullName(
            hero,
            out var firstName,
            out var fullName,
            false // or true if you want deterministic
        );

        if (fullName != null) {
            // Note: SetName expects (fullName, firstName). If you only care about fullName, pass it twice.
            hero.SetName(fullName, firstName ?? fullName);
            BatterLog.Info(
                $"[Sanitizer] Assigned generated name '{fullName}' to hero {HeroClanSanitizerBehavior.SafeHeroName(hero)}");
            return true;
        }

        BatterLog.Warning(
            $"[Sanitizer] NameGenerator produced no name for {HeroClanSanitizerBehavior.SafeHeroName(hero)}");
        return false;
    }

    private Boolean IsPlaceholderName(String name) {
        if (String.IsNullOrWhiteSpace(name)) return true;
        // Look for “NPCCharacter”, “NewHero”, etc.
        return name.IndexOf("NPCCharacter", StringComparison.OrdinalIgnoreCase) >= 0
               || name.IndexOf("NewHero", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    #endregion

    #region Clan & Faction Repair

    public static Boolean FixClanAndFaction(Hero hero) {
        if (hero.IsWanderer)
            return false;

        var changed = false;

        // 1) Attempt to infer clan from hero or parents
        var clan = hero.Clan ?? hero.Father?.Clan ?? hero.Mother?.Clan;
        // 2) Attempt to infer faction from hero or parents
        var faction = hero.MapFaction ?? hero.Father?.MapFaction ?? hero.Mother?.MapFaction;

        // 3) If no clan, create a fallback one
        if (clan == null) {
            clan = HeroClanSanitizerBehavior.CreateFallbackClanForHero(hero);
            BatterLog.Info($"[Sanitizer] Created fallback clan for {HeroClanSanitizerBehavior.SafeHeroName(hero)}");
            changed = true;
        }

        // 4) If hero.Clan is still null, assign the new clan
        if (hero.Clan == null && clan != null) {
            hero.Clan = clan;
            changed = true;
        }

        // 5) If hero.MapFaction is null, we must assign the clan to a Kingdom
        if (hero.MapFaction == null) {
            Kingdom kingdomToUse = null;

            // a) If the clan already belongs to a kingdom, reuse it
            if (clan.Kingdom != null)
                kingdomToUse = clan.Kingdom;
            else
                // b) Prefer a kingdom that has the same culture
                kingdomToUse = Kingdom.All.FirstOrDefault(k => k.Culture == hero.Culture);

            // c) As a last resort, pick any existing kingdom
            if (kingdomToUse == null && Kingdom.All.Any())
                kingdomToUse = Kingdom.All[HeroClanSanitizerBehavior._random.Next(Kingdom.All.Count)];

            if (kingdomToUse != null) {
                clan.Kingdom = kingdomToUse;
                changed = true;
            }
        }
        else {
            // 6) If hero.MapFaction was already set, ensure clan’s faction matches
            if (faction is Kingdom kf && clan.Kingdom != kf) {
                clan.Kingdom = kf;
                changed = true;
            }
        }

        return changed;
    }

    public static Clan CreateFallbackClanForHero(Hero hero) {
        var culture = hero.Culture ?? Hero.MainHero?.Culture;
        if (culture == null)
            return null;

        // Build a unique string ID for the new clan
        var clanStringId = $"sanitized_clan_{hero.StringId}_{HeroClanSanitizerBehavior._random.Next(10000)}";
        if (Clan.All.Any(c => c.StringId == clanStringId)) clanStringId = $"{clanStringId}_{DateTime.UtcNow.Ticks}";

        // Create the clan object (using the static factory)
        var newClan = Clan.CreateClan(clanStringId);

        // Generate a clan name via NameGenerator:
        // Pass in the hero’s culture and (optionally) the settlement where the clan originates
        var originSettlement = hero.BornSettlement ?? Settlement.All.FirstOrDefault(s => s.Culture == culture);
        var generatedName = NameGenerator.Current.GenerateClanName(culture, originSettlement);
        if (generatedName == null)
            // Fallback to a plain TextObject if generator returned null
            generatedName = new($"Clan_{hero.FirstName}");

        // Initialize the clan with that name
        newClan.InitializeClan(
            generatedName, // formal clan name
            generatedName, // informal clan name (reuse for simplicity)
            culture,
            Banner.CreateRandomBanner()
        );

        // Assign hero as leader and set hero.Clan
        newClan.SetLeader(hero);
        hero.Clan = newClan;

        // Assign the new clan to a Kingdom:
        var chosenKingdom = Kingdom.All.FirstOrDefault(k => k.Culture == culture)
                            ?? Hero.MainHero?.Clan?.Kingdom
                            ?? Kingdom.All.FirstOrDefault();

        if (chosenKingdom != null) newClan.Kingdom = chosenKingdom;

        return newClan;
    }

    #endregion

    #region Helpers

    private Hero CreateNewRandomHero(CultureObject culture) {
        if (culture == null) {
            culture = Hero.MainHero?.Culture;
            if (culture == null)
                return null;
        }

        // 1) Only pick CharacterObject whose culture matches and is a “real” hero template
        var candidates = CharacterObject.All
            .Where(co =>
                co.Culture == culture
                && co.IsHero
                && !co.IsTemplate
                && !co.IsChildTemplate
                && co.IsRegular)
            .ToList();
        if (!candidates.Any())
            return null;

        var template = candidates[HeroClanSanitizerBehavior._random.Next(candidates.Count)];

        // 2) Pick a settlement of that culture (or a random one if none match)
        var bornSettlement = Settlement.All
                                 .FirstOrDefault(s => s.Culture == culture)
                             ?? Settlement.All.GetRandomElement();

        // 3) Age (20±5)
        var ageYears = 20 + HeroClanSanitizerBehavior._random.Next(-5, 6);

        // 4) Create it
        var newHero = HeroCreator.CreateSpecialHero(template, bornSettlement, null, null, ageYears);
        if (newHero == null)
            return null;

        // 5) Make active, sanitize equipment
        newHero.ChangeState(Hero.CharacterStates.Active);
        newHero.CheckInvalidEquipmentsAndReplaceIfNeeded();

        // 6) If still no name, generate via NameGenerator
        if (newHero.Name == null) {
            TextObject firstName, fullName;
            NameGenerator.Current.GenerateHeroNameAndHeroFullName(newHero, out firstName, out fullName);
            if (firstName != null && fullName != null)
                newHero.SetName(fullName, firstName);
        }

        return newHero;
    }


    private static Boolean AreAlreadyFriends(Hero a, Hero b) {
        if (a == null || b == null) return false;
        return CharacterRelationManager.GetHeroRelation(a, b) > 30
               || CharacterRelationManager.GetHeroRelation(b, a) > 30;
    }

    private static Boolean ShouldSkip(Hero hero) {
        if (hero == null) return true;

        // 1) If dead or disabled, skip immediately
        if (hero.HeroState is Hero.CharacterStates.Dead or Hero.CharacterStates.Disabled)
            return true;

        // 2) If we already gave them our “sanitized” flag, skip
        if (hero.HasFlag(HeroClanSanitizerBehavior.SanitizedFlagKey))
            return false; // true; NOTE: this should be true, but had issues, uh oh

        // 4) If it's merely a prisoner, fugitive, wounded, or traveling, skip those too:
        // if (hero.IsPrisoner || hero.IsFugitive || hero.IsWounded || hero.IsTraveling)
        //     return true;

        // 3) Skip pure wanderers (no clan, no political role)
        if (hero.IsWanderer)
            return true;

        // 4) Only touch “political” heroes: lords, notables, minor‐faction heroes, clan leaders, companions, etc.
        //    You can adjust this filter if you want to include/exclude other categories.
        var isPolitical = hero.IsLord
                          || hero.IsMinorFactionHero
                          || hero.IsClanLeader
                          || hero.IsPartyLeader
                          || hero.IsFactionLeader
                          || hero.IsKingdomLeader;

        if (!isPolitical)
            return true;

        return false;
    }

    private static void MarkHeroAsSanitized(Hero hero) {
        if (hero == null) return;
        hero.SetFlag(HeroClanSanitizerBehavior.SanitizedFlagKey); // <- use SetFlag(...) instead of SetCustomProperty
    }

    private static String SafeHeroName(Hero hero) {
        try {
            if (hero == null) return "[null hero]";
            return hero.Name?.ToString() ?? $"[Unnamed: {hero.StringId}]";
        }
        catch {
            return $"[ErrorName:{hero?.StringId}]";
        }
    }

    #endregion
}