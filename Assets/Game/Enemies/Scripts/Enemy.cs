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

    public float walkSpeed;
    public float runSpeed;

    public float damage;
    public float attackRange;
    public float attackSpeed;
    private float timeToAttack;

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
        if(timeToAttack > 0) timeToAttack -= Time.DeltaTime();

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
            Wander();
        }
        else if (enemyState == EnemyState.chasing)
        {
            Chase();
        }
        else if (enemyState == EnemyState.engaging)
        {
            Engage(playerDir);
        }
    }

    protected void Wander()
    {
        agent.speed = walkSpeed;
        if (Vector3.Distance(transform.position, moveLocation) <= agent.stoppingDistance)
        {
            NavMeshHit hit;
            Vector3 point = Vector3.positiveInfinity;
            while (!NavMesh.SamplePosition(point, out hit, agent.height * 2, NavMesh.AllAreas)) point = Random.insideUnitSphere * patrolRange + transform.position;

            moveLocation = hit.position;
            agent.SetDestination(hit.position);
        }
    }

    protected void Chase()
    {
        agent.speed = runSpeed;
        if (canSeePlayer) moveLocation = player.position;
        agent.SetDestination(moveLocation);

        if (Vector3.Distance(transform.position, moveLocation) <= agent.stoppingDistance) enemyState = EnemyState.wandering;
    }

    protected void Engage(Vector3 playerDir)
    {
        agent.speed = runSpeed;

        moveLocation = player.position;
        agent.SetDestination(moveLocation);

        if (playerDir.magnitude <= attackRange && timeToAttack <= 0) Attack();
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

    protected void Attack()
    {
        print("attacking");
        timeToAttack += 1/attackSpeed;

        player.TakeDamage(damage);
    }

    protected void Die()
    {
        Destroy(gameObject);
    }
}