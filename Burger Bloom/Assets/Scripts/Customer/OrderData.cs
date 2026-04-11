using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OrderData_", menuName = "ScriptableObjects/Order Template")]
public class OrderData : ScriptableObject
{
    [Header("Info")]
    public string OrderName;
    public Sprite OrderIcon;

    [Header("Bun")]
    public IngredientType Bun = IngredientType.RegularBun;

    [Header("Protein")]
    public IngredientType Protein = IngredientType.BeefPatty;

    [Header("Vegetables (in order)")]
    public List<IngredientType> RequiredIngredients = new();

    [Header("Sauces")]
    public List<string> RequiredSauces = new();

    [Header("Economy")]
    public float BasePrice = 50f;
    public float Patience = 60f;   // seconds customer waits
    public int UnlockLevel = 1;

    private static readonly IngredientType[] _buns = { IngredientType.RegularBun, IngredientType.SesameBun, IngredientType.BriocheBun };
    private static readonly IngredientType[] _proteins = { IngredientType.BeefPatty, IngredientType.ChickenPatty };
    private static readonly IngredientType[] _veggies = { IngredientType.Lettuce, IngredientType.Tomato, IngredientType.Onion, IngredientType.Pickle };
    private static readonly string[] _sauces = { "Ketchup", "Mustard", "Mayo", "BBQ", "Chili", "Garlic" };

    public static OrderData GenerateRandom(int playerLevel)
    {
        var data = CreateInstance<OrderData>();
        data.OrderName = "Random Order";
        data.Bun = _buns[UnityEngine.Random.Range(0, Mathf.Min(playerLevel, _buns.Length))];
        data.Protein = _proteins[0]; // expand with level

        // 1–3 veggies
        int veggieCount = UnityEngine.Random.Range(0, Mathf.Min(3, playerLevel) + 1);
        var pool = new List<IngredientType>(_veggies);
        for (int i = 0; i < veggieCount; i++)
        {
            int idx = UnityEngine.Random.Range(0, pool.Count);
            data.RequiredIngredients.Add(pool[idx]);
            pool.RemoveAt(idx);
        }

        // 1–2 sauces
        int sauceCount = UnityEngine.Random.Range(1, Mathf.Min(2, playerLevel) + 1);
        var sPool = new List<string>(_sauces);
        for (int i = 0; i < sauceCount; i++)
        {
            int idx = UnityEngine.Random.Range(0, sPool.Count);
            data.RequiredSauces.Add(sPool[idx]);
            sPool.RemoveAt(idx);
        }

        // Price based on complexity
        data.BasePrice = 40f + (data.RequiredIngredients.Count * 5f) + (data.RequiredSauces.Count * 3f);
        data.Patience = 90f - (playerLevel * 5f); // harder levels = less patience

        return data;
    }
}
