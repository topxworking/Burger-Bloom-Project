using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    public float gameDuration = 180f;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    private int score;
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
    }

    void Update()
    {
        if (!gameActive) return;

        timeLeft -= Time.deltaTime;
        timerText.text = $"{Mathf.CeilToInt(timeLeft):00}s";

        if (timeLeft <= 0)
        {
            timeLeft = 0;
            gameActive = false;
            Debug.Log($"Game Over! Score: {score}");
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text = $"Score: {score}";
    }
}