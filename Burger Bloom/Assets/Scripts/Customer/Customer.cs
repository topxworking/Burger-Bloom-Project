using UnityEngine;
using UnityEngine.AI;
using static PlayerInteract;

public enum CustomerState { Entering, WaitingInLine, Ordering, WaitingForFood, Leaving, Left }

public class Customer : MonoBehaviour, IInteractable
{
    [Header("Waypoints")]
    [SerializeField] private Transform _counterSpot;
    [SerializeField] private Transform _exitSpot;

    [Header("Settings")]
    [SerializeField] private float _satisfactionThreshold = 0.7f;

    private NavMeshAgent _agent;
    private StateMachine<CustomerState> _fsm;
    private OrderData _order;
    private float _patienceLeft;
    private float _patienceMax;

    public System.Action<Customer> OnReturnToPool;

    public OrderData Order => _order;
    public float PatienceRadio => _patienceLeft / Mathf.Max(_patienceMax, 1f);

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        InitFSM();
    }

    public void Init(OrderData order, Transform counter, Transform exit)
    {
        _order = order;
        _counterSpot = counter;
        _exitSpot = exit;
        _patienceLeft = order.Patience;
        _patienceMax = order.Patience;
        _fsm.TransitionTo(CustomerState.Entering);
    }

    private void InitFSM()
    {
        _fsm = new StateMachine<CustomerState>(CustomerState.Entering);

        _fsm.RegisterEnter(CustomerState.Entering, OnEntering);
        _fsm.RegisterEnter(CustomerState.WaitingInLine, OnWaitingInLine);
        _fsm.RegisterEnter(CustomerState.Ordering, OnOrdering);
        _fsm.RegisterEnter(CustomerState.WaitingForFood, OnWaitingForFood);
        _fsm.RegisterEnter(CustomerState.Leaving, OnLeaving);
        _fsm.RegisterEnter(CustomerState.Left, OnLeft);

        _fsm.RegisterTick(CustomerState.Entering, TickEntering);
        _fsm.RegisterTick(CustomerState.WaitingForFood, TickWaiting);
        _fsm.RegisterTick(CustomerState.Leaving, TickLeaving);
    }

    private void OnEntering() => _agent.SetDestination(_counterSpot.position);
    private void OnWaitingInLine() => _agent.ResetPath();
    private void OnOrdering()
    {
        EventBus.Publish(new OnOrderReceived { Order = _order });
    }
    private void OnWaitingForFood() { }
    private void OnLeaving() => _agent.SetDestination(_exitSpot.position);
    private void OnLeft()
    {
        OnReturnToPool?.Invoke(this);
    }

    private void TickEntering()
    {
        if (!_agent.pathPending && _agent.remainingDistance < 0.3f)
            _fsm.TransitionTo(CustomerState.WaitingInLine);
    }

    private void TickWaiting()
    {
        _patienceLeft -= Time.deltaTime;
        if (_patienceLeft <= 0f)
        {
            EventBus.Publish(new OnOrderFailed { Order = _order });
            _fsm.TransitionTo(CustomerState.Leaving);
        }
    }

    private void TickLeaving()
    {
        if (!_agent.pathPending && _agent.remainingDistance < 0.3f)
            _fsm.TransitionTo(CustomerState.Left);
    }

    private void Update()
    {
        _fsm.Tick();
    }

    // IInteractable
    public string GetPromptText() => "Serve Burger [E]";

    public bool CanInteract(PlayerInteract player)
    {
        return _fsm.Current == CustomerState.WaitingForFood &&
            player.Hands.IsHolding &&
            player.Hands.HeldIngredient is BurgerAssemblyIngredient;
    }

    public void Interact(PlayerInteract player)
    {
        if (player.Hands.HeldIngredient is not BurgerAssemblyIngredient bai) return;

        float score = bai.Assembly.ScoreAgainstOrder(_order);
        float tip = CalculateTip(score);

        EventBus.Publish(new OnOrderCompleted
        {
            Order = _order,
            Success = score >= _satisfactionThreshold,
            Tip = tip
        });

        player.Hands.Drop();
        _fsm.TransitionTo(CustomerState.Leaving);
    }

    private float CalculateTip(float score)
    {
        if (score < _satisfactionThreshold) return 0f;

        float patienceBonus = PatienceRadio * 10f;
        return Mathf.RoundToInt(score * 20f + patienceBonus);
    }

    public void ConfirmOrderAccepted()
    {
        if (_fsm.Current == CustomerState.WaitingInLine)
            _fsm.TransitionTo(CustomerState.Ordering);
    }

    public void SetWaitingForFood()
    {
        if (_fsm.Current == CustomerState.Ordering)
            _fsm.TransitionTo(CustomerState.WaitingForFood);
    }
}
