using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth = 100;

    [SerializeField] private Slider healthBar;

    private EnemyAI enemyAI;

    public event Action OnDestroyed;

    private void Start()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();

        enemyAI = GetComponent<EnemyAI>();
    }

    private void Update()
    {
        UpdateHealthBar();
    }

    public void AdjustCurrentHealth(int adj)
    {
        currentHealth += adj;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (enemyAI != null)
        {
            enemyAI.DropCow();
        }

        if (OnDestroyed != null)
        {
            OnDestroyed.Invoke();
        }

        // TODO: VFX

        Destroy(gameObject);
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = (float)currentHealth / maxHealth;
        }
    }
}
