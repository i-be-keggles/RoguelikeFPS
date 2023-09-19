using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AttackDrone : DefensePod
{
    public float hSpeed;
    public float vSpeed;
    public float oSpeed;

    public float orbitRadius;
    public float hoverHeight;
    private float curHeight;

    public float orbitLead;

    public Transform player;
    private Rigidbody rb;


    private void Start()
    {
        interactable = GetComponent<Interactable>();
        rb = GetComponent<Rigidbody>();
        ammo = maxAmmo;
    }

    private void Update()
    {
        timeToFire -= Time.deltaTime;
        if (target == null)
        {
            EvaluateTarget();
            Orbit(player.position);

            transform.forward = Vector3.Lerp(transform.forward, rb.velocity, turnSpeed * Time.deltaTime);
        }
        else
        {
            Orbit(target.transform.position);

            Vector3 dir = target.CentrePoint() - transform.position;
            float angleTo = Vector3.Angle(transform.forward, dir);
            if (angleTo > fireAngle / 3)
            {
                float y = Vector3.SignedAngle(transform.forward, dir, Vector3.up);
                float x = Vector3.Angle(dir, new Vector3(dir.x, 0, dir.z)) * (transform.position.y > target.CentrePoint().y ? -1 : 1);
                float lx = transform.localEulerAngles.x > 180? transform.localEulerAngles.x - 360: transform.localEulerAngles.x;
                float px = lx - x * (Math.Abs(x) > Math.Abs(lx) ? -1 : 1);

                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0) + new Vector3(-px, y + y*orbitLead) * turnSpeed * Time.deltaTime;
            }

            if (timeToFire <= 0 && angleTo <= fireAngle)
            {
                Fire();
                print("Firing");
            }
        }

        ControlHeight();
    }

    public void ControlHeight()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity, hitMask))
        {
            curHeight = transform.position.y - Math.Max(hit.point.y, target==null? player.position.y : target.CentrePoint().y);
        }

        rb.AddForce(Vector3.up * (hoverHeight - curHeight) * vSpeed * 10 * Time.deltaTime, ForceMode.Force);
    }

    public void Orbit(Vector3 p)
    {
        Vector3 to = p - transform.position;
        to.y = 0;
        rb.AddForce(to.normalized * (to.magnitude - orbitRadius) * hSpeed * 10 * Time.deltaTime, ForceMode.Force); 
        if(to.magnitude < orbitRadius * 1.5f)
        {
            Vector3 right = Vector3.Cross(to, Vector3.up).normalized;
            rb.AddForce(right * (Vector3.Dot(rb.velocity, right) > 0 ? 1 : -1) * oSpeed * 10 * Time.deltaTime, ForceMode.Force);
        }
    }
}