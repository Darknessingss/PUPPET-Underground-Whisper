using System;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackInterval = 1.5f;
    [SerializeField] private float moveSpeed = 3.5f;

    private float attackTimer;

    private void Update()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackInterval)
        {
            attackTimer = 0f;
            Attack();
        }

        Move();
    }

    private void Attack()
    {

    }

    private void Move()
    {

    }
}
