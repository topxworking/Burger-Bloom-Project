using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailySummaryUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject _panel;
    [SerializeField] private Animator _panelAnimator;

    [Header("Stats Texts")]
    [SerializeField] private TextMeshProUGUI _dayText;
    [SerializeField] private TextMeshProUGUI _revenueText;
    [SerializeField] private TextMeshProUGUI _ordersText;
    [SerializeField] private TextMeshProUGUI _tipsText;
    [SerializeField] private TextMeshProUGUI _totalText;

    [Header("Star Rating")]
    [SerializeField] private Image[] _stars;
    [SerializeField] private Sprite _starFull;
    [SerializeField] private Sprite _starEmpty;

    [Header("Buttons")]
    [SerializeField] private Button _nextDayBtn;
    [SerializeField] private Button _mainMenuBtn;

    [Header("Count-up Speed")]
    [SerializeField] private float _countDuration = 1.5f;

    private float _todayRevenue;
    private float _todayTips;
    private int _ordersCompleted;
    private int _ordersFailed;

    private void Start()
    {
        _panel.SetActive(false);

        EventBus.Subscribe<OnDayEnded>(OnDayEnd);
        EventBus.Subscribe<OnOrderCompleted>(OnOrderDone);
        EventBus.Subscribe<OnOrderFailed>(OnOrderFail);

        _nextDayBtn?.onClick.AddListener(OnNextDay);
        _mainMenuBtn?.onClick.AddListener(OnMainMenu);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<OnDayEnded>(OnDayEnd);
        EventBus.Unsubscribe<OnOrderCompleted>(OnOrderDone);
        EventBus.Unsubscribe<OnOrderFailed>(OnOrderFail);
    }

    private void OnOrderDone(OnOrderCompleted e)
    {
        _ordersCompleted++;
        _todayTips += e.Tip;
    }

    private void OnOrderFail(OnOrderFailed e) => _ordersFailed++;

    private void OnDayEnd(OnDayEnded e)
    {
        _todayRevenue = e.Revenue;
        StartCoroutine(ShowSummary(e.Day));
    }

    private IEnumerator ShowSummary(int day)
    {
        yield return new WaitForSeconds(0.5f);

        _panel.SetActive(true);
        _panelAnimator?.SetTrigger("Show");

        if (_dayText) _dayText.text = $"Day {day} Complete!";

        yield return CountUp(_revenueText, "Revenue", _todayRevenue - _todayTips);
        yield return CountUp(_tipsText, "Tips", _todayTips);
        yield return CountUp(_totalText, "Total", _todayRevenue);

        if (_ordersText)
            _ordersText.text = $"Orders: {_ordersCompleted} Completed  {_ordersFailed} Failed";

        float rating = _ordersCompleted > 0
            ? (float)_ordersCompleted / (_ordersCompleted + _ordersFailed)
            : 0f;

        SetStars(rating);

        _ordersCompleted = 0;
        _ordersFailed = 0;
        _todayTips = 0;
        _todayRevenue = 0;
    }

    private IEnumerator CountUp(TextMeshProUGUI label, string prefix, float target)
    {
        if (label == null) yield break;
        float t = 0f;
        while (t < _countDuration)
        {
            t += Time.deltaTime;
            float value = Mathf.Lerp(0f, target, t / _countDuration);
            label.text = $"{prefix}: ${value:N0}";
            yield return null;
        }
        label.text = $"{prefix}: ${target:N0}";
    }

    private void SetStars(float ratio)
    {
        if (_stars == null) return;
        int earned = ratio >= 0.9f ? 3 : ratio >= 0.65f ? 2 : ratio >= 0.35f ? 1 : 0;
        for (int i = 0; i < _stars.Length; i++)
            if (_stars[i] != null)
                _stars[i].sprite = i < earned ? _starFull : _starEmpty;
    }

    private void OnNextDay()
    {
        _panel.SetActive(false);
        InventorySave();
        GameManager.Instance.NextDay();
    }

    private void OnMainMenu()
    {
        InventorySave();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    private void InventorySave()
    {
        InventorySystem.Instance?.Save();
    }
}
