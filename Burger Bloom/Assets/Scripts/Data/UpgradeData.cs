using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "BurgerBloom/UpgradeData")]
public class UpgradeData : ScriptableObject
{
    [Header("Ingredient Upgrades")]
    public int bunLevel = 1;
    public int meatLevel = 1;
    public int[] upgradeCost = { 50, 100, 200 };

    [Header("Shop Upgrades")]
    public float cookTimeBase = 5f;
    public float cookTimeReduce = 0.5f;
    public float burnTimeBase = 4f;
    public float burnTimeIncrease = 0.5f;
    public float walkSpeedBase = 4f;
    public float walkSpeedBonus = 0.5f;

    public int cookUpgradeCost = 80;
    public int burnUpgradeCost = 80;
    public int speedUpgradeCost = 60;

    public int cookLevel = 0;
    public int burnLevel = 0;
    public int speedLevel = 0;
    public int maxShopLevel = 3;

    public int GetBurgerPrice()
    {
        int base_price = 50;
        int bunBonus = (bunLevel - 1) * 20;
        int meatBonus = (meatLevel - 1) * 30;
        return base_price + bunBonus + meatBonus;
    }

    public float GetCookTime() => Mathf.Max(1f, cookTimeBase - cookLevel * cookTimeReduce);
    public float GetBurnTime() => burnTimeBase + burnLevel * burnTimeIncrease;
    public float GetWalkSpeed() => walkSpeedBase + speedLevel * walkSpeedBonus;
}