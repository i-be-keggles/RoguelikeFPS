using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;

public class AirstrikePayload : MonoBehaviour
{
    private float damage;
    private float radius;
    private float speed;
    private float force;
    private float falloff;

    public LayerMask mask;

    public GameObject explosion;

    public float timer;

    public void Init(float _damage, float _radius, float _speed, float _force, float _falloff)
    {
        damage = _damage;
        radius = _radius;
        speed = _speed;
        force = _force;
        falloff = _falloff;

        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.2f, -Vector3.up, out hit, speed * 10, mask))
        {
            timer = (transform.position.y - hit.point.y) / speed;
        }
        else timer = speed;
    }

    private void FixedUpdate()
    {
        transform.position = transform.position - new Vector3(0, speed * Time.fixedDeltaTime, 0);
        
        timer -= Time.fixedDeltaTime;
        if (timer <= 0) Explode();
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