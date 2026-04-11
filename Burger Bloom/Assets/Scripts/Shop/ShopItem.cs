using UnityEngine;

public enum ShopCategory { Ingredients, Equipment, Upgrades }

[CreateAssetMenu(fileName = "ShopItem_", menuName = "Burger Bloom/Shop Item")]
public class ShopItem : ScriptableObject
{
    [Header("Display")]
    public string DisplayName;
    [TextArea]
    public string Description;
    public Sprite Icon;
    public ShopCategory Category;

    [Header("Unlock")]
    public int UnlockLevel = 1;

    [Header("Pricing")]
    public float PricePerBox = 50f;
    public int UnitsPerBox = 10;

    [Header("Ingredient Link")]
    public IngredientType IngredientType;

    public bool IsUpgrade = false;
    public string UpgradeId = "";
    public int MaxPurchases = 1;
    public float UpgradeValue = 0f;
}
