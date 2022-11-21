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


    private void Update()
    {
        if (fuseLit) fuseTime -= Time.DeltaTime();
        if (fuseTime <= 0) Die();
    }

    protected override void Attack()
    {
        if (fuseLit) return;

        fuseLit = true;
        fuseTime = fuseLength;
        //play animation
    }

    protected override void Engage(Vector3 playerDir)
    {
        if(playerDir.magnitude > explosionRadius * 0.75 && fuseLit)
        {
            fuseLit = false;
        }

        super.Engage();
    }

    protected override void Die()
    {
        //explode fx

        GameObject[] objects = OverlapSphere(explosionRadius);

        foreach(GameObject go in objects)
        {
            Vector3 dir = go.transform.position - transform.position;
            float relativeMagnitude = (1 - dir.magnitude / explosionRadius);

            RigidBody rb = go.getComponentInParent<RigidBody>();
            if (rb != null) rb.addForce(dir.normalized * explosionForce  * relativeMagnitude, Physics.ExplosionForce);

            Enemy enemy = go.GetComponentInParent<Enemy>();
            if (enemy != null) enemy.TakeDamage(damage * relativeMagnitude);

            Player player = go.GetComponentInParent<Player>();
            if (player != null) player.TakeDamage(damage * relativeMagnitude);
        }
    }
}