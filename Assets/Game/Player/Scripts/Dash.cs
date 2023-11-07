using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New DashAbility", menuName = "Abilities/Dash")]
public class Dash : PlayerAbility
{
    [Space]

    public float length;

    public float speed;
    public float vertForce;

    [Min(0)]
    public float easeOut;

    public override IEnumerator Trigger(PlayerAbilityHandler handler)
    {
        PlayerMovement move = handler.GetComponent<PlayerMovement>();

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 prevDir = move.GetForce();

        Vector3 dir = x == 0 && z == 0? handler.cam.forward : Vector3.zero;
        dir += x * handler.transform.right;
        dir += z * (z > 0? handler.cam.forward : handler.transform.forward);
        dir.y += vertForce / speed;

        move.recieveInput = false;
        LayerMask mask = move.groundMask;
        move.groundMask = LayerMask.NameToLayer("");
        move.SetForce(dir.normalized * speed);
        yield return new WaitForSeconds(length * 0.9f);
        if (prevDir.magnitude < easeOut)
        {
            move.SetForce(dir.normalized * easeOut);
            yield return new WaitForSeconds(length * 0.1f);
        }
        else move.SetForce(dir.normalized * prevDir.magnitude);

        move.groundMask = mask;
        move.recieveInput = true;


        yield return null;
    }
}