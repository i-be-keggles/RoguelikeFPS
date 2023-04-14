using UnityEngine;
using System;

public class OrbitalDrop : MonoBehaviour
{
    public event EventHandler landed;

    private float targetHeight;

    public float speed;
    public LayerMask mask;

    private void Start()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, mask))
        {
            targetHeight = hit.point.y;
        }
        else targetHeight = transform.position.y - speed * 20f;
    }


    private void FixedUpdate()
    {
        if (transform.position.y > targetHeight)
        {
            transform.position = transform.position - new Vector3(0, Math.Min(transform.position.y - targetHeight, speed * Time.fixedDeltaTime), 0);
        }
        else
        {
            landed?.Invoke(this, EventArgs.Empty);
            print("Landed");
            this.enabled = false;
        }
    }
}