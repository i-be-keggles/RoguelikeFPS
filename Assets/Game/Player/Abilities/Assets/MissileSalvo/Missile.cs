using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class Missile : MonoBehaviour
{
    public float safetyTimer;
    public Collider collider;

    private Vector3? target;
    private float speed;
    private float turnSpeed;

    private Rigidbody rb;

    public GameObject explosion;

    public LayerMask mask;

    [Space]
    float radius, damage, force, falloff, stunTime;

    private void Start()
    {
        collider.enabled = false;
        rb = GetComponent<Rigidbody>();

        float spread = 5f;
        transform.localEulerAngles += new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread));
    }

    public void Init(Vector3? t, float s, float ts, float _radius, float _damage, float _force, float _falloff, float st)
    {
        target = t;
        speed = s;
        turnSpeed = ts;

        radius = _radius;
        damage = _damage;
        force = _force;
        falloff = _falloff;
        stunTime = st;

        StartCoroutine(Arm());
    }

    public void FixedUpdate()
    {
        rb.velocity = transform.forward * speed;
        
        if(target != null)
        {
            transform.forward = Vector3.Slerp(transform.forward, target.Value - transform.position, turnSpeed * Time.fixedDeltaTime);
        }

        RaycastHit hit;
        if(Physics.SphereCast(transform.position, 0.2f, transform.forward, out hit, 0.2f, mask))
        {
            Explode();
        }
    }

    private IEnumerator Arm()
    {
        yield return new WaitForSeconds(safetyTimer);
        collider.enabled = true;
        yield return new WaitForSeconds(10);
        Explode();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    public void Explode()
    {
        print("boom.");

        Explosion e = Instantiate(explosion, transform.position, Quaternion.identity).GetComponent<Explosion>();
        e.Init(radius, damage, force, transform, falloff, stunTime);
        Destroy(e, 3f);

        Destroy(gameObject);
    }
}
