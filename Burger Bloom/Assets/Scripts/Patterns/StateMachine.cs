using System.Collections.Generic;
using System;

public class StateMachine<TState> where TState : Enum
{
    private TState _currentState;
    private readonly Dictionary<TState, Action> _onEnter = new();
    private readonly Dictionary<TState, Action> _onExit = new();
    private readonly Dictionary<TState, Action> _onTick = new();

    public TState Current => _currentState;
    public event Action<TState, TState> OnStateChanged;

    public StateMachine(TState initial)
    {
        _currentState = initial;
    }

    public void RegisterEnter(TState state, Action action) => _onEnter[state] = action;
    public void RegisterExit(TState state, Action action) => _onExit[state] = action;
    public void RegisterTick(TState state, Action action) => _onTick[state] = action;

    public void TransitionTo(TState next)
    {
        if (_currentState.Equals(next)) return;

        if (_onExit.TryGetValue(_currentState, out var exit)) exit?.Invoke();
        var prev = _currentState;
        OnStateChanged?.Invoke(prev, next);
        if (_onEnter.TryGetValue(_currentState, out var enter)) enter?.Invoke();
    }

    public void Tick()
    {
        if (_onTick.TryGetValue(_currentState, out var tick)) tick?.Invoke();
    }
}
