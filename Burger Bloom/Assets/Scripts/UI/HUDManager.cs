using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [Header("Time")]
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private Image _timeFillBar;

    [Header("Money")]
    [SerializeField] private TextMeshProUGUI _moneyText;
    [SerializeField] private Animator _moneyAnimator;

    [Header("Day / Status")]
    [SerializeField] private TextMeshProUGUI _dayText;
    [SerializeField] private TextMeshProUGUI _statusText;
    [SerializeField] private Image _statusBg;
    [SerializeField] private Color _openColor = new Color(0.2f, 0.8f, 0.3f);
    [SerializeField] private Color _closeColor = new Color(0.8f, 0.2f, 0.2f);

    [Header("Shop Sign Button")]
    [SerializeField] private GameObject _signPrompt;

    [Header("Level")]
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private Image _xpBar;

    private void Start()
    {
        EventBus.Subscribe<OnTimeChange>(OnTime);
        EventBus.Subscribe<OnMoneyChanged>(OnMoney);
        EventBus.Subscribe<OnShopOpened>(OnOpen);
        EventBus.Subscribe<OnShopClosed>(OnClose);
        EventBus.Subscribe<OnDayStarted>(OnDayStart);
        EventBus.Subscribe<OnLevelUp>(OnLevel);

        RefreshAll();
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<OnTimeChange>(OnTime);
        EventBus.Unsubscribe<OnMoneyChanged>(OnMoney);
        EventBus.Unsubscribe<OnShopOpened>(OnOpen);
        EventBus.Unsubscribe<OnShopClosed>(OnClose);
        EventBus.Unsubscribe<OnDayStarted>(OnDayStart);
        EventBus.Unsubscribe<OnLevelUp>(OnLevel);
    }

    private void OnTime(OnTimeChange e)
    {
        if (_timeText) _timeText.text = DayManager.Instance.GetTimeString();
        if (_timeFillBar) _timeFillBar.fillAmount = DayManager.Instance.DayProgress;
    }

    private void OnMoney(OnMoneyChanged e)
    {
        if (_moneyText) _moneyText.text = $"${e.NewAmount:N0}";
        if (_moneyAnimator) _moneyAnimator.SetTrigger("Pop");
    }

    private void OnOpen(OnShopOpened _)
    {
        if (_statusText) _statusText.text = "OPEN";
        if (_statusBg) _statusBg.color = _openColor;
    }

    private void OnClose(OnShopClosed _)
    {
        if (_statusText) _statusText.text = "CLOSED";
        if (_statusBg) _statusBg.color = _closeColor;
    }

    private void OnDayStart(OnDayStarted e)
    {
        if (_dayText) _dayText.text = $"Day {e.Day}";
    }

    private void OnLevel(OnLevelUp e)
    {
        if (_levelText) _levelText.text = $"Lv.{e.NewLevel}";
    }

    private void RefreshAll()
    {
        if (_moneyText) _moneyText.text = $"${GameManager.Instance?.Money:N0}";
        if (_dayText) _dayText.text = $"Day {GameManager.Instance?.Day}";
        if (_levelText) _levelText.text = $"Lv.{GameManager.Instance?.Level}";
    }
}
