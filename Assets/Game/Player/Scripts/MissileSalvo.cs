using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New MissileSalvoAbility", menuName = "Abilities/Missile Salvo")]
public class MissileSalvo : PlayerAbility
{
    [Space]

    public int numMissiles;
    public GameObject missile;

    public float missileSpeed;
    public float missileTurnSpeed;

    public float fireInterval;

    [Tooltip("Ability has been activated (and destination locked); waiting for release (for direction control)")]
    public bool locked;
    public Vector3? lockPosition;
    public float lockRange;
    public LayerMask lockMask;


    public override IEnumerator Trigger(PlayerAbilityHandler handler)
    {
        RaycastHit hit;
        if(Physics.Raycast(handler.cam.position, handler.cam.forward, out hit, lockRange, lockMask)){
            lockPosition = hit.point;
        }
        locked = true;

        yield return null;
    }
    public override IEnumerator SecondaryTrigger(PlayerAbilityHandler handler)
    {
        for (int i = 0; i < numMissiles; i++)
        {
            Instantiate(missile, handler.cam.position, handler.cam.rotation).GetComponent<Missile>().Init(lockPosition, missileSpeed, missileTurnSpeed);
            yield return new WaitForSeconds(fireInterval);
        }
        locked = false;
        lockPosition = null;
    }
}