using System;
using System.Collections.Generic;
using UnityEngine;

public struct OnShopOpened { }
public struct OnShopClosed { }
public struct OnDayStarted { public int Day; }
public struct OnDayEnded { public int Day; public float Revenue; }
public struct OnTimeChange { public float GameHour; }

public struct OnOrderReceived { public OrderData Order; }
public struct OnOrderCompleted { public OrderData Order; public bool Success; public float Tip; }
public struct OnOrderFailed { public OrderData Order; }

public struct OnMoneyChanged { public float NewAmount; public float Delta; }
public struct OnLevelUp { public int NewLevel; }
public struct OnStockChanged { public string IngredientId; public int NewCount; }
public struct OnDeliveryArrived { public string IngredientId; public int Quantity; }

public struct OnIngredientPickedUp { public string IngredientId; }
public struct OnIngredientPlaced { public string IngredientId; }
public struct OnPattyStateChanged { public MeatState State; }

public static class EventBus
{
    private static readonly Dictionary<Type, Delegate> _handlers = new();

    public static void Subscribe<T>(Action<T> handler)
    {
        var t = typeof(T);
        _handlers[t] = _handlers.ContainsKey(t)
            ? Delegate.Combine(_handlers[t], handler)
            : handler;
    }

    public static void Unsubscribe<T>(Action<T> handler)
    {
        var t = typeof(T);
        if (_handlers.ContainsKey(t))
            _handlers[t] = Delegate.Remove(_handlers[t], handler);
    }

    public static void Publish<T>(T evt)
    {
        var t = typeof(T);
        if (_handlers.TryGetValue(t, out var del))
            (del as Action<T>)?.Invoke(evt);
    }

    public static void Clear() => _handlers.Clear();
}

public enum MeatState { Raw, Cooking, Cooked, Overcooked, Burnt }

