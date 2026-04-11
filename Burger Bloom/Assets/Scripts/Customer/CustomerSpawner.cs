using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [Header("Prefabs & Pool")]
    [SerializeField] private Customer _customerPrefab;
    [SerializeField] private int _poolSize = 10;

    [Header("Spawn Settings")]
    [SerializeField] private float _baseInterval = 20f;
    [SerializeField] private int _maxConcurrent = 5;

    [Header("Waypoints")]
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _counterSpot;
    [SerializeField] private Transform _exitSpot;

    private ObjectPool<Customer> _pool;
    private float _timer;
    private int _activeCount;
    private bool _open;

    private void Start()
    {
        _pool = new ObjectPool<Customer>(_customerPrefab, transform, _poolSize);
        EventBus.Subscribe<OnShopOpened>(OnOpen);
        EventBus.Subscribe<OnShopClosed>(OnClose);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<OnShopOpened>(OnOpen);
        EventBus.Unsubscribe<OnShopClosed>(OnClose);
    }

    private void OnOpen(OnShopOpened _) { _open = true; _timer = 0f; }
    private void OnClose(OnShopClosed _) => _open = false;

    private void Update()
    {
        if (!_open) return;
        if (_activeCount >= _maxConcurrent) return;

        float multiplier = DayManager.Instance.CustomerSpawnMultiplier;
        if (multiplier <= 0f) return;

        float interval = _baseInterval / multiplier;
        _timer += Time.deltaTime;

        if (_timer >= interval)
        {
            _timer = 0f;
            SpawnCustomer();
        }
    }

    private void SpawnCustomer()
    {
        var c = _pool.Get();
        c.transform.position = _spawnPoint.position;
        c.transform.rotation = _spawnPoint.rotation;

        int level = GameManager.Instance.Level;
        var order = OrderData.GenerateRandom(level);

        c.OnReturnToPool = ReturnToPool;
        c.Init(order, _counterSpot, _exitSpot);
        _activeCount++;
    }

    private void ReturnToPool(Customer c)
    {
        _pool.Release(c);
        _activeCount = Mathf.Max(0, _activeCount - 1);
    }
}
