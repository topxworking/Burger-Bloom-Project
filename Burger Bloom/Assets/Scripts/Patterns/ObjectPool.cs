using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour
{
    private readonly T _prefab;
    private readonly Transform _parent;
    private readonly Queue<T> _available = new();

    public ObjectPool(T prefab, Transform parent, int preWarm = 5)
    {
        _prefab = prefab;
        _parent = parent;
        for (int i = 0; i < preWarm; i++)
            Release(Create());
    }

    private T Create()
    {
        var obj = Object.Instantiate(_prefab, _parent);
        obj.gameObject.SetActive(false);
        return obj;
    }

    public T Get()
    {
        var obj = _available.Count > 0 ? _available.Dequeue() : Create();
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Release(T obj)
    {
        obj.gameObject.SetActive(false);
        _available.Enqueue(obj);
    }
}
