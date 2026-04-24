using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Customer : MonoBehaviour
{
    [Header("Settings")]
    public float patience = 30f;

    [Header("References")]
    public Transform exitPoint;
    public CustomerOrderUI orderUI;
    public CustomerDialogue dialogue;

    [Header("Animation")]
    public Animator animator;
    public Transform shopDirection;

    public SauceType RequestedSauce { get; private set; }
    public MeatType RequestedMeat { get; private set; }

    private NavMeshAgent agent;
    private int slotIndex = -1;
    private float timer;
    private bool isServed;
    private bool isWaiting;

    static readonly int HashWalking = Animator.StringToHash("isWalking");

    public System.Action<Customer> OnServed;
    public System.Action<Customer> OnLeft;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        dialogue = GetComponent<CustomerDialogue>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        // สุ่มออเดอร์
        RequestedSauce = (SauceType)Random.Range(0, 3);
        RequestedMeat = (MeatType)Random.Range(0, 2);

        slotIndex = QueueManager.Instance.JoinQueue(this);
        if (slotIndex < 0) { Leave(false); return; }
        agent.SetDestination(QueueManager.Instance.GetSlotPosition(slotIndex));

        if (orderUI) orderUI.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isServed) return;

        UpdateWalkAnimation();

        if (!isWaiting && !agent.pathPending &&
        agent.remainingDistance <= agent.stoppingDistance)
        {
            isWaiting = true;
            SetWalking(false);
            FaceShop();

            if (QueueManager.Instance.IsFirst(this))
                ShowOrderUI();
        }

        if (isWaiting && QueueManager.Instance.IsFirst(this))
        {
            timer += Time.deltaTime;

            if (orderUI != null)
            {
                float normalized = 1f - (timer / patience);
                orderUI.SetPatience(normalized);
            }

            if (timer >= patience) Leave(false);
        }
    }

    void UpdateWalkAnimation()
    {
        if (agent == null || animator == null) return;

        bool moving = !agent.pathPending &&
                       agent.remainingDistance > agent.stoppingDistance &&
                       agent.velocity.magnitude > 0.1f;

        SetWalking(moving);
    }

    void SetWalking(bool walking)
    {
        if (animator == null) return;
        animator.SetBool(HashWalking, walking);
    }

    void FaceShop()
    {
        Vector3 target = shopDirection != null
            ? shopDirection.position
            : QueueManager.Instance.queueOrigin.position;

        Vector3 dir = target - transform.position;
        dir.y = 0f;

        if (dir == Vector3.zero) return;

        transform.rotation = Quaternion.LookRotation(dir);
    }

    public void UpdateQueueSlot(int newIndex)
    {
        slotIndex = newIndex;
        isWaiting = false;
        HideOrderUI();
        agent.SetDestination(QueueManager.Instance.GetSlotPosition(slotIndex));
        SetWalking(true);
    }

    public void ReceiveBurger(BurgerStack burger)
    {
        if (isServed) return;
        if (burger == null || !burger.HasMeat) return;
        if (!burger.HasSauce) return;
        if (burger.AppliedSauce != RequestedSauce) return;
        if (burger.MeatType != RequestedMeat) return;

        isServed = true;
        SoundManager.Instance.PlayServeSuccess();

        if (orderUI != null && dialogue != null)
        {
            orderUI.gameObject.SetActive(true);
            orderUI.SetDialogue(dialogue.GetServedLine());
        }

        int payment = GameManager.Instance.upgradeData.GetBurgerPrice();
        GameManager.Instance.AddMoney(payment);
        SummaryManager.Instance.AddRevenue(payment);
        Destroy(burger.gameObject);

        StartCoroutine(LeaveAfterDialogue());
    }

    IEnumerator LeaveAfterDialogue()
    {
        yield return new WaitForSeconds(1.5f);
        Leave(true);
    }

    public void Leave(bool satisfied)
    {
        SetWalking(true);
        QueueManager.Instance.LeaveQueue(this);
        isWaiting = false;
        HideOrderUI();
        agent.SetDestination(exitPoint.position);
        if (satisfied) OnServed?.Invoke(this);
        else OnLeft?.Invoke(this);
        Destroy(gameObject, 5f);
    }

    void ShowOrderUI()
    {
        if (orderUI == null || orderUI.gameObject.activeSelf) return;
        orderUI.gameObject.SetActive(true);

        orderUI.SetPatience(1f);

        if (dialogue != null)
            orderUI.SetDialogue(dialogue.GetOrderLine(RequestedMeat, RequestedSauce));
    }

    void HideOrderUI()
    {
        if (orderUI) orderUI.gameObject.SetActive(false);
    }

    private static readonly string[] dismissLines =
{
    "Are you serious right now?!",
    "I was next! This is unacceptable!",
    "Unbelievable... I'm leaving.",
    "You just lost a customer!",
    "Fine. I'll eat somewhere else."
};

    public void ForceLeave()
    {
        if (isServed) return;

        ShowOrderUI();
        if (orderUI != null && dialogue != null)
        {
            string line = dismissLines[Random.Range(0, dismissLines.Length)];
            orderUI.SetDialogue(line);
        }

        StartCoroutine(DelayedLeave());
    }

    IEnumerator DelayedLeave()
    {
        yield return new WaitForSeconds(1.5f);
        Leave(false);
    }
}
