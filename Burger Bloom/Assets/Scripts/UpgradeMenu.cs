using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeMenu : MonoBehaviour
{
    [Header("References")]
    public UpgradeData upgradeData;
    public PlayerController playerController;
    public GrillStation grillStation;
    public GameObject menuPanel;

    [Header("Tab")]
    public GameObject stockPanel;
    public GameObject upgradePanel;
    public Button stockTabButton;
    public Button upgradeTabButton;

    [Header("Stock UI")]
    public TextMeshProUGUI bunStockText;
    public TextMeshProUGUI beefStockText;
    public TextMeshProUGUI chickenStockText;
    public TextMeshProUGUI bunPriceText;
    public TextMeshProUGUI beefPriceText;
    public TextMeshProUGUI chickenPriceText;
    public Button bunBuyButton;
    public Button beefBuyButton;
    public Button chickenBuyButton;

    [Header("Upgrade UI")]
    public TextMeshProUGUI bunUpgradeText;
    public TextMeshProUGUI meatUpgradeText;
    public TextMeshProUGUI cookText;
    public TextMeshProUGUI burnText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI bunUpgradePriceText;
    public TextMeshProUGUI meatUpgradePriceText;
    public TextMeshProUGUI cookPriceText;
    public TextMeshProUGUI burnPriceText;
    public TextMeshProUGUI speedPriceText;

    void Start()
    {
        ShowStockTab();
    }

    public void ShowStockTab()
    {
        stockPanel.SetActive(true);
        upgradePanel.SetActive(false);

        RefreshUI();
    }

    public void ShowUpgradeTab()
    {
        stockPanel.SetActive(false);
        upgradePanel.SetActive(true);

        RefreshUI();
    }

    public void OpenMenu()
    {
        menuPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        RefreshUI();
    }

    public void CloseMenu()
    {
        menuPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnCloseButton() => CloseMenu();

    void RefreshUI()
    {
        RefreshStock();
        RefreshUpgrades();
    }

    void RefreshStock()
    {
        var sm = StockManager.Instance;
        bool canOrder = DayManager.Instance.CanOrderStock();

        int bunCost = sm.bunCostPerUnit * sm.stockPerOrder;
        int beefCost = sm.beefCostPerUnit * sm.stockPerOrder;
        int chickenCost = sm.chickenCostPerUnit * sm.stockPerOrder;

        // Stock count
        if (bunStockText) bunStockText.text = $"x{sm.bunStock}";
        if (beefStockText) beefStockText.text = $"x{sm.beefStock}";
        if (chickenStockText) chickenStockText.text = $"x{sm.chickenStock}";

        // Price
        if (bunPriceText) bunPriceText.text = $"${bunCost}";
        if (beefPriceText) beefPriceText.text = $"${beefCost}";
        if (chickenPriceText) chickenPriceText.text = $"${chickenCost}";

        // Grey out ถ้าสั่งไม่ได้หรือเงินไม่พอ
        SetButtonState(bunBuyButton, canOrder && GameManager.Instance.Money >= bunCost);
        SetButtonState(beefBuyButton, canOrder && GameManager.Instance.Money >= beefCost);
        SetButtonState(chickenBuyButton, canOrder && GameManager.Instance.Money >= chickenCost);
    }

    void RefreshUpgrades()
    {
        int nextBunCost = GetUpgradeCost(upgradeData.bunLevel);
        int nextMeatCost = GetUpgradeCost(upgradeData.meatLevel);

        // Ingredient upgrades
        if (bunUpgradeText)
            bunUpgradeText.text = $"Bun  Lv.{upgradeData.bunLevel}";
        if (bunUpgradePriceText)
            bunUpgradePriceText.text = $"${nextBunCost}";

        if (meatUpgradeText)
            meatUpgradeText.text = $"Meat  Lv.{upgradeData.meatLevel}";
        if (meatUpgradePriceText)
            meatUpgradePriceText.text = $"${nextMeatCost}";

        // Shop upgrades
        SetShopUpgradeUI(cookText, cookPriceText,
            "Cook Speed", upgradeData.cookLevel,
            upgradeData.cookUpgradeCost, upgradeData.maxShopLevel,
            $"{upgradeData.GetCookTime():F1}s");

        SetShopUpgradeUI(burnText, burnPriceText,
            "Burn Resist", upgradeData.burnLevel,
            upgradeData.burnUpgradeCost, upgradeData.maxShopLevel,
            $"{upgradeData.GetBurnTime():F1}s");

        SetShopUpgradeUI(speedText, speedPriceText,
            "Run Speed", upgradeData.speedLevel,
            upgradeData.speedUpgradeCost, upgradeData.maxShopLevel,
            $"{upgradeData.GetWalkSpeed():F1}");
    }

    void SetShopUpgradeUI(TextMeshProUGUI label, TextMeshProUGUI price,
                          string name, int level, int cost, int max, string statVal)
    {
        if (label)
            label.text = $"{name} \nLv.{level} \n({statVal})";
        if (price)
            price.text = level >= max ? "MAX" : $"${cost}";
    }

    void SetButtonState(Button btn, bool interactable)
    {
        if (btn == null) return;
        btn.interactable = interactable;
    }

    public void BuyBunStock()
    {
        StockManager.Instance.OrderBun();
        UpgradeExpenseTracker.Instance.AddExpense(
            StockManager.Instance.bunCostPerUnit * StockManager.Instance.stockPerOrder);
        RefreshUI();
    }

    public void BuyBeefStock()
    {
        StockManager.Instance.OrderBeef();
        UpgradeExpenseTracker.Instance.AddExpense(
            StockManager.Instance.beefCostPerUnit * StockManager.Instance.stockPerOrder);
        RefreshUI();
    }

    public void BuyChickenStock()
    {
        StockManager.Instance.OrderChicken();
        UpgradeExpenseTracker.Instance.AddExpense(
            StockManager.Instance.chickenCostPerUnit * StockManager.Instance.stockPerOrder);
        RefreshUI();
    }

    int GetUpgradeCost(int level)
    {
        if (level - 1 >= upgradeData.upgradeCost.Length) return 999;
        return upgradeData.upgradeCost[level - 1];
    }

    public void UpgradeBun()
    {
        int cost = GetUpgradeCost(upgradeData.bunLevel);
        if (!GameManager.Instance.SpendMoney(cost)) return;
        UpgradeExpenseTracker.Instance.AddExpense(cost);
        upgradeData.bunLevel++;
        RefreshUI();
    }

    public void UpgradeMeat()
    {
        int cost = GetUpgradeCost(upgradeData.meatLevel);
        if (!GameManager.Instance.SpendMoney(cost)) return;
        UpgradeExpenseTracker.Instance.AddExpense(cost);
        upgradeData.meatLevel++;
        RefreshUI();
    }

    public void UpgradeCook()
    {
        if (upgradeData.cookLevel >= upgradeData.maxShopLevel) return;
        if (!GameManager.Instance.SpendMoney(upgradeData.cookUpgradeCost)) return;
        UpgradeExpenseTracker.Instance.AddExpense(upgradeData.cookUpgradeCost);
        upgradeData.cookLevel++;
        grillStation.RefreshCookTimes();
        RefreshUI();
    }

    public void UpgradeBurn()
    {
        if (upgradeData.burnLevel >= upgradeData.maxShopLevel) return;
        if (!GameManager.Instance.SpendMoney(upgradeData.burnUpgradeCost)) return;
        UpgradeExpenseTracker.Instance.AddExpense(upgradeData.burnUpgradeCost);
        upgradeData.burnLevel++;
        grillStation.RefreshCookTimes();
        RefreshUI();
    }

    public void UpgradeSpeed()
    {
        if (upgradeData.speedLevel >= upgradeData.maxShopLevel) return;
        if (!GameManager.Instance.SpendMoney(upgradeData.speedUpgradeCost)) return;
        UpgradeExpenseTracker.Instance.AddExpense(upgradeData.speedUpgradeCost);
        upgradeData.speedLevel++;
        playerController.walkSpeed = upgradeData.GetWalkSpeed();
        RefreshUI();
    }
}
