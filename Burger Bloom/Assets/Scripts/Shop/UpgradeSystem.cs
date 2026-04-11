using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UpgradeEntry
{
    public string UpgradeId;
    public int PurchaseCount;
}

public class UpgradeSystem : Singleton<UpgradeSystem>
{
    private Dictionary<string, int> _purchased = new();

    public const string EXTRA_GRILL_SLOT = "ExtraGrillSlot";
    public const string FASTER_COOKING = "FasterCooking";
    public const string LARGER_DELIVERY = "LargerDelivery";
    public const string BETTER_TIP = "BetterTip";
    public const string EXTRA_PATIENCE = "ExtraPatience";

    public int ExtraGrillSlots => GetCount(EXTRA_GRILL_SLOT);

    public float CookingSpeedMultiplier =>
        1f + GetCount(FASTER_COOKING) * 0.15f;

    public int DeliveryUnits =>
        10 + GetCount(LARGER_DELIVERY) * 5;

    public float TipMultiplier =>
        1f + GetCount(BETTER_TIP) * 0.2f;

    public float ExtraPatienceSeconds =>
        GetCount(EXTRA_PATIENCE) * 15f;


    public bool CanPurchase(ShopItem item)
    {
        if (!item.IsUpgrade) return true;
        int count = GetCount(item.UpgradeId);
        return count < item.MaxPurchases &&
               GameManager.Instance.Level >= item.UnlockLevel;
    }

    public bool Purchase(ShopItem item)
    {
        if (!item.IsUpgrade) return false;
        if (!CanPurchase(item)) return false;
        if (!GameManager.Instance.SpendMoney(item.PricePerBox)) return false;

        if (!_purchased.ContainsKey(item.UpgradeId))
            _purchased[item.UpgradeId] = 0;
        _purchased[item.UpgradeId]++;

        Debug.Log($"[Upgrade] Purchased {item.UpgradeId} (x{_purchased[item.UpgradeId]})");
        return true;
    }

    public int GetCount(string upgradeId)
        => _purchased.TryGetValue(upgradeId, out int c) ? c : 0;

    public List<UpgradeEntry> Serialize()
    {
        var list = new List<UpgradeEntry>();
        foreach (var kv in _purchased)
            list.Add(new UpgradeEntry { UpgradeId = kv.Key, PurchaseCount = kv.Value });
        return list;
    }

    public void Deserialize(List<UpgradeEntry> entries)
    {
        _purchased.Clear();
        foreach (var e in entries)
            _purchased[e.UpgradeId] = e.PurchaseCount;
    }
}
