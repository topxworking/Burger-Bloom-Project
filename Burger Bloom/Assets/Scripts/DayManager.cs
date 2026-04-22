using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance { get; private set; }

    [Header("Time Settings")]
    public float openHour = 8f;
    public float closeHour = 16f;

    [Header("UI")]
    public TextMeshProUGUI clockText;
    public TextMeshProUGUI shopStatusText;

    [Header("Events")]
    public UnityEvent OnShopOpened;
    public UnityEvent OnShopClosed;

    public bool IsOpen { get; private set; }
    public float CurrentHour { get; private set; }
    public bool DayEnded { get; private set; }

    private float timeScale = 1f / 60f;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        CurrentHour = openHour;
        IsOpen = false;
        DayEnded = false;
        UpdateClockUI();
        shopStatusText.text = "CLOSED";
    }

    void Update()
    {
        if (!IsOpen || DayEnded) return;

        CurrentHour += Time.deltaTime * timeScale;
        UpdateClockUI();

        if (CurrentHour >= closeHour)
        {
            CurrentHour = closeHour;
            ForceCloseShop();
        }
    }

    public void OpenShop()
    {
        if (IsOpen || DayEnded) return;
        IsOpen = true;
        shopStatusText.text = "OPEN";
        OnShopOpened.Invoke();

        FindAnyObjectByType<CustomerSpawner>()?.OnShopOpen();

        Debug.Log("Shop Opened!");
    }

    public void CloseShop()
    {
        if (!IsOpen) return;
        IsOpen = false;
        DayEnded = true;
        shopStatusText.text = "CLOSED";
        OnShopClosed.Invoke();
        SummaryManager.Instance.ShowSummary();
    }

    void ForceCloseShop()
    {
        IsOpen = false;
        DayEnded = true;
        shopStatusText.text = "CLOSED";
        OnShopClosed.Invoke();
        SummaryManager.Instance.ShowSummary();
    }

    public bool CanOrderStock() => !DayEnded && CurrentHour < closeHour;

    void UpdateClockUI()
    {
        int hour = Mathf.FloorToInt(CurrentHour);
        int minute = Mathf.FloorToInt((CurrentHour - hour) * 60f);

        string period = hour >= 12 ? "PM" : "AM";
        int hour12 = hour % 12;
        if (hour12 == 0) hour12 = 12;

        clockText.text = $"{hour12:00}:{minute:00} {period}";
    }
}
