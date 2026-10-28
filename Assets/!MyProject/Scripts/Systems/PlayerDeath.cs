using System;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public event Action OnPlayerDied;

    [Header("Death UI")]
    [SerializeField] private ManagerInterface uiManager;

    private bool _isDead;
    public void TryDie()
    {
        if (_isDead) 
            return;

        _isDead = true;

        OnPlayerDied?.Invoke();
        uiManager.ShowDeathScreen();
    }
}