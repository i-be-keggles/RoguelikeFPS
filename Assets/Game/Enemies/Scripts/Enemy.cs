using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int health;
    public int maxHealth;

    private NavMeshAgent agent;

    public Vector3 scale;

    public Transform player;

    public void Start()
    {
        scale = transform.localScale;

        //TODO: pass in when spawning
        player = FindObjectOfType<PlayerMovement>().transform;

        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        agent.SetDestination(player.position);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        float s = 0.5f + ((float)health / (float)maxHealth)/2f;
        transform.localScale = scale * s;


        if (health <= 0) Die();
    }

    protected void Die()
    {
        Destroy(gameObject);
    }
}