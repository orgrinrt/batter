#region

using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

#endregion

namespace Batter.Core.Models;

public class OrgrinsItemValueModel : DefaultItemValueModel {
    public override int CalculateValue(IItemObject item) {
        var vanillaValue = base.CalculateValue(item);

        // Get computed price from registry using direct item reference
        var computedPrice = 0; //ItemValuationRegistry.

        // Combine: computed price acts as override/adjustment
        return Math.Max(0, vanillaValue + computedPrice);
    }

    public override float CalculateTier(IItemObject item) {
        var vanillaTier = base.CalculateTier(item);

        if (ComputedPriceRegistry.Instance.TryGetPrice(item, out var price)) {
            // Example: assume price range maps from 0 to 75k → tier 0.5–7
            var priceFactor = MathF.Clamp(price / 75000f, 0f, 1f);
            var priceBasedTier = priceFactor * 6.5f; // adjust tier delta max
            return MathF.Min(7f, (vanillaTier + 2 * priceBasedTier) / 3f);
        }

        return vanillaTier;
    }

    public override float GetEquipmentValueFromTier(float itemTierf) {
        return base.GetEquipmentValueFromTier(itemTierf);
    }
}