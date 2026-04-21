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
    public TextMeshProUGUI timerText;

    public int Money { get; private set; }
    private float timeLeft;
    private bool gameActive;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        timeLeft = gameDuration;
        gameActive = true;
        UpdateMoneyUI();
    }

    void Update()
    {
        if (!gameActive) return;
        timeLeft -= Time.deltaTime;
        timerText.text = $"{Mathf.CeilToInt(timeLeft):00}s";
        if (timeLeft <= 0) { timeLeft = 0; gameActive = false; }
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
}