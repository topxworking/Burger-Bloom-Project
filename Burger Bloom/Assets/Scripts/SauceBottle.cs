using UnityEngine;

public class SauceBottle : Ingredient
{
    protected override void Awake()
    {
        type = IngredientType.Sauce;
        base.Awake();
    }
}
