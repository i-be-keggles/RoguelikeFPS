using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildHeightSnapper : MonoBehaviour
{
    public LayerMask mask;

    private void Start()
    {
        Snap();
    }

    public void Snap()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            Transform t = transform.GetChild(i);

            RaycastHit hit;
            if (Physics.Raycast(t.position + Vector3.up * 10f, -Vector3.up, out hit, 50f, mask))
            {
                t.transform.position = hit.point;
            }
        }
    }
}
