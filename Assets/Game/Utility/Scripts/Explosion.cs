using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public LayerMask mask;

    public Transform debugGizmo;//just for gizmo/balance purpose

    //falloff controls degree of function should be btwn 0 and 1. 0 = no damage reduction vs range, 1 = linear damage reduction vs distance
    public void Init (float radius, float damage, float force, Transform origin, float falloff = 0.5f)
    {
        debugGizmo.localScale *= radius;

        Collider[] cols = Physics.OverlapSphere(transform.position, radius, mask);

        foreach(Collider col in cols)
        {
            if (origin != null && col.transform.IsChildOf(origin)) continue;

            //m = 1 - d^(1/falloff), d = distance/radius
            float m = GetFalloff(Vector3.Distance(transform.position, col.transform.position), radius, falloff);

            if (m <= 0) continue;

            PlayerLifeCycleHandler player = col.transform.GetComponentInParent<PlayerLifeCycleHandler>();
            if (player != null) player.TakeDamage((int)(damage * m));

            Enemy enemy = col.transform.GetComponentInParent<Enemy>();
            if(enemy != null) enemy.TakeDamage((int)(damage * m));

            Rigidbody rb = col.transform.GetComponentInParent<Rigidbody>();
            if(rb != null) rb.AddForce((col.transform.position - transform.position).normalized * m, ForceMode.Impulse);
        }
        Destroy (gameObject, 3f);
    }


    public static float GetFalloff(float distance, float radius, float falloff = 0.5f)
    {
        float m = 1 - Mathf.Pow(distance / radius, 1f / falloff);
        return Mathf.Clamp(m,0,1);
    }
}