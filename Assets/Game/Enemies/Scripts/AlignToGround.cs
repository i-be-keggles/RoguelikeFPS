using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignToGround : MonoBehaviour
{
    public LayerMask mask;
    public float speed;
    private float rot;

    private void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, 3f, mask))
        {
            rot = Mathf.Lerp(rot, Vector3.SignedAngle(Vector3.up, hit.normal, transform.right), Time.deltaTime * speed);
            transform.localEulerAngles = new Vector3(rot, transform.localEulerAngles.y, transform.localEulerAngles.z);
        }
    }
}
