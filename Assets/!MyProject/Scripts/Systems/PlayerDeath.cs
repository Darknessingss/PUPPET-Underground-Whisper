using System;
using System.Collections;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public event Action OnPlayerDied;

    [SerializeField] private DeathScreenManager deathScreen;
    [SerializeField] private EkController controller;
    [SerializeField] private Animator animator;
    [SerializeField] private string deathStateName = "DeathState";
    [SerializeField] private float deathAnimationDuration = 1.5f;
    [SerializeField] private float crossFadeDuration = 0.1f;

    private bool _isDead;
    public bool IsDead => _isDead;

    public void TryDie()
    {
        if (_isDead) return;
        _isDead = true;

        if (controller != null) controller.SetDead();
        OnPlayerDied?.Invoke();
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        if (animator != null)
        {
            animator.CrossFade(deathStateName, crossFadeDuration);
            yield return new WaitForSeconds(deathAnimationDuration);
        }

        Time.timeScale = 0f;

        if (deathScreen != null)
            deathScreen.PlayDeathSequence();
    }
}