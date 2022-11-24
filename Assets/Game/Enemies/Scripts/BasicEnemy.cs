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


    protected override void Engage(Vector3 playerDir)
    {
        agent.speed = runSpeed;
        timeToDodge -= Time.deltaTime;

        if(timeToDodge <= 0 && !distancing && !dodging && playerDir.magnitude > attackRange*3 && angleToPlayer < 45)
        {
            StartCoroutine(Dodge());
        }

        if(!distancing) moveLocation = player.transform.position;

        if(playerDir.magnitude <= attackRange && !distancing && timeToAttack <= 0)
        {
            distancing = true;
            Attack();
            NavMeshHit hit;
            moveLocation = player.transform.position + new Vector3(playerDir.normalized.x, 0, playerDir.normalized.z) * distancingMagnitude;
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
        if(distancing)Engage(player.transform.position - transform.position);
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