using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;

public class EnemySpawning : AirstrikePayload
{
    private float damage;
    private float radius;
    private float speed;
    private float force;
    private float falloff;

    public LayerMask mask;

    public float activationRange = 0.2f; //range and radius of raycast to activate explosion
    public float activationRadius = 0.2f;


    public void Init(float _damage, float _radius, float _speed, float _force, float _falloff)
    {
        damage = _damage;
        radius = _radius;
        speed = _speed;
        force = _force;
        falloff = _falloff;
    }

    private void FixedUpdate()
    {
        transform.position = transform.position - new Vector3(0, speed * Time.deltaTime, 0);
        RaycastHit hit;
        if(Physics.SphereCast(transform.position, activationRadius, -Vector3.up, out hit, activationRange, mask))
        {
            Explode();
        }
    }

    public void Explode()
    {
        print("boom. (smol)");

        Explosion e = Instantiate(explosion, transform.position, Quaternion.identity).GetComponent<Explosion>();
        e.Init(radius, damage, force, transform, falloff);
        Destroy(e, 3f);

        Destroy(gameObject);
    }
}