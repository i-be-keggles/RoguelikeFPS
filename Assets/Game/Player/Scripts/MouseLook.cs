using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float sensitivity = 2;

    [SerializeField] private Transform cam;
    [SerializeField] private Transform root;

    public float min = -90;
    public float max = 90;


    private float xRot = 0;
    private float yRot;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yRot = root.localEulerAngles.y;
    }

    void Update()
    {
        xRot -= Input.GetAxis("Mouse Y") * sensitivity;
        yRot += Input.GetAxis("Mouse X") * sensitivity;

        xRot = Mathf.Clamp(xRot, min, max);

        cam.localEulerAngles = new Vector3(xRot, 0, 0);
        root.localEulerAngles = new Vector3(0, yRot, 0);
    }
}
