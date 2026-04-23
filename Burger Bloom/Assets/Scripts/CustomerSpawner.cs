using System.Collections;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject[] customerPrefabs;

    [Header("References")]
    public Transform spawnPoint;
    public Transform exitPoint;
    public Transform shopFront;

    [Header("Spawn Time")]
    public float initialDelay = 10f;
    public float minSpawnTime = 15f;
    public float maxSpawnTime = 25f;

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
        if (customerPrefabs == null || customerPrefabs.Length == 0) return;

        int index = Random.Range(0, customerPrefabs.Length);
        var go = Instantiate(customerPrefabs[index],
                                      spawnPoint.position,
                                      spawnPoint.rotation);
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