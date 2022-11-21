using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemy : Enemy
{
    public float dodgeInterval;
    public float dodgePower;

    private float timeToDodge;
    private int dodgeLeftRight = 1;

    public bool distancing; //small phase where enemy runs past player after attacking
    public float distancingMagnitude; //how far the enemy runs past player
    

    protected override void Engage(Vector3 playerDir)
    {
        agent.speed = runSpeed;
        timeToDodge -= Time.DeltaTime();

        if(timeToDodge <= 0 && !distancing)
        {
            Dodge();
        }

        if(!distancing) moveLocation = player.position;

        if(playerDir.magnitude <= agent.stoppingDistance && !distancing)
        {
            distancing = true;
            Attack();
            moveLocation = player.position + playerDir.normalized * distancingMagnitude;
        }
        if(Vector3.Distance(transform.position, movelocation) <= agent.stoppindDistance)
        {
            distancing = false;
        }

        agent.SetDestination(moveLocation);
    }

    private void Dodge()
    {
        timeToDodge = dodgeInterval;

        agent.Move(transform.right * dodgeLeftRight);
        dodgeLeftRight *= -1;
    }
}