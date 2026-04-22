using UnityEngine;

public class UpgradeExpenseTracker : MonoBehaviour
{
    public static UpgradeExpenseTracker Instance { get; private set; }
    public int TotalUpgradeExpense { get; private set; }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void AddExpense(int amount) => TotalUpgradeExpense += amount;
}
