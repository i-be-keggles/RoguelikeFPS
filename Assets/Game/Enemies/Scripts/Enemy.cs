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

    public int damage;
    public float attackRange;
    public float attackSpeed;
    protected float timeToAttack;

    [Header("Status")]
    public EnemyState enemyState;

    public float angleToPlayer;
    public bool canSeePlayer;
    
    public Vector3 moveLocation;
    public Vector3 scale;

    [Space]
    [Header("References")]
    protected NavMeshAgent agent;

    public PlayerLifeCycleHandler player;

    public LayerMask sightMask;

    private Coroutine loseSight;


    public void Start()
    {
        scale = transform.localScale;

        //TODO: pass in when spawning
        player = FindObjectOfType<PlayerLifeCycleHandler>();

        agent = GetComponent<NavMeshAgent>();

        agent.SetDestination(moveLocation);
    }

    protected void Update()
    {
        if(timeToAttack > 0) timeToAttack -= Time.deltaTime;

        Vector3 playerDir = player.transform.position-transform.position;
        angleToPlayer = Vector3.Angle(transform.forward, playerDir);

        if(playerDir.magnitude <= sightRange * 1.1f)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, playerDir, out hit, sightRange, sightMask);
            if (angleToPlayer <= sightAngle && hit.collider != null && hit.collider.transform == player.transform)
            {
                if(loseSight != null) StopCoroutine(loseSight);
                canSeePlayer = true;
                enemyState = EnemyState.engaging;
            }
            else if (canSeePlayer && !enemyState.Equals(EnemyState.chasing)) //should only trigger on frame where sight lost
            {
                print("lost sight");
                enemyState = EnemyState.chasing;
                loseSight = StartCoroutine(LoseSight());
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

    protected virtual void Wander()
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

    protected virtual void Chase()
    {
        agent.speed = runSpeed;
        if (canSeePlayer) moveLocation = player.transform.position;
        agent.SetDestination(moveLocation);

        if (Vector3.Distance(transform.position, moveLocation) <= agent.stoppingDistance) enemyState = EnemyState.wandering;
    }

    protected virtual void Engage(Vector3 playerDir)
    {
        agent.speed = runSpeed;

        moveLocation = player.transform.position;
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

    protected virtual void Attack()
    {
        print("attacking");
        timeToAttack += 1/attackSpeed;

        player.TakeDamage(damage);
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}