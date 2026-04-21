using UnityEngine;
using UnityEngine.AI;

public class Customer : MonoBehaviour
{
    [Header("Settings")]
    public float patience = 30f;

    [Header("References")]
    public Transform exitPoint;

    public SauceType RequestedSauce { get; private set; }

    [Header("UI")]
    public CustomerOrderUI orderUI;

    private NavMeshAgent agent;
    private int slotIndex = -1;
    private float timer;
    private bool isServed;
    private bool isWaiting;

    public System.Action<Customer> OnServed;
    public System.Action<Customer> OnLeft;

    void Awake() => agent = GetComponent<NavMeshAgent>();

    void Start()
    {
        RequestedSauce = (SauceType)Random.Range(0, 3);

        if (orderUI) orderUI.gameObject.SetActive(false);

        slotIndex = QueueManager.Instance.JoinQueue(this);
        if (slotIndex < 0) { Leave(false); return; }
        agent.SetDestination(QueueManager.Instance.GetSlotPosition(slotIndex));

        if (slotIndex == 0) ShowOrderUI();
    }

    void Update()
    {
        if (isServed) return;

        if (!isWaiting && !agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance)
            isWaiting = true;

        if (isWaiting && QueueManager.Instance.IsFirst(this))
        {
            ShowOrderUI();

            timer += Time.deltaTime;
            if (timer >= patience) Leave(false);
        }
    }

    public void UpdateQueueSlot(int newIndex)
    {
        slotIndex = newIndex;
        isWaiting = false;
        agent.SetDestination(QueueManager.Instance.GetSlotPosition(slotIndex));

        if (newIndex == 0) ShowOrderUI();
        else HideOrderUI();
    }

    public void ReceiveBurger(BurgerStack burger)
    {
        if (isServed) return;
        if (burger == null) return;

        if (!burger.HasMeat)
        {
            Debug.Log("ไม่มีเนื้อ!");
            return;
        }

        if (!burger.HasSauce)
        {
            Debug.Log("ยังไม่มีซอส!");
            return;
        }

        if (burger.AppliedSauce != RequestedSauce)
        {
            Debug.Log($"ซอสผิด! ต้องการ {RequestedSauce} แต่ได้ {burger.AppliedSauce}");
            return;
        }

        isServed = true;
        GameManager.Instance.AddScore(150);
        Debug.Log($"เสิร์ฟสำเร็จ! +150");
        Destroy(burger.gameObject);
        Leave(true);
    }

    public void Leave(bool satisfied)
    {
        QueueManager.Instance.LeaveQueue(this);
        isWaiting = false;
        agent.SetDestination(exitPoint.position);
        if (satisfied) OnServed?.Invoke(this);
        else OnLeft?.Invoke(this);
        Destroy(gameObject, 5f);
    }

    void ShowOrderUI()
    {
        if (orderUI == null) return;
        orderUI.gameObject.SetActive(true);
        orderUI.SetOrder(RequestedSauce);
    }

    void HideOrderUI()
    {
        if (orderUI == null) return;
        orderUI.gameObject.SetActive(false);
    }
}
