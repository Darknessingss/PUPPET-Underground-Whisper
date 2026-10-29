using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance { get; private set; }

    [System.Serializable]
    public class PoolEntry
    {
        public GameObject prefab;
        public int size = 20;
    }

    [SerializeField] private List<PoolEntry> entries = new List<PoolEntry>();

    private readonly Dictionary<GameObject, Queue<GameObject>> poolsByPrefab = new Dictionary<GameObject, Queue<GameObject>>();
    private readonly Dictionary<GameObject, GameObject> prefabByInstance = new Dictionary<GameObject, GameObject>();

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

            var instanceQueue = new Queue<GameObject>(entry.size);
            for (int i = 0; i < entry.size; i++)
            {
                var instance = Instantiate(entry.prefab, transform);
                instance.SetActive(false);
                instanceQueue.Enqueue(instance);
                prefabByInstance[instance] = entry.prefab;
            }
            poolsByPrefab[entry.prefab] = instanceQueue;
        }
    }

    public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!poolsByPrefab.ContainsKey(prefab))
        {
            var newInstance = Instantiate(prefab, position, rotation, transform);
            prefabByInstance[newInstance] = prefab;
            return newInstance;
        }

        var instanceQueue = poolsByPrefab[prefab];
        GameObject poolInstance = null;
        while (instanceQueue.Count > 0)
        {
            var candidate = instanceQueue.Dequeue();
            if (candidate != null)
            {
                poolInstance = candidate;
                break;
            }
        }

        if (poolInstance == null)
        {
            poolInstance = Instantiate(prefab, transform);
            prefabByInstance[poolInstance] = prefab;
        }

        poolInstance.transform.SetPositionAndRotation(position, rotation);
        poolInstance.SetActive(true);
        return poolInstance;
    }

    public void Return(GameObject instance)
    {
        if (instance == null)
            return;

        if (!prefabByInstance.ContainsKey(instance))
        {
            Destroy(instance);
            return;
        }

        var prefab = prefabByInstance[instance];
        instance.SetActive(false);
        instance.transform.SetParent(transform);

        if (!poolsByPrefab.ContainsKey(prefab))
            poolsByPrefab[prefab] = new Queue<GameObject>();

        poolsByPrefab[prefab].Enqueue(instance);
    }
}