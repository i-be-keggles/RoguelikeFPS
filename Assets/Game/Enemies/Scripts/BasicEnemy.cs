using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Principal;
using Unity.Burst.CompilerServices;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemy : Enemy
{
    public float dodgeInterval;
    public float dodgePower;

    private float timeToDodge;
    [SerializeField]private int dodgeLeftRight = 1;

    public bool distancing; //small phase where enemy runs past player after attacking
    public float distancingMagnitude; //how far the enemy runs past player

    [SerializeField]private bool dodging;


    protected override void Engage()
    {
        if (target == null) return;

        agent.speed = runSpeed;
        timeToDodge -= Time.deltaTime;

        Vector3 dir = transform.position - player.transform.position;

        if (timeToDodge <= 0 && !distancing && !dodging && dir.magnitude > attackRange*3 && angleToPlayer < 45)
        {
            StartCoroutine(Dodge());
        }

        if(!distancing) moveLocation = target.transform.position;

        if(dir.magnitude <= attackRange && !distancing && timeToAttack <= 0)
        {
            distancing = true;
            Attack();
            NavMeshHit hit;
            moveLocation = target.transform.position + new Vector3(dir.normalized.x, 0, dir.normalized.z) * distancingMagnitude;
            if (NavMesh.SamplePosition(moveLocation, out hit, agent.height * 2, NavMesh.AllAreas)) moveLocation = hit.position;
            else distancing = false;

            StartCoroutine(Dodge());
        }
        if(Vector3.Distance(transform.position, moveLocation) <= agent.stoppingDistance)
        {
            distancing = false;
        }

        if(dodging) agent.Move(transform.right * dodgeLeftRight * Time.deltaTime * dodgePower);

        agent.SetDestination(moveLocation);
    }

    protected override void Chase()
    {
        if(distancing)Engage();
        else base.Chase();
    }

    private IEnumerator Dodge()
    {
        dodging = true;

        yield return new WaitForSeconds(0.25f);

        dodging = false;
        timeToDodge = dodgeInterval;
        dodgeLeftRight *= -1;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = UnityEngine.Color.red;
        Gizmos.DrawSphere(moveLocation, 0.5f);
    }
}