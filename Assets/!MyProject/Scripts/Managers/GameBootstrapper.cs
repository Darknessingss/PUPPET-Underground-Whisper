using System.Collections;
using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    public static GameBootstrapper Instance { get; private set; }

    [SerializeField] private MusicManager musicManager;
    [SerializeField] private LoreManager loreManager;
    [SerializeField] private BulletPool bulletPool;
    [SerializeField] private EnemyAudioService enemyAudioService;
    [SerializeField] private WeaponAudioService weaponAudioService;
    [SerializeField] private UIAudioService uiAudioService;

    private bool _initialized;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private IEnumerator Start()
    {
        if (_initialized) yield break;
        _initialized = true;

        EnsureService(ref enemyAudioService, persist: false);
        EnsureService(ref weaponAudioService, persist: false);
        EnsureService(ref uiAudioService, persist: false);
        EnsureService(ref bulletPool, persist: false);
        EnsureService(ref loreManager, persist: false);
        EnsureService(ref musicManager, persist: true);

        yield return null;
        yield return null;
    }

    private void EnsureService<T>(ref T service, bool persist) where T : MonoBehaviour
    {
        if (service == null)
            service = FindFirstObjectByType<T>();

        if (service == null)
        {
            service = new GameObject($"[{typeof(T).Name}]").AddComponent<T>();
        }

        if (persist && service.transform.parent == null)
            DontDestroyOnLoad(service.gameObject);
    }
}