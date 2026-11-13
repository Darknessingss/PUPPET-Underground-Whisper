using System.Collections;
using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    [SerializeField] private MusicManager musicManager;
    [SerializeField] private LoreManager loreManager;
    [SerializeField] private BulletPool bulletPool;
    [SerializeField] private EnemyAudioService enemyAudioService;
    [SerializeField] private WeaponAudioService weaponAudioService;
    [SerializeField] private UIAudioService uiAudioService;

    private void Awake()
    {
        Validate(musicManager);
        Validate(loreManager);
        Validate(bulletPool);
        Validate(enemyAudioService);
        Validate(weaponAudioService);
        Validate(uiAudioService);
    }

    private void Validate(Object service)
    {
        if (service == null)
            Debug.LogError($"[Bootstrapper] Не назначен сервис: проверьте инспектор.", this);
    }
}