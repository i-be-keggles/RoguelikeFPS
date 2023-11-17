using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Healthpack : MonoBehaviour
{
    public int health;

    private Interactable interactable;

    private void Start()
    {
        interactable = GetComponent<Interactable>();
        interactable.interactedWith += OnPickup;
    }

    public void OnPickup(object sender, EventArgs e)
    {
        PlayerLifeCycleHandler p = FindObjectOfType<PlayerLifeCycleHandler>();
        if (p.curHealth < p.maxHealth)
        {
            p.Heal(health);
            Destroy(gameObject);
        }
    }
}
