using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLifeCycleHandler : MonoBehaviour
{
    public int maxHealth = 100;
    public int curHealth;

    private UIManager ui;

    private void Start()
    {
        curHealth = maxHealth;
        ui = FindObjectOfType<UIManager>();
    }

    public void TakeDamage(int damage)
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
        Application.Quit();
    }
}