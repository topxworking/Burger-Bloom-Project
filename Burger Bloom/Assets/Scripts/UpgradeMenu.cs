using UnityEngine;
using TMPro;

public class UpgradeMenu : MonoBehaviour
{
    [Header("References")]
    public UpgradeData upgradeData;
    public PlayerController playerController;
    public GrillStation grillStation;
    public GameObject menuPanel;

    [Header("UI Text")]
    public TextMeshProUGUI bunText;
    public TextMeshProUGUI meatText;
    public TextMeshProUGUI cookText;
    public TextMeshProUGUI burnText;
    public TextMeshProUGUI speedText;

    public void OpenMenu()
    {
        Time.timeScale = 0f;
        menuPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        RefreshUI();
    }

    public void CloseMenu()
    {
        Time.timeScale = 1f;
        menuPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnCloseButton() => CloseMenu();

    void RefreshUI()
    {
        int nextBunCost = GetUpgradeCost(upgradeData.bunLevel);
        int nextMeatCost = GetUpgradeCost(upgradeData.meatLevel);

        bunText.text = $"Bun Lv.{upgradeData.bunLevel}  >  ${nextBunCost}\n" +
                         $"Burger Price: ${upgradeData.GetBurgerPrice()}";

        meatText.text = $"Meat Lv.{upgradeData.meatLevel}  >  ${nextMeatCost}\n" +
                         $"Burger Price: ${upgradeData.GetBurgerPrice()}";

        cookText.text = upgradeData.cookLevel < upgradeData.maxShopLevel
            ? $"Cook Speed Lv.{upgradeData.cookLevel}  >  ${upgradeData.cookUpgradeCost}\n" +
              $"Cook Time: {upgradeData.GetCookTime():F1}s"
            : "Cook Speed [MAX]";

        burnText.text = upgradeData.burnLevel < upgradeData.maxShopLevel
            ? $"Burn Resist Lv.{upgradeData.burnLevel}  >  ${upgradeData.burnUpgradeCost}\n" +
              $"Burn Time: {upgradeData.GetBurnTime():F1}s"
            : "Burn Resist [MAX]";

        speedText.text = upgradeData.speedLevel < upgradeData.maxShopLevel
            ? $"Run Speed Lv.{upgradeData.speedLevel}  >  ${upgradeData.speedUpgradeCost}\n" +
              $"Speed: {upgradeData.GetWalkSpeed():F1}"
            : "Run Speed [MAX]";

        bool canOrder = DayManager.Instance.CanOrderStock();
    }

    int GetUpgradeCost(int level)
    {
        if (level - 1 >= upgradeData.upgradeCost.Length) return 999;
        return upgradeData.upgradeCost[level - 1];
    }

    public void UpgradeBun()
    {
        int cost = GetUpgradeCost(upgradeData.bunLevel);
        if (!GameManager.Instance.SpendMoney(cost)) { Debug.Log("Not enough money!"); return; }
        upgradeData.bunLevel++;
        UpgradeExpenseTracker.Instance.AddExpense(cost);
        RefreshUI();
    }

    public void UpgradeMeat()
    {
        int cost = GetUpgradeCost(upgradeData.meatLevel);
        if (!GameManager.Instance.SpendMoney(cost)) { Debug.Log("Not enough money!"); return; }
        upgradeData.meatLevel++;
        UpgradeExpenseTracker.Instance.AddExpense(cost);
        RefreshUI();
    }

    public void UpgradeCook()
    {
        if (upgradeData.cookLevel >= upgradeData.maxShopLevel) { Debug.Log("Already MAX!"); return; }
        if (!GameManager.Instance.SpendMoney(upgradeData.cookUpgradeCost)) { Debug.Log("Not enough money!"); return; }
        upgradeData.cookLevel++;
        grillStation.RefreshCookTimes();
        RefreshUI();
    }

    public void UpgradeBurn()
    {
        if (upgradeData.burnLevel >= upgradeData.maxShopLevel) { Debug.Log("Already MAX!"); return; }
        if (!GameManager.Instance.SpendMoney(upgradeData.burnUpgradeCost)) { Debug.Log("Not enough money!"); return; }
        upgradeData.burnLevel++;
        grillStation.RefreshCookTimes();
        RefreshUI();
    }

    public void UpgradeSpeed()
    {
        if (upgradeData.speedLevel >= upgradeData.maxShopLevel) { Debug.Log("Already MAX!"); return; }
        if (!GameManager.Instance.SpendMoney(upgradeData.speedUpgradeCost)) { Debug.Log("Not enough money!"); return; }
        upgradeData.speedLevel++;
        playerController.walkSpeed = upgradeData.GetWalkSpeed();
        RefreshUI();
    }

    public void BuyBeefStock()
    {
        if (!DayManager.Instance.CanOrderStock())
        {
            Debug.Log("Cannot order stock after 16:00!");
            return;
        }
        StockManager.Instance.OrderBeef();
        RefreshUI();
    }

    public void BuyChickenStock()
    {
        if (!DayManager.Instance.CanOrderStock())
        {
            Debug.Log("Cannot order stock after 16:00!");
            return;
        }
        StockManager.Instance.OrderChicken();
        RefreshUI();
    }
    public void BuyBunStock()
    {
        if (!DayManager.Instance.CanOrderStock())
        {
            Debug.Log("Cannot order stock after 16:00!");
            return;
        }
        StockManager.Instance.OrderBun();
        RefreshUI();
    }

}
