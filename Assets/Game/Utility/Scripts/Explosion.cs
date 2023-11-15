using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public LayerMask mask;

    public Transform debugGizmo;//just for gizmo/balance purpose

    //falloff controls degree of function should be btwn 0 and 1. 0 = no damage reduction vs range, 1 = linear damage reduction vs distance
    public void Init (float radius, float damage, float force, Transform origin, float falloff = 0.5f, float stunTime = 0f)
    {
        debugGizmo.localScale *= radius;

        Collider[] cols = Physics.OverlapSphere(transform.position, radius, mask);
        List<Rigidbody> rbs = new List<Rigidbody>();

        foreach (Collider col in cols)
        {
            if (origin != null && col.transform.IsChildOf(origin)) continue;

            //FoliageDisplacementHandler d = col.GetComponentInParent<FoliageDisplacementHandler>();
            //if(d != null) d.Impact(transform.position, radius);

            //m = 1 - d^(1/falloff), d = distance/radius
            float m = GetFalloff(Vector3.Distance(transform.position, col.transform.position), radius, falloff);

            if (m <= 0) continue;

            PlayerLifeCycleHandler player = col.transform.GetComponentInParent<PlayerLifeCycleHandler>();
            if (player != null) player.TakeDamage(this, (int)(damage * m));

            Enemy enemy = col.transform.GetComponentInParent<Enemy>();
            if(enemy != null)
            {
                enemy.TakeDamage((int)(damage * m));
                if(stunTime > 0) enemy.Stun(stunTime);
            }

            Rigidbody rb = col.transform.GetComponentInParent<Rigidbody>();
            if(rb != null && !rbs.Contains(rb))
            {
                rb.AddForce((col.transform.position - transform.position).normalized * m * force, ForceMode.Impulse);
                rbs.Add(rb);
            }
        }
        Destroy (gameObject, 3f);
    }


    public static float GetFalloff(float distance, float radius, float falloff = 0.5f)
    {
        float m = 1 - Mathf.Pow(distance / radius, 1f / falloff);
        return Mathf.Clamp(m,0,1);
    }
}