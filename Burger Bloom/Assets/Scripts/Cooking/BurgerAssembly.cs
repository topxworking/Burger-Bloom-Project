using System.Collections.Generic;
using UnityEngine;

public class BurgerAssembly : MonoBehaviour, IInteractable
{
    [Header("Stack Settings")]
    [SerializeField] private Transform _stackBase;
    [SerializeField] private float _layerHeight = 0.03f;

    private readonly List<IngredientType> _stackedTypes = new();
    private readonly List<ISauceStrategy> _sauces = new();
    private readonly List<GameObject> _visualStack = new();

    public IReadOnlyList<IngredientType> ingredientTypes => _stackedTypes;
    public IReadOnlyList<ISauceStrategy> sauces => _sauces;
    public bool IsEmpty => _stackedTypes.Count == 0;

    public string GetPromptText() => IsEmpty ? "Pick up Place [E]" : "Pick up Burger [E]";
    public bool CanInteract(PlayerInteract player) => !player.Hands.IsHolding;

    public void Interact(PlayerInteract player)
    {
        var wrapper = GetComponent<BurgerAssemblyIngredient>();
        if (wrapper != null) player.Hands.PickUp(wrapper);
    }

    public bool AddIngredient(Ingredient ingredient)
    {
        _stackedTypes.Add(ingredient.Type);

        int idx = _visualStack.Count;
        float y = idx * _layerHeight;
        ingredient.transform.SetParent(_stackBase);
        ingredient.transform.localPosition = new Vector3(0, y, 0);
        ingredient.transform.localRotation = Quaternion.identity;
        _visualStack.Add(ingredient.gameObject);
        ingredient.OnPlaced();
        return true;
    }

    public void AddSauce(ISauceStrategy sauce)
    {
        _sauces.Add(sauce);

        Debug.Log($"Added {sauce.Name} sauce");
    }

    public void Clear()
    {
        foreach (var go in _visualStack) Destroy(go);
        _visualStack.Clear();
        _stackedTypes.Clear();
        _sauces.Clear();
    }

    public float ScoreAgainstOrder(OrderData order)
    {
        float score = 0f;
        int total = order.RequiredIngredients.Count + order.RequiredSauces.Count;
        if (total == 0) return 1f;

        // Check ingredients
        var remaining = new List<IngredientType>(order.RequiredIngredients);
        foreach (var t in _stackedTypes)
        {
            if (remaining.Remove(t)) score++;
        }

        // Check sauces
        var sauceNames = new List<string>(order.RequiredSauces);
        foreach (var s in _sauces)
        {
            if (sauceNames.Remove(s.Name)) score++;
        }

        return score / total;
    }
}

[RequireComponent(typeof(BurgerAssembly), typeof(Rigidbody), typeof(Collider))]
public class BurgerAssemblyIngredient : Ingredient
{
    public BurgerAssembly Assembly { get; private set; }
    protected override void Awake()
    {
        base.Awake();   
        Assembly = GetComponent<BurgerAssembly>();
    }
}
