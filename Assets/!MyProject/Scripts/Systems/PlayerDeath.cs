using System;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public event Action OnPlayerDied;

    [Header("Death UI")]
    [SerializeField] private DeathScreenManager deathScreen;

    private bool _isDead;
    public void TryDie()
    {
        if (_isDead) return;
        _isDead = true;

        OnPlayerDied?.Invoke();
        if (deathScreen != null)
            deathScreen.PlayDeathSequence();
    }
}