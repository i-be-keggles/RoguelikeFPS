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
        }
        else
        {
            Orbit(target.transform.position);

            Vector3 dir = target.CentrePoint() - arms.position;
            float angleTo = Vector3.Angle(arms.forward, dir);
            if (angleTo > fireAngle / 3)
            {
                Vector3 targetRot = new Vector3(Mathf.Clamp(transform.eulerAngles.x + Vector3.SignedAngle(transform.forward, dir, transform.right), xLimit.x, xLimit.y) + Vector3.SignedAngle(transform.forward, dir, Vector3.up), transform.eulerAngles.z);
                transform.eulerAngles = Vector3.Lerp(head.eulerAngles, targetRot, turnSpeed * Time.deltaTime);
                //arms.eulerAngles = Vector3.Lerp(arms.eulerAngles, new Vector3(Mathf.Clamp(arms.eulerAngles.x + Vector3.SignedAngle(arms.forward, dir, arms.right), xLimit.x, xLimit.y), arms.eulerAngles.y, arms.eulerAngles.z), turnSpeed * Time.deltaTime);
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
            curHeight = transform.position.y - hit.point.y;
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
            print("orbiting");
        }
    }
}