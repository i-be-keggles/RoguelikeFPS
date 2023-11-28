using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class PlayerLifeCycleHandler : MonoBehaviour
{
    public int maxHealth = 100;
    public int curHealth;

    private UIManager ui;

    public bool god = false;

    private void Start()
    {
        curHealth = maxHealth;
        ui = FindObjectOfType<UIManager>();
        GetComponent<EnemyTarget>().onTakeDamage += TakeDamage;
    }

    public void Heal(int amount)
    {
        curHealth = Math.Min(curHealth + amount, maxHealth);
        ui.UpdateHealthBar(curHealth);
    }

    public void TakeDamage(object sender, int damage)
    {
        curHealth -= damage;
        ui.UpdateHealthBar(curHealth);
        if (curHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        print("died lol");
        if (!god)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
