using UnityEngine;
using System;

public class OrbitalDrop : MonoBehaviour
{
    public event EventHandler landed;

    public float targetHeight;

    public float speed;
    public LayerMask mask;
    public AudioClip landSound;

    private void Awake()
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
            AudioSource source = GetComponent<AudioSource>();
            source.clip = landSound;
            source.loop = false;
            source.Play();
            this.enabled = false;
        }
    }

    public void PromptHeight(float height)
    {
        if(Math.Abs(height - targetHeight) > 2) targetHeight = height;
        print("Height is now: " + targetHeight);
    }
}