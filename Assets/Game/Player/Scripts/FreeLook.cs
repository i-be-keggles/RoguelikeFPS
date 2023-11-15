using UnityEngine;
public class FreeLook : MonoBehaviour
{
    public float speed = 10;
    public float shiftSpeed = 20;
    public Vector3 input = Vector3.zero;
    public Vector3 dir = Vector3.zero;
    public Vector3 aInput = Vector3.zero;
    public Vector3 aDir = Vector3.zero;
    public float accel = 3f;
    public float aaccel = 1f;
    public float lookSens = 0.5f;


    private void Start()
    {
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<MouseLook>().enabled = false;
    }

    private void FixedUpdate()
    {
        input = new Vector3(Input.GetAxis("Horizontal"), Input.GetKey(KeyCode.Space)? 1 : Input.GetKey(KeyCode.LeftControl) ? -1 : 0, Input.GetAxis("Vertical"));
        dir = Vector3.Lerp(dir, input, Time.fixedDeltaTime * accel);

        aInput = new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));
        aDir = Vector3.Lerp(aDir, aInput, Time.fixedDeltaTime * aaccel);

        shiftSpeed += Input.GetAxis("Mouse ScrollWheel") * Time.fixedDeltaTime*10;

        transform.position += transform.TransformDirection(dir) * Time.fixedDeltaTime * (Input.GetKey(KeyCode.LeftShift)? shiftSpeed : speed);
        transform.eulerAngles += aDir * lookSens;
    }
}