using UnityEngine;
using TMPro;

public class StockManager : MonoBehaviour
{
    public static StockManager Instance { get; private set; }

    [Header("Stock Settings")]
    public int beefStock = 5;
    public int chickenStock = 5;
    public int beefCostPerUnit = 10;
    public int chickenCostPerUnit = 8;
    public int stockPerOrder = 5;
    public int bunStock = 10;
    public int bunCostPerUnit = 5;

    [Header("UI")]
    public TextMeshProUGUI beefStockText;
    public TextMeshProUGUI chickenStockText;
    public TextMeshProUGUI bunStockText;

    public int TotalExpense { get; private set; }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start() => RefreshUI();

    public bool UseBeef()
    {
        if (beefStock <= 0) { Debug.Log("Beef out of stock!"); return false; }
        beefStock--;
        RefreshUI();
        return true;
    }

    public bool UseChicken()
    {
        if (chickenStock <= 0) { Debug.Log("Chicken out of stock!"); return false; }
        chickenStock--;
        RefreshUI();
        return true;
    }

    public bool UseBun()
    {
        if (bunStock <= 0) { Debug.Log("Bun out of stock!"); return false; }
        bunStock--;
        RefreshUI();
        return true;
    }

    public bool OrderBun()
    {
        if (!DayManager.Instance.CanOrderStock())
        {
            Debug.Log("Cannot order after 16:00!");
            return false;
        }

        int cost = bunCostPerUnit * stockPerOrder;
        if (!GameManager.Instance.SpendMoney(cost)) { Debug.Log("Not enough money!"); return false; }

        bunStock += stockPerOrder;
        TotalExpense += cost;
        RefreshUI();
        return true;
    }

    public bool OrderBeef()
    {
        if (!DayManager.Instance.CanOrderStock())
        {
            Debug.Log("Cannot order after 16:00!");
            return false;
        }

        int cost = beefCostPerUnit * stockPerOrder;
        if (!GameManager.Instance.SpendMoney(cost)) { Debug.Log("Not enough money!"); return false; }

        beefStock += stockPerOrder;
        TotalExpense += cost;
        RefreshUI();
        return true;
    }

    public bool OrderChicken()
    {
        if (!DayManager.Instance.CanOrderStock())
        {
            Debug.Log("Cannot order after 16:00!");
            return false;
        }

        int cost = chickenCostPerUnit * stockPerOrder;
        if (!GameManager.Instance.SpendMoney(cost)) { Debug.Log("Not enough money!"); return false; }

        chickenStock += stockPerOrder;
        TotalExpense += cost;
        RefreshUI();
        return true;
    }

    void RefreshUI()
    {
        if (beefStockText) beefStockText.text = $"{beefStock}";
        if (chickenStockText) chickenStockText.text = $"{chickenStock}";
        if (bunStockText) bunStockText.text = $"{bunStock}";
    }
}
