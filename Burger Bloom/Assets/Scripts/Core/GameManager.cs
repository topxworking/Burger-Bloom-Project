using UnityEngine;

public enum GameState { MainMenu, DayPrepare, ShopOpen, ShopClosed, DaySummary }

public class GameManager : Singleton<GameManager>
{
    [Header("Progress")]
    [SerializeField] private float _money = 500f;
    [SerializeField] private int _level = 1;
    [SerializeField] private int _xp = 0;
    [SerializeField] private int _day = 1;

    [Header("Level XP")]
    [SerializeField] private int[] _xpThresholds = { 0, 100, 250, 500, 900, 1500 };

    private StateMachine<GameState> _fsm;
    public GameState State => _fsm.Current;

    private float _todayRevenue;
    private int _ordersCompleted;
    private int _ordersFailed;

    public float Money => _money;
    public int Level => _level;
    public int Day => _day;
    public float Revenue => _todayRevenue;

    protected override void Awake()
    {
        base.Awake();
        InitFSM();
        SubscribeEvents();
    }

    protected void OnDestroy()
    {
        UnsubscribeEvents();
    }

    private void InitFSM()
    {
        _fsm = new StateMachine<GameState>(GameState.DayPrepare);

        _fsm.RegisterEnter(GameState.ShopOpen, OnShopOpen);
        _fsm.RegisterEnter(GameState.ShopClosed, OnShopClosed);
        _fsm.RegisterEnter(GameState.DaySummary, OnDaySummary);
    }

    private void OnShopOpen()
    {
        _todayRevenue = 0;
        _ordersCompleted = 0;
        _ordersFailed = 0;
        EventBus.Publish(new OnShopOpened());
    }

    private void OnShopClosed()
    {
        EventBus.Publish(new OnShopClosed());
    }

    private void OnDaySummary()
    {
        EventBus.Publish(new OnDayEnded { Day = _day, Revenue = _todayRevenue });
    }

    public void OpenShop()
    {
        if (_fsm.Current == GameState.DayPrepare)
            _fsm.TransitionTo(GameState.ShopOpen);
    }

    public void CloseShop()
    {
        if (_fsm.Current == GameState.ShopOpen)
        {
            _fsm.TransitionTo(GameState.ShopClosed);
            _fsm.TransitionTo(GameState.DaySummary);
        }
    }

    public void NextDay()
    {
        _day++;
        _fsm.TransitionTo(GameState.DayPrepare);
    }

    public bool SpendMoney(float amount)
    {
        if (_money < amount) return false;
        _money -= amount;
        EventBus.Publish(new OnMoneyChanged { NewAmount = _money, Delta = -amount });
        return true;
    }

    public void EarnMoney(float amount)
    {
        _money += amount;
        _todayRevenue += amount;
        EventBus.Publish(new OnMoneyChanged { NewAmount = _money, Delta = amount });
    }

    public void AddXP(int amount)
    {
        _xp += amount;
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        if (_level >= _xpThresholds.Length) return;
        if (_xp >= _xpThresholds[_level])
        {
            _level++;
            EventBus.Publish(new OnLevelUp { NewLevel = _level });
        }
    }

    private void SubscribeEvents()
    {
        EventBus.Subscribe<OnOrderCompleted>(OnOrderDone);
        EventBus.Subscribe<OnOrderFailed>(OnOrderMissed);
    }

    private void UnsubscribeEvents()
    {
        EventBus.Unsubscribe<OnOrderCompleted>(OnOrderDone);
        EventBus.Unsubscribe<OnOrderFailed>(OnOrderMissed);
    }

    private void OnOrderDone(OnOrderCompleted e)
    {
        _ordersCompleted++;
        float payment = e.Order.BasePrice + e.Tip;
        EarnMoney(payment);
        AddXP(10);
    }

    private void OnOrderMissed(OnOrderFailed e)
    {
        _ordersFailed++;
    }
}
