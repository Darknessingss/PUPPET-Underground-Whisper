using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] private float attackInterval;
    [SerializeField] private float fireInterval;
    [SerializeField] private float reloadInterval;
    [SerializeField] private float damage;

    [SerializeField] private float bulletSpeed;
    [SerializeField] public int magazineSize;


    protected float _lastAttackTime;
    protected int _currentAmmo;
    protected bool _isReloading;


    public float Damage => damage;
    public int CurrentAmmo => _currentAmmo;
    public int MagazineSize => magazineSize;
    public bool IsReloading => _isReloading;
}
