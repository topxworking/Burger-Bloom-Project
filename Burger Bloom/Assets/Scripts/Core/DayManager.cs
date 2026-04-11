using UnityEngine;

public class DayManager : Singleton<DayManager>
{
    [Header("Time Settings")]
    [SerializeField] private float _dayStartHour = 9f;
    [SerializeField] private float _dayEndHour = 17f;
    [SerializeField] private float _realDaySeconds = 480f;

    private float _gameHour;
    private float _elapsed;
    private bool _running;

    public float GameHour => _gameHour;
    public bool IsOpen => _running && GameManager.Instance.State == GameState.ShopOpen;
    public float DayProgress => Mathf.InverseLerp(_dayStartHour, _dayEndHour, _gameHour);

    public float CustomerSpawnMultiplier
    {
        get
        {
            if (!IsOpen) return 0f;
            if (_gameHour < 16f) return 1f;
            return Mathf.Lerp(1f, 0f, (_gameHour - 16f));
        }
    }

    private void Start()
    {
        EventBus.Subscribe<OnShopOpened>(OnOpen);
        EventBus.Subscribe<OnShopClosed>(OnClose);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<OnShopOpened>(OnOpen);
        EventBus.Unsubscribe<OnShopClosed>(OnClose);
    }

    private void OnOpen(OnShopOpened _)
    {
        _elapsed = 0f;
        _gameHour = _dayStartHour;
        _running = true;
    }

    private void OnClose(OnShopClosed _) => _running = false;

    private void Update()
    {
        if (!_running) return;

        _elapsed += Time.deltaTime;
        float t = _elapsed / _realDaySeconds;
        _gameHour = Mathf.Lerp(_dayStartHour, _dayEndHour, t);

        EventBus.Publish(new OnTimeChange { GameHour = _gameHour });

        if (_gameHour >= _dayEndHour)
        {
            _running = false;
            GameManager.Instance.CloseShop();
        }
    }

    public string GetTimeString()
    {
        int hours = Mathf.FloorToInt(_gameHour);
        int minutes = Mathf.FloorToInt((_gameHour - hours) * 60f);
        return $"{hours:00}:{minutes:00}";
    }
}
