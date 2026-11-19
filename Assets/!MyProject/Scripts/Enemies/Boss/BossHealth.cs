using System;
using UnityEngine;

public class BossHealth : EnemyHealth
{
    [SerializeField] private BossData bossData;

    public BossData BossData => bossData;

    public event Action OnStompPhase;
    public event Action OnEnragePhase;

    private bool _stompPhaseTriggered;
    private bool _enragePhaseTriggered;

    private float HealthPercent => MaxHealth > 0f ? CurrentHealth / MaxHealth : 0f;

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);

        if (!_stompPhaseTriggered && HealthPercent <= 0.5f)
        {
            _stompPhaseTriggered = true;
            OnStompPhase?.Invoke();
        }

        if (!_enragePhaseTriggered && HealthPercent <= 0.2f)
        {
            _enragePhaseTriggered = true;
            OnEnragePhase?.Invoke();
        }
    }
}