using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum EnemyState { wandering, chasing, engaging, stunned, dead }

    [Header("Stats")]
    public int health;
    public int maxHealth;

    public float sightRange;
    public float sightAngle;

    public float loseSightTime; //how long it takes for enemy to be unaware of player's position after losing sight

    public float patrolRange;

    [Header("Status")]
    public EnemyState enemyState;

    public float angleToPlayer;
    public bool canSeePlayer;
    
    public Vector3 moveLocation;
    public Vector3 scale;

    [Space]
    [Header("References")]
    private NavMeshAgent agent;

    public Transform player;

    public LayerMask sightMask;


    public void Start()
    {
        scale = transform.localScale;

        //TODO: pass in when spawning
        player = FindObjectOfType<PlayerMovement>().transform;

        agent = GetComponent<NavMeshAgent>();

        agent.SetDestination(moveLocation);
    }

    private void Update()
    {
        Vector3 playerDir = player.position-transform.position;
        angleToPlayer = Vector3.Angle(transform.forward, playerDir);

        if(playerDir.magnitude <= sightRange * 1.1f)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, playerDir, out hit, sightRange, sightMask);
            if (angleToPlayer <= sightAngle && hit.collider != null && hit.collider.transform == player)
            {
                StopAllCoroutines();
                canSeePlayer = true;
                enemyState = EnemyState.engaging;
            }
            else if (canSeePlayer && !enemyState.Equals(EnemyState.chasing)) //should only trigger on frame where sight lost
            {
                print("lost sight");
                enemyState = EnemyState.chasing;
                StartCoroutine(LoseSight());
            }
        }

        if (enemyState == EnemyState.wandering)
        {
            if (Vector3.Distance(transform.position, moveLocation) <= agent.stoppingDistance)
            {
                NavMeshHit hit;
                Vector3 point = Vector3.positiveInfinity;
                while (!NavMesh.SamplePosition(point, out hit, agent.height*2, NavMesh.AllAreas)) point = Random.insideUnitSphere * patrolRange + transform.position;

                moveLocation = hit.position;
                agent.SetDestination(hit.position);
            }
        }
        else if (enemyState == EnemyState.chasing)
        {
            if (canSeePlayer) moveLocation = player.position;
            agent.SetDestination(moveLocation);

            if (Vector3.Distance(transform.position, moveLocation) <= agent.stoppingDistance) enemyState = EnemyState.wandering;
        }
        else if (enemyState == EnemyState.engaging)
        {
            moveLocation = player.position;
            agent.SetDestination(moveLocation);
        }
    }

    private IEnumerator LoseSight()
    {
        yield return new WaitForSeconds(loseSightTime);
        canSeePlayer = false;
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