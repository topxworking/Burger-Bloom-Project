using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Windows;
using System.Collections;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance { get; private set; }

    [Header("Time Settings")]
    public float openHour = 8f;
    public float closeHour = 16f;
    public float forceCloseHour = 21f;

    [Header("UI")]
    public TextMeshProUGUI clockText;

    [Header("Events")]
    public UnityEvent OnShopOpened;
    public UnityEvent OnShopClosed;

    [Header("Day")]
    public TextMeshProUGUI dayText;

    private GameInputs input;

    public int CurrentDay { get; private set; } = 1;
    public bool IsOpen { get; private set; }
    public float CurrentHour { get; private set; }
    public bool DayStarted { get; private set; }
    public bool DayEnded { get; private set; }

    public void LoadState(int day, float hour, bool open, bool started)
    {
        CurrentDay = day;
        CurrentHour = hour;
        IsOpen = open;
        DayStarted = started;
        DayEnded = false;

        UpdateClockUI();
        UpdateDayUI();

        if (open)
        {
            OnShopOpened.Invoke();
            FindAnyObjectByType<CustomerSpawner>()?.OnShopOpen();
        }

        StartCoroutine(RefreshSignNextFrame(open));
    }

    IEnumerator RefreshSignNextFrame(bool open)
    {
        yield return null;
        var sign = FindAnyObjectByType<ShopSign>();
        if (sign != null) sign.RefreshMaterial(open);
    }

    private float timeScale = 1f / 60f;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        input = new GameInputs();
    }

    void OnEnable()
    {
        input.Enable();
        input.Player.EndDay.performed += _ => TryEndDay();
    }

    void OnDisable()
    {
        input.Disable();
        input.Player.EndDay.performed -= _ => TryEndDay();
    }

    void Start()
    {
        CurrentHour = openHour;
        IsOpen = false;
        DayEnded = false;
        UpdateClockUI();
        UpdateDayUI();
    }

    void Update()
    {
        if (!DayStarted || DayEnded) return;

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
        DayStarted = true;
        OnShopOpened.Invoke();
        FindAnyObjectByType<CustomerSpawner>()?.OnShopOpen();
    }

    public void CloseShop()
    {
        if (!IsOpen || DayEnded) return;

        IsOpen = false;
        OnShopClosed.Invoke();

        DismissAllCustomers();

        // ถ้าหลัง 04:00 PM > สรุปวันได้
        if (CurrentHour >= closeHour)
        {
            DayEnded = true;
            SummaryManager.Instance.ShowSummary();
        }
        else
        {
            NotificationManager.Instance.Show("Press Enter after 4:00 PM to end the day");
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
        OnShopClosed.Invoke();
        DismissAllCustomers();
        SummaryManager.Instance.ShowSummary();
    }

    void DismissAllCustomers()
    {
        var customers = FindObjectsByType<Customer>(FindObjectsInactive.Exclude);
        foreach (var c in customers)
            c.ForceLeave();
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

    void TryEndDay()
    {
        if (DayEnded) return;
        if (CurrentHour < closeHour)
        {
            NotificationManager.Instance.Show($"Cannot end day before 04:00 PM!");
            return;
        }
        ForceCloseWithSummary();
    }

    public void StartNextDay()
    {
        CurrentDay = CurrentDay + 1;
        CurrentHour = openHour;
        IsOpen = false;
        DayEnded = false;
        DayStarted = false;
        UpdateClockUI();
        UpdateDayUI();

        var sign = FindAnyObjectByType<ShopSign>();
        if (sign != null)
            sign.RefreshMaterial(false);
    }

    public void LoadDay(int day)
    {
        CurrentDay = day;
        CurrentHour = openHour;
        IsOpen = false;
        DayEnded = false;
        DayStarted = false;
        UpdateClockUI();
        UpdateDayUI();
    }
}
