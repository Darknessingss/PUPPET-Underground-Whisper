using System;
using UnityEngine;
using UnityEngine.Assemblies;

public class EkHealth : MonoBehaviour
{

    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth = 100;

    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
    }


    private void Die()
    {
        if(currentHealth != 0)
        {
            isDead = true;
            Debug.Log("Ek has died.");
        }
    }
}
