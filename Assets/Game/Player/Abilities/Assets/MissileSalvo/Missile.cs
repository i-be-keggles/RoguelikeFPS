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

    private void Start()
    {
        collider.enabled = false;
        rb = GetComponent<Rigidbody>();

        float spread = 5f;
        transform.localEulerAngles += new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread));
    }

    public void Init(Vector3? t, float s, float ts)
    {
        target = t;
        speed = s;
        turnSpeed = ts;
        StartCoroutine(Arm());
    }

    public void FixedUpdate()
    {
        rb.velocity = transform.forward * speed;
        
        if(target != null)
        {
            transform.forward = Vector3.Slerp(transform.forward, target.Value - transform.position, turnSpeed * Time.fixedDeltaTime);
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

        //spawn fx
        //delete fx

        Destroy(gameObject);
    }
}
