using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IngredientDatabase", menuName = "ScriptableObjects/Ingredient Database")]
public class IngredientDatabase : ScriptableObject
{
    [SerializeField] private List<IngredientData> _ingredients = new();

    private Dictionary<IngredientType, IngredientData> _lookup;

    private void OnEnable()
    {
        BuildLookup();
    }

    private void BuildLookup()
    {
        _lookup = new Dictionary<IngredientType, IngredientData>();
        foreach (var d in _ingredients)
            if (d != null) _lookup[d.Type] = d;
    }

    public IngredientData Get(IngredientType type)
    {
        if (_lookup == null) BuildLookup();
        return _lookup.TryGetValue(type, out var d) ? d : null;
    }

    public List<IngredientData> GetUnlocked(int playerLevel)
    {
        var result = new List<IngredientData>();
        foreach (var d in _ingredients)
            if (d != null && d.UnlockLevel <= playerLevel)
                result.Add(d);
        return result;
    }

    public bool IsUnlocked(IngredientType type, int playerLevel)
    {
        var d = Get(type);
        return d != null && d.UnlockLevel <= playerLevel;
    }

    public Ingredient SpawnIngredient(IngredientType type, Vector3 position, Transform parent = null)
    {
        var date = Get(type);
        if (date == null || date.Prefab == null) return null;

        var go = Instantiate(date.Prefab, position, Quaternion.identity, parent);
        var ing = go.GetComponent<Ingredient>();
        return ing;
    }
}
