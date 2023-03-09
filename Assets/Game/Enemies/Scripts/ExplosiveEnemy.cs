using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.AI;

public class ExplosiveEnemy : Enemy
{
    public float explosionRadius;
    public float explosionForce;

    public float fuseLength; //time it takes to explode after activation
    public float fuseTime; //current timer time

    public float defuseLimit; //how long fuse has to be lit before being unable to defuse

    public bool fuseLit;

    [Space]
    public GameObject explosion;

    public float radius;
    public float force;
    public float falloff = 0.5f;


    private void Start()
    {
        fuseTime = fuseLength;
        base.Start();
    }

    private void Update()
    {
        if (fuseLit) fuseTime -= Time.deltaTime;
        else base.Update();
        if (fuseTime <= 0) Die();

        if (Vector3.Distance(player.transform.position, transform.position) > explosionRadius * 0.75 && fuseLit && fuseTime > fuseLength - defuseLimit)
        {
            fuseLit = false;
        }
    }

    protected override void Attack()
    {
        if (fuseLit) return;

        fuseLit = true;
        fuseTime = fuseLength;
        //play animation
    }

    //just create standardized explosion object
    protected override void Die()
    {
        Instantiate(explosion, transform.position, Quaternion.identity).GetComponent<Explosion>().Init(radius, damage, force, transform, falloff);
        base.Die();
    }
}