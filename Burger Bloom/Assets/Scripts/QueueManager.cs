using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    public static QueueManager Instance { get; private set; }

    [Header("Queue Settings")]
    public Transform queueOrigin;
    public Vector3 queueDirection = Vector3.back;
    public float slotSpacing = 1.5f;
    public int maxQueueSize = 5;

    private List<Customer> queue = new();

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public int JoinQueue(Customer customer)
    {
        if (queue.Count >= maxQueueSize) return -1;
        queue.Add(customer);
        return queue.Count - 1;
    }

    public void LeaveQueue(Customer customer)
    {
        int index = queue.IndexOf(customer);
        if (index < 0) return;
        queue.RemoveAt(index);
        RefreshPositions();
    }

    public Vector3 GetSlotPosition(int index)
    {
        return queueOrigin.position + queueDirection.normalized * slotSpacing * index;
    }

    void RefreshPositions()
    {
        for (int i = 0; i < queue.Count; i++)
            queue[i].UpdateQueueSlot(i);
    }

    public bool IsFirst(Customer customer) => queue.Count > 0 && queue[0] == customer;
}
