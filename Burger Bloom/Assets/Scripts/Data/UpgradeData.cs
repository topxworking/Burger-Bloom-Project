using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "BurgerBloom/UpgradeData")]
public class UpgradeData : ScriptableObject
{
    [Header("Ingredient Upgrades")]
    public int bunLevel = 1;
    public int meatLevel = 1;
    public int[] upgradeCost = { 50, 90, 150, 250, 400 };

    [Header("Shop Upgrades")]
    public float cookTimeBase = 5f;
    public float cookTimeReduce = 0.5f;
    public float burnTimeBase = 4f;
    public float burnTimeIncrease = 0.5f;
    public float walkSpeedBase = 4f;
    public float walkSpeedBonus = 0.5f;

    public int cookUpgradeCost = 100;
    public int burnUpgradeCost = 100;
    public int speedUpgradeCost = 80;

    public int cookLevel = 0;
    public int burnLevel = 0;
    public int speedLevel = 0;
    public int maxShopLevel = 5;

    public int GetBurgerPrice()
    {
        float basePrice = 50f;

        float bunMultiplier = Mathf.Pow(1.10f, bunLevel - 1);   // +10% ต่อเลเวล
        float meatMultiplier = Mathf.Pow(1.15f, meatLevel - 1); // +15% ต่อเลเวล

        float finalPrice = basePrice * bunMultiplier * meatMultiplier;

        return Mathf.RoundToInt(finalPrice);
    }

    public float GetCookTime() => Mathf.Max(1f, cookTimeBase - cookLevel * cookTimeReduce);
    public float GetBurnTime() => burnTimeBase + burnLevel * burnTimeIncrease;
    public float GetWalkSpeed() => walkSpeedBase + speedLevel * walkSpeedBonus;
}