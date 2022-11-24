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
        print("exploded");
        base.Die();
        //explode fx

        /*
        GameObject[] objects = Physics.OverlapSphere(explosionRadius);

        foreach(GameObject go in objects)
        {
            Vector3 dir = go.transform.position - transform.position;
            float relativeMagnitude = (1 - dir.magnitude / explosionRadius);

            Rigidbody rb = go.GetComponentInParent<Rigidbody>();
            if (rb != null) rb.AddForce(dir.normalized * explosionForce  * relativeMagnitude, Physics.ExplosionForce);

            Enemy enemy = go.GetComponentInParent<Enemy>();
            if (enemy != null) enemy.TakeDamage(damage * relativeMagnitude);

            Player player = go.GetComponentInParent<Player>();
            if (player != null) player.TakeDamage(damage * relativeMagnitude);
        }
        */
    }
}