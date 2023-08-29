using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.AI;
using System;

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

    public int scoreValue;

    [Header("Status")]
    public EnemyState enemyState;

    public float angleToPlayer;
    public bool canSeePlayer;
    
    public Vector3 moveLocation;
    public Vector3 scale;

    [Space]
    [Header("References")]
    protected NavMeshAgent agent;

    public EnemyTarget player;
    public EnemyTarget target;

    public LayerMask sightMask;

    private Coroutine loseSight;
    public ScoreManager scoreManager;


    public void Start()
    {
        scale = transform.localScale;

        //TODO: pass in when spawning
        if(player == null) player = FindObjectOfType<PlayerLifeCycleHandler>().GetComponent<EnemyTarget>();
        agent = GetComponent<NavMeshAgent>();

        agent.SetDestination(moveLocation);
    }

    public void Init(EnemyTarget p, ScoreManager s)
    {
        player = p;
        scoreManager = s;
    }

    protected void Update()
    {
        if (player == null) return; //TODO: remove when properly passed (see start method)

        if (timeToAttack > 0) timeToAttack -= Time.deltaTime;

        Vector3 playerDir = player.transform.position-transform.position;
        angleToPlayer = Vector3.Angle(transform.forward, playerDir);

        if(playerDir.magnitude <= sightRange * 1.1f)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, playerDir, out hit, sightRange, sightMask);
            if ((target == null || target == player) && angleToPlayer <= sightAngle && hit.collider != null && hit.collider.transform == player.transform)
            {
                if(loseSight != null) StopCoroutine(loseSight);
                canSeePlayer = true;
                
                target = player;
                enemyState = EnemyState.engaging;
            }
            else if (canSeePlayer && !enemyState.Equals(EnemyState.chasing)) //should only trigger on frame where sight lost
            {
                enemyState = EnemyState.chasing;
                loseSight = StartCoroutine(LoseSight());
                target = null;
            }
        }

        if(target != null)
        {
            enemyState = EnemyState.engaging;
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
            Engage();
        }
    }

    protected virtual void Wander()
    {
        if (EvaluateTarget() != null) return;

        agent.speed = walkSpeed;
        if (Vector3.Distance(transform.position, moveLocation) <= agent.stoppingDistance)
        {
            NavMeshHit hit;
            Vector3 point = Vector3.positiveInfinity;
            while (!NavMesh.SamplePosition(point, out hit, agent.height * 2, NavMesh.AllAreas)) point = UnityEngine.Random.insideUnitSphere * patrolRange + transform.position;

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

    protected virtual void Engage()
    {
        agent.speed = runSpeed;

        moveLocation = target.transform.position;
        agent.SetDestination(moveLocation);

        if (Vector3.Distance(transform.position, target.transform.position) <= attackRange && timeToAttack <= 0) Attack();
    }

    private IEnumerator LoseSight()
    {
        yield return new WaitForSeconds(loseSightTime);
        canSeePlayer = false;
        EvaluateTarget();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        float s = 0.5f + ((float)health / (float)maxHealth)/2f;
        transform.localScale = scale * s;

        moveLocation = player.transform.position;

        EvaluateTarget();

        if (health <= 0) Die();
    }

    public EnemyTarget EvaluateTarget()
    {
        List<EnemyTarget> targets = new List<EnemyTarget>();
        float totPriority = 0;

        Collider[] cols = Physics.OverlapSphere(transform.position, sightRange);
        
        foreach(Collider col in cols)
        {
            EnemyTarget t = col.GetComponent<EnemyTarget>();
            if (t != null && t.isActiveAndEnabled)
            {
                targets.Add(t);
                totPriority += t.priority;
            }
        }

        float n = UnityEngine.Random.Range(0f, totPriority);

        for(int i = 0; i < targets.Count; i++)
        {
            n -= targets[i].priority;
            if(n < 0f)
            {
                if (targets[i] == player && !canSeePlayer) break;
                target = targets[i];
                return targets[i];
            }
        }

        return null;
    }

    public void OnLoseTarget(object sender, EventArgs e)
    {
        enemyState = EnemyState.wandering;
        EvaluateTarget();
    }

    protected virtual void Attack()
    {
        timeToAttack += 1/attackSpeed;

        target.TakeDamage(damage);
    }

    protected virtual void Die()
    {
        scoreManager.AddScore(scoreValue);
        Destroy(gameObject);
    }
}