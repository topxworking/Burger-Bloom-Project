using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Customer : MonoBehaviour
{
    [Header("Settings")]
    public float patience = 30f;

    [Header("References")]
    public Transform exitPoint;

    public System.Action<Customer> OnServed;
    public System.Action<Customer> OnLeft;

    private NavMeshAgent agent;
    private int slotIndex = -1;
    private float timer;
    private bool isServed;
    private bool isWaiting;

    void Awake() => agent = GetComponent<NavMeshAgent>();

    void Start()
    {
        slotIndex = QueueManager.Instance.JoinQueue(this);

        if (slotIndex < 0)
        {
            Leave(satisfied: false);
            return;
        }

        agent.SetDestination(QueueManager.Instance.GetSlotPosition(slotIndex));
    }

    void Update()
    {
        if (isServed) return;

        if (!isWaiting && !agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance)
        {
            isWaiting = true;
        }

        if (isWaiting && QueueManager.Instance.IsFirst(this))
        {
            timer += Time.deltaTime;
            if (timer >= patience)
                Leave(satisfied: false);
        }
    }

    public void UpdateQueueSlot(int newIndex)
    {
        slotIndex = newIndex;
        isWaiting = false;
        agent.SetDestination(QueueManager.Instance.GetSlotPosition(slotIndex));
    }

    public void ReceiveBurger(BurgerStack burger)
    {
        if (isServed) return;
        isServed = true;
        Destroy(burger.gameObject);
        GameManager.Instance.AddScore(100);
        Leave(satisfied: true);
    }

    void Leave(bool satisfied)
    {
        QueueManager.Instance.LeaveQueue(this);
        isWaiting = false;
        agent.SetDestination(exitPoint.position);
        if (satisfied) OnServed?.Invoke(this);
        else OnLeft?.Invoke(this);
        Destroy(gameObject, 5f);
    }
}