using Batter.Core.Behaviours;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Batter.Core.Models;

public class OrgrinsItemValueModel : DefaultItemValueModel {
    public override Int32 CalculateValue(ItemObject item) {
        var vanillaValue = base.CalculateValue(item);

        // look up computed price from registry
        var computedPrice = ComputedPriceRegistry.Instance.GetPrice(item.StringId);

        // Combine: XSLT-computed price acts as override/adjustment
        return Math.Max(0, vanillaValue + computedPrice);
    }

    public override Single CalculateTier(ItemObject item) {
        var vanillaTier = base.CalculateTier(item);

        if (ComputedPriceRegistry.Instance.TryGetPrice(item.StringId, out var price)) {
            // Example: assume price range maps from 0 to 75k → tier 0.5–7
            var priceFactor = MathF.Clamp(price / 75000f, 0f, 1f);
            var priceBasedTier = priceFactor * 6.5f; // adjust tier delta max
            return MathF.Min(7f, (vanillaTier + 2 * priceBasedTier) / 3f);
        }

        return vanillaTier;
    }

    public override Single GetEquipmentValueFromTier(Single itemTierf) {
        return base.GetEquipmentValueFromTier(itemTierf);
    }
}