using UnityEngine;
using TMPro;

public class SummaryManager : MonoBehaviour
{
    public static SummaryManager Instance { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI dayText;
    public GameObject summaryPanel;
    public TextMeshProUGUI revenueText;
    public TextMeshProUGUI expenseText;
    public TextMeshProUGUI profitText;
    public TextMeshProUGUI upgradeExpenseText;
    public TextMeshProUGUI servedText;
    public int TotalRevenue { get; private set; }
    public int TotalServed { get; private set; }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        summaryPanel.SetActive(false);
    }

    public void AddRevenue(int amount)
    {
        TotalRevenue += amount;
        TotalServed++;
    }

    public void ShowSummary()
    {
        int stockExpense = StockManager.Instance.TotalExpense;
        int upgradeExpense = UpgradeExpenseTracker.Instance.TotalUpgradeExpense;
        int totalExpense = stockExpense + upgradeExpense;
        int profit = TotalRevenue - totalExpense;

        if (dayText)
            dayText.text = $"Day {DayManager.Instance.CurrentDay} Complete!";

        revenueText.text = $"Revenue: ${TotalRevenue}";
        expenseText.text = $"Stock Cost: ${stockExpense}";
        upgradeExpenseText.text = $"Upgrade Cost: ${upgradeExpense}";
        profitText.text = $"Net Profit: ${profit}";
        servedText.text = $"Customers Served: {TotalServed}";

        profitText.color = profit >= 0 ? Color.green : Color.red;

        summaryPanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnNextDay()
    {
        // Reset ตัวเลข
        TotalRevenue = 0;
        TotalServed = 0;

        summaryPanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        DayManager.Instance.StartNextDay();
    }
}
