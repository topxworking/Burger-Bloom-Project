using System.Collections;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject customerPrefab;
    public Transform spawnPoint;
    public Transform exitPoint;

    [Header("Random Spawn Time")]
    public float minSpawnTime = 5f;
    public float maxSpawnTime = 15f;

    void Start() => StartCoroutine(SpawnLoop());

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            float wait = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(wait);

            if (QueueManager.Instance.CurrentQueueSize < QueueManager.Instance.maxQueueSize)
            {
                SpawnCustomer();
            }
        }
    }

    void SpawnCustomer()
    {
        var go = Instantiate(customerPrefab, spawnPoint.position, spawnPoint.rotation);
        var customer = go.GetComponent<Customer>();
        customer.exitPoint = exitPoint;
    }
}