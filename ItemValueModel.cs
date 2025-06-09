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
using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Library;

namespace SafeWarLogPatch
{
    public class OrgrinsItemValueModel : DefaultItemValueModel
    {
        public override int CalculateValue(ItemObject item)
        {
            int vanillaValue = base.CalculateValue(item);

            // look up computed price from registry
            int computedPrice = ComputedPriceRegistry.Instance.GetPrice(item.StringId);

            // Combine: XSLT-computed price acts as override/adjustment
            return Math.Max(0, vanillaValue + computedPrice);
        }

        public override float CalculateTier(ItemObject item)
        {
            float vanillaTier = base.CalculateTier(item);

            if (ComputedPriceRegistry.Instance.TryGetPrice(item.StringId, out int price))
            {
                // Example: assume price range maps from 0 to 75k → tier 0.5–7
                float priceFactor = MathF.Clamp(price / 75000f, 0f, 1f);
                float priceBasedTier = priceFactor * 6.5f; // adjust tier delta max
                return MathF.Min(7f, (vanillaTier + (2 * priceBasedTier)) / 3f);
            }

            return vanillaTier;
        }

        public override float GetEquipmentValueFromTier(float itemTierf)
        {
            return base.GetEquipmentValueFromTier(itemTierf);
        }
    }
}
