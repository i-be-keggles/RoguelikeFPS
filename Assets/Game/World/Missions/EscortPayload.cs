using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EscortPayload : MonoBehaviour
{
    [Header("HP")]
    public int maxShield;
    public int curShield;
    public int maxHealth;
    public int curHealth;
    private float healTimer;
    private int shieldStatus; //0 = cooldown, 1 = regenerating

    [Space]
    public float shieldRegenRate;       //hp/second shield gains when not taking damage
    public float shieldDamageDelay;     //time in seconds shield starts regenerating after taking damage
    public float shieldRestartDelay;    //time in seconds shield starts regenerating after going down

    [Range(0, 1)] public float repairPodThreshold;  //percentage of health repair pod is called down
                                                    //repair pod is mandatory, restores 50% health
    [Header("NAV")]
    public float speed;
    public float moving;
    public NavMeshAgent agent;
    public NavMeshPath path;

    public EventHandler repairRequest;

    private void Update()
    {
        healTimer -= Time.deltaTime;
        if(healTimer <= 0)
        {
            curShield++;
            shieldStatus = 1;
            healTimer = 1f / shieldRegenRate;
        }
    }

    public void TakeDamage(int damage)
    {
        if(curShield > 0) curShield = Math.Max(0, curShield - damage);
        else
        {
            curHealth -= damage;
            if (curHealth <= curHealth * repairPodThreshold)
            {
                print("Requesting repair pod.");
                repairRequest.Invoke(this, EventArgs.Empty);
            }
            if(curHealth <= 0)
            {
                Die();
            }
        }

        healTimer = curShield <= 0 ? shieldRestartDelay : shieldDamageDelay;
        shieldStatus = 0;
    }

    public void Die()
    {
        print("Objective failed. Payload destroyed.");
    }
}