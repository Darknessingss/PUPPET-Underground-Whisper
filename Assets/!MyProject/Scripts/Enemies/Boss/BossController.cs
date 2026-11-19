using UnityEngine;

[RequireComponent(typeof(BossHealth))]
public class BossController : MonoBehaviour
{
    [SerializeField] private BossTentacleMechanic tentacleMechanic;
    [SerializeField] private BossStompMechanic stompMechanic;
    [SerializeField] private BossEnrageMechanic enrageMechanic;

    private BossHealth _bossHealth;

    private void Awake()
    {
        _bossHealth = GetComponent<BossHealth>();

        if (tentacleMechanic == null) tentacleMechanic = GetComponent<BossTentacleMechanic>();
        if (stompMechanic == null) stompMechanic = GetComponent<BossStompMechanic>();
        if (enrageMechanic == null) enrageMechanic = GetComponent<BossEnrageMechanic>();

        if (_bossHealth.BossData == null)
            Debug.LogError("BossData не назначен на BossHealth!");
    }

    private void Start()
    {
        _bossHealth.OnStompPhase += HandleStompPhase;
        _bossHealth.OnEnragePhase += HandleEnragePhase;
    }

    private void OnDestroy()
    {
        _bossHealth.OnStompPhase -= HandleStompPhase;
        _bossHealth.OnEnragePhase -= HandleEnragePhase;
    }

    private void HandleStompPhase()
    {
        if (stompMechanic != null) stompMechanic.StartCharge();
    }

    private void HandleEnragePhase()
    {
        if (enrageMechanic != null) enrageMechanic.Activate();
    }
}