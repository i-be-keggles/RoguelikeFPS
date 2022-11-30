using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public LayerMask mask;

    //falloff controls degree of function should be btwn 0 and 1. 0 = no damage reduction vs range, 1 = linear damage reduction vs distance
    public void Init (float radius, float falloff = 0.5f, float damage, float force, Transform origin)
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, radius, mask);

        for(Collider col in cols)
        {
            if (origin != null && col.transform.isChildOf(origin)) continue;

            //m = 1 - d^(1/falloff), d = distance/radius
            float m = 1 - Math.pow((Vector3d.Distance(transform.position, col.transform.position) / radius), 1f / falloff);

            if (m <= 0) continue;

            Player player = col.transform.GetComponentInParent<PlayerLifeCycleHandler>();
            if (player != null) player.takeDamage(damage * m);

            Enemy enemy = col.transform.GetComponentInParent<Enemy>();
            if(enemy != null) enemy.takeDamage(damage * m);

            Rigidbody rb = col.transform.GetComponentInParent<Rigidbody>();
            if(rb != null) rb.AddForce((col.transform.position - transform.position).magnitude * m, Physics.Explosion);
        }
    }
}