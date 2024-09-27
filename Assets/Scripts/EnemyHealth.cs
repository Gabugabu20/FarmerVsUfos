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

    [SerializeField] private Renderer ufoRenderer;
    private Material redOverlayMaterial;

    private EnemyAI enemyAI;

    [Header("Explosion Effect")]
    [SerializeField] private GameObject explosionPrefab;

    public event Action OnDestroyed;

    private void Start()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();

        if (ufoRenderer != null && ufoRenderer.materials.Length > 1)
        {
            redOverlayMaterial = ufoRenderer.materials[2];
        }

        UpdateUFOColor();

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
        UpdateUFOColor();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy died");
        if (enemyAI != null)
        {
            enemyAI.DropCow();
        }

        if (OnDestroyed != null)
        {
            OnDestroyed.Invoke();
        }

        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = (float)currentHealth / maxHealth;
        }
    }

    private void UpdateUFOColor()
    {
        if (redOverlayMaterial != null)
        {
            float damagePercentage = 1 - ((float)currentHealth / maxHealth);
            Color newColor = new Color(1, 0, 0, damagePercentage);
            redOverlayMaterial.color = newColor;
        }
    }
}
