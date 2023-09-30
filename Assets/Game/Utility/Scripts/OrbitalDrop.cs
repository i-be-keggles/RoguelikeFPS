using UnityEngine;
using System;

public class OrbitalDrop : MonoBehaviour
{
    public event EventHandler landed;

    public float targetHeight;

    public float speed;
    public LayerMask mask;
    public AudioClip landSound;

    public GameObject dirtImpact;
    private Vector3 dirtRot;

    private void Awake()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position - Vector3.up*3f, -Vector3.up, out hit, mask))
        {
            targetHeight = hit.point.y;
            if (hit.collider.gameObject.layer != 9) dirtImpact = null;
            else dirtRot = hit.normal;
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
            AudioSource source = GetComponent<AudioSource>();
            source.clip = landSound;
            source.loop = false;
            source.Play();
            this.enabled = false;
            if (dirtImpact != null) Instantiate(dirtImpact, transform.position, Quaternion.LookRotation(dirtRot), transform);
        }
    }

    public void PromptHeight(float height)
    {
        if(Math.Abs(height - targetHeight) > 2) targetHeight = height;
    }

    public float HeightFromTime(float time)
    {
        return speed * time;
    }
}