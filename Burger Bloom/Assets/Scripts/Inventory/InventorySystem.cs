using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : Singleton<InventorySystem>
{
    [Header("Starting Stock")]
    [SerializeField] private int _defaultStartStock = 10;

    private readonly Dictionary<IngredientType, int> _stock = new();

    protected override void Awake()
    {
        base.Awake();
        LoadOrDefault();
    }

    private void LoadOrDefault()
    {
        var save = SaveSystem.Load();

        foreach (IngredientType type in System.Enum.GetValues(typeof(IngredientType)))
        {
            int saved = save.stock != null && (int)type < save.stock.Length
                ? save.stock[(int)type]
                : _defaultStartStock;
            _stock[type] = saved;
        }
    }

    public int GetStock(IngredientType type) =>
        _stock.TryGetValue(type, out int v) ? v : 0;

    public bool HasStock(IngredientType type, int qty = 1) =>
        GetStock(type) >= qty;

    public bool Consume(IngredientType type, int qty = 1)
    {
        if (!HasStock(type, qty)) return false;
        _stock[type] -= qty;
        EventBus.Publish(new OnStockChanged { IngredientId = type.ToString(), NewCount = _stock[type] });
        return true;
    }

    public void AddStock(IngredientType type, int qty)
    {
        if (!_stock.ContainsKey(type)) _stock[type] = 0;
        _stock[type] += qty;
        EventBus.Publish(new OnStockChanged { IngredientId = type.ToString(), NewCount = _stock[type] });
        EventBus.Publish(new OnDeliveryArrived { IngredientId = type.ToString(), Quantity = qty });
    }

    public void Restock(IngredientType type, int qty) => AddStock(type, qty);

    public void Save()
    {
        var save = SaveSystem.Load();
        int count = System.Enum.GetValues(typeof(IngredientType)).Length;
        save.stock = new int[count];
        foreach (var kv in _stock)
            save.stock[(int)kv.Key] = kv.Value;
        SaveSystem.Save(save);
    }
}
