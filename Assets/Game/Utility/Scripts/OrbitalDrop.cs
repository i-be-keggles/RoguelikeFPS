using UnityEngine;
using System;

public class OrbitalDrop : MonoBehaviour
{
    public event EventHandler landed;

    public float targetHeight;

    public float speed;
    public LayerMask mask;
    public AudioClip landSound;

    public GameObject trailParticles;
    public GameObject impactParticles;
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
        if (transform.position.y - targetHeight > 0.01f)
        {
            transform.position = transform.position - new Vector3(0, speed * Time.fixedDeltaTime, 0);
        }
        else
        {
            landed?.Invoke(this, EventArgs.Empty);
            transform.position = new Vector3(transform.position.x, targetHeight, transform.position.z);
            AudioSource source = GetComponent<AudioSource>();
            source.clip = landSound;
            source.loop = false;
            source.Play();
            this.enabled = false;
            if (dirtImpact != null) Instantiate(dirtImpact, transform.position, Quaternion.LookRotation(dirtRot), transform);
            if (impactParticles != null) Destroy(Instantiate(impactParticles, transform.position, transform.rotation), 5f);
            if (trailParticles != null) Destroy(trailParticles, 4f);

            Collider[] cols = Physics.OverlapSphere(transform.position, 15f);
            foreach(Collider col in cols)
            {
                CameraShake s = col.GetComponentInParent<CameraShake>();
                if (s != null)
                {
                    s.Shake(transform.position, 15, 0.5f, 0.7f);
                }
            }
        }
    }

    public float HeightFromTime(float time)
    {
        return speed * time;
    }
}