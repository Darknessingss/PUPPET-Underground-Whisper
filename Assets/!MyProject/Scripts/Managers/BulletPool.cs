using System;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance { get; private set; }

    [Serializable]
    public class PoolEntry
    {
        public GameObject prefab;
        public int size = 20;
    }

    [SerializeField] private List<PoolEntry> entries = new();

    private readonly Dictionary<GameObject, Queue<GameObject>> poolsByPrefab = new();
    private readonly Dictionary<GameObject, GameObject> prefabByInstance = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        foreach (var entry in entries)
        {
            if (entry.prefab == null) 
                continue;

            var queue = new Queue<GameObject>(entry.size);
            for (int i = 0; i < entry.size; i++)
            {
                var instance = Instantiate(entry.prefab, transform);
                instance.SetActive(false);
                queue.Enqueue(instance);
                prefabByInstance[instance] = entry.prefab;
            }
            poolsByPrefab[entry.prefab] = queue;
        }
    }

    public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null) 
            return null;

        if (!poolsByPrefab.TryGetValue(prefab, out var queue))
        {
            queue = new Queue<GameObject>();
            poolsByPrefab[prefab] = queue;
        }

        GameObject instance = null;
        while (queue.Count > 0)
        {
            var candidate = queue.Dequeue();
            if (candidate != null)
            {
                instance = candidate;
                break;
            }
            prefabByInstance.Remove(candidate);
        }

        if (instance == null)
        {
            instance = Instantiate(prefab, transform);
            prefabByInstance[instance] = prefab;
        }

        instance.transform.SetPositionAndRotation(position, rotation);
        instance.SetActive(true);
        return instance;
    }

    public void Return(GameObject instance)
    {
        if (instance == null) 
            return;

        if (!prefabByInstance.TryGetValue(instance, out var prefab))
        {
            Destroy(instance);
            return;
        }

        instance.SetActive(false);

        if (!poolsByPrefab.TryGetValue(prefab, out var queue))
        {
            queue = new Queue<GameObject>();
            poolsByPrefab[prefab] = queue;
        }
        queue.Enqueue(instance);
    }
}