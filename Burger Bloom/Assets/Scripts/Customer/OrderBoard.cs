using System.Collections.Generic;
using UnityEngine;

public class OrderBoard : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _ticketPrefab;
    [SerializeField] private Transform _ticketContainer;
    [SerializeField] private int _maxActiveOrders = 6;

    private readonly List<OrderData> _activeOrders = new();
    private readonly Dictionary<OrderData, GameObject> _tickets = new();

    private void Start()
    {
        EventBus.Subscribe<OnOrderReceived>(OnOrderIn);
        EventBus.Subscribe<OnOrderCompleted>(OnOrderOut);
        EventBus.Subscribe<OnOrderFailed>(OnOrderFail);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<OnOrderReceived>(OnOrderIn);
        EventBus.Unsubscribe<OnOrderCompleted>(OnOrderOut);
        EventBus.Unsubscribe<OnOrderFailed>(OnOrderFail);
    }

    private void OnOrderIn(OnOrderReceived e)
    {
        if (_activeOrders.Count >= _maxActiveOrders) return;
        _activeOrders.Add(e.Order);
        SpawnTicket(e.Order);
    }

    private void OnOrderOut(OnOrderCompleted e) => RemoveTicket(e.Order);
    private void OnOrderFail(OnOrderFailed e) => RemoveTicket(e.Order);

    private void SpawnTicket(OrderData order)
    {
        if (_ticketPrefab == null) return;
        var go = Instantiate(_ticketPrefab, _ticketContainer);
        var ticket = go.GetComponent<OrderTicketUI>();
        ticket?.SetOrder(order);
        _tickets[order] = go;
    }

    private void RemoveTicket(OrderData order)
    {
        _activeOrders.Remove(order);
        if (_tickets.TryGetValue(order, out var go))
        {
            Destroy(go);
            _tickets.Remove(order);
        }
    }

    public IReadOnlyList<OrderData> ActiveOrders => _activeOrders;
}
