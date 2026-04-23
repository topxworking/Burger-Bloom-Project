using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    public float gameDuration = 180f;

    [Header("Data")]
    public UpgradeData upgradeData;

    [Header("UI")]
    public TextMeshProUGUI moneyText;

    public int Money { get; private set; }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        UpdateMoneyUI();
    }

    public void AddMoney(int amount)
    {
        Money += amount;
        UpdateMoneyUI();
    }

    public bool SpendMoney(int amount)
    {
        if (Money < amount) return false;
        Money -= amount;
        UpdateMoneyUI();
        return true;
    }

    void UpdateMoneyUI() => moneyText.text = $"${Money}";

    public void SetMoney(int amount)
    {
        Money = amount;
        moneyText.text = $"${Money}";
    }
}