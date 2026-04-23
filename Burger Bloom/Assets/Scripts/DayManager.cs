using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance { get; private set; }

    [Header("Time Settings")]
    public float openHour = 8f;
    public float closeHour = 16f;
    public float forceCloseHour = 21f;

    [Header("UI")]
    public TextMeshProUGUI clockText;
    public TextMeshProUGUI shopStatusText;

    [Header("Events")]
    public UnityEvent OnShopOpened;
    public UnityEvent OnShopClosed;

    [Header("Day")]
    public TextMeshProUGUI dayText;
    public int CurrentDay { get; private set; } = 1;

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
        shopStatusText.text = "CLOSED";
        UpdateClockUI();
        UpdateDayUI();
    }

    void Update()
    {
        if (!IsOpen || DayEnded) return;

        CurrentHour += Time.deltaTime * timeScale;
        UpdateClockUI();

        if (CurrentHour >= forceCloseHour)
        {
            CurrentHour = forceCloseHour;
            ForceCloseWithSummary();
        }
    }

    void UpdateDayUI()
    {
        if (dayText) dayText.text = $"Day {CurrentDay}";
    }

    public void OpenShop()
    {
        if (IsOpen || DayEnded) return;
        IsOpen = true;
        shopStatusText.text = "OPEN";
        OnShopOpened.Invoke();
        FindAnyObjectByType<CustomerSpawner>()?.OnShopOpen();
    }

    public void CloseShop()
    {
        if (!IsOpen || DayEnded) return;

        IsOpen = false;
        shopStatusText.text = "CLOSED";
        OnShopClosed.Invoke();

        // ถ้าหลัง 04:00 PM > สรุปวันได้
        if (CurrentHour >= closeHour)
        {
            DayEnded = true;
            SummaryManager.Instance.ShowSummary();
        }
        else
        {
            // ก่อน 04:00 PM > ปิดแค่หยุด spawn ลูกค้า ยังไม่สรุป
            shopStatusText.text = "CLOSED EARLY";
        }
    }

    public void ReopenShop()
    {
        if (IsOpen || DayEnded) return;
        if (CurrentHour >= closeHour) return; // เลย 04:00 PM แล้วเปิดไม่ได้
        OpenShop();
    }

    void ForceCloseWithSummary()
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

    public void StartNextDay()
    {
        CurrentDay++;
        CurrentHour = openHour;
        IsOpen = false;
        DayEnded = false;
        shopStatusText.text = "CLOSED";
        UpdateClockUI();
        UpdateDayUI();
    }
}
