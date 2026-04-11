using UnityEngine;

public class IngredientSpawnPoint : MonoBehaviour, IInteractable
{
    [Header("Ingredient")]
    [SerializeField] private IngredientData _ingredientData;
    [SerializeField] private IngredientDatabase _database;

    [Header("Spawn")]
    [SerializeField] private Transform _spawnTransform;

    [Header("World UI")]
    [SerializeField] private TMPro.TextMeshPro _stockLabel;
    [SerializeField] private Renderer _stockIndicator;

    private void Start()
    {
        RefreshLabel();
    }

    private void RefreshLabel()
    {
        if (_ingredientData == null) return;
        int stock = InventorySystem.Instance.GetStock(_ingredientData.Type);

        if (_stockLabel)
            _stockLabel.text = $"{_ingredientData.DisplayName}\n×{stock}";

        if (_stockIndicator)
            _stockIndicator.material.color = stock > 5
                ? Color.green : stock > 2 ? Color.yellow : Color.red;
    }

    public string GetPromptText()
    {
        if (_ingredientData == null) return "?";
        int stock = InventorySystem.Instance.GetStock(_ingredientData.Type);
        return stock > 0
            ? $"Take {_ingredientData.DisplayName} [E] (×{stock})"
            : $"Out of stock: {_ingredientData.DisplayName}";
    }

    public bool CanInteract(PlayerInteract player)
    {
        if (_ingredientData == null) return false;
        if (player.Hands.IsHolding) return false;
        return InventorySystem.Instance.HasStock(_ingredientData.Type);
    }

    public void Interact(PlayerInteract player)
    {
        if (_ingredientData == null) return;
        
        bool consumed = InventorySystem.Instance.Consume(_ingredientData.Type);
        if (!consumed) return;

        Vector3 pos = _spawnTransform != null ? _spawnTransform.position : transform.position + Vector3.up * 0.1f;
        var ingredient = _database.SpawnIngredient(_ingredientData.Type, pos);
        if (ingredient == null) return;

        player.Hands.PickUp(ingredient);
        RefreshLabel();

        EventBus.Subscribe<OnStockChanged>(OnStockChanged);
    }

    private void OnStockChanged(OnStockChanged e)
    {
        if (_ingredientData != null && e.IngredientId == _ingredientData.Type.ToString())
        {
            RefreshLabel();
            EventBus.Unsubscribe<OnStockChanged>(OnStockChanged);
        }
    }
}
