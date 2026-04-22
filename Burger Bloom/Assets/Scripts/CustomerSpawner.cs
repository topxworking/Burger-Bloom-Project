using System.Collections;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject customerPrefab;
    public Transform spawnPoint;
    public Transform exitPoint;
    public Transform shopFront;

    [Header("Spawn Time")]
    public float initialDelay = 20f;
    public float minSpawnTime = 5f;
    public float maxSpawnTime = 15f;

    void Start() => StartCoroutine(SpawnLoop());

    IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            if (DayManager.Instance.IsOpen && !DayManager.Instance.DayEnded)
            {
                if (QueueManager.Instance.CurrentQueueSize < QueueManager.Instance.maxQueueSize)
                    SpawnCustomer();
            }

            float wait = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(wait);
        }
    }

    void SpawnCustomer()
    {
        if (QueueManager.Instance.CurrentQueueSize >= QueueManager.Instance.maxQueueSize) return;
        var go = Instantiate(customerPrefab, spawnPoint.position, spawnPoint.rotation);
        var customer = go.GetComponent<Customer>();
        customer.exitPoint = exitPoint;
        customer.shopDirection = shopFront;
    }

    public void OnShopOpen()
    {
        StopAllCoroutines();
        StartCoroutine(SpawnLoop());
    }
}