using UnityEngine;

public enum IngredientType
{
    // Buns
    RegularBun, SesameBun, MholeMealBun, BriocheBun,
    // Proteins
    BeefPatty, ChickenPatty, FishFillet, VeggiePatty,
    // Vegetables
    Lettuce, Tomato, Onion, Pickle, Jalapeno,
    // Sauces
    Ketchup, Mustard, Mayo, BBQSauce, ChiliSauce, GarlicSauce,
    // Extras
    Cheese, BaconStrip, FriedEgg
}

[CreateAssetMenu(fileName = "Ingredient_", menuName = "Burger Bloom/Ingredient Data")]
public class IngredientData : ScriptableObject
{
    public IngredientType Type;
    public string DisplayName;
    [TextArea] public string Description;
    public Sprite Icon;
    public GameObject Prefab;
    public int UnlockLevel = 1;
    public float ShopPrice = 20f;
    public bool IsGrillable = false;
    public bool IsSauce = false;
    public Color SauceColor = Color.red;
}
