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

    private readonly Dictionary<GameObject, Queue<GameObject>> pools = new Dictionary<GameObject, Queue<GameObject>>();
    private readonly Dictionary<GameObject, GameObject> instanceToPrefab = new Dictionary<GameObject, GameObject>();

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
            if (entry.prefab == null) continue;

            var q = new Queue<GameObject>(entry.size);
            for (int i = 0; i < entry.size; i++)
            {
                var go = Instantiate(entry.prefab, transform);
                go.SetActive(false);
                q.Enqueue(go);
                instanceToPrefab[go] = entry.prefab;
            }
            pools[entry.prefab] = q;
        }
    }

    public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!pools.ContainsKey(prefab))
        {
            var go = Instantiate(prefab, position, rotation, transform);
            instanceToPrefab[go] = prefab;
            return go;
        }

        var q = pools[prefab];
        GameObject inst = null;
        while (q.Count > 0)
        {
            var candidate = q.Dequeue();
            if (candidate != null)
            {
                inst = candidate;
                break;
            }
        }

        if (inst == null)
        {
            inst = Instantiate(prefab, transform);
            instanceToPrefab[inst] = prefab;
        }

        inst.transform.SetPositionAndRotation(position, rotation);
        inst.SetActive(true);
        return inst;
    }

    public void Return(GameObject instance)
    {
        if (instance == null) return;

        if (!instanceToPrefab.ContainsKey(instance))
        {
            Destroy(instance);
            return;
        }

        var prefab = instanceToPrefab[instance];
        instance.SetActive(false);
        instance.transform.SetParent(transform);

        if (!pools.ContainsKey(prefab))
            pools[prefab] = new Queue<GameObject>();

        pools[prefab].Enqueue(instance);
    }
}