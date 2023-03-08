using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New AirstrikeAbility", menuName = "Abilities/Airstrike")]
public class Airstrike : PlayerAbility      //for ease of implementation, it's called airstrike. Flavour it as an "orbital bombardment"
{
    public float totalDamage;
    public int nPayloads;
    public float travelTime; //time between ability activating (marker hitting ground) and the payloads hitting the ground
    public float travelSpeed; //m/s
    public float radius;
    [Range(0,2)]public float variance;

    public GameObject flarePrefab;
    private GameObject activeFlare;
    public GameObject payload; //make these like a bunch of laser/energy blasts - think mini gibraltar ult

    public float throwForce;

    [Space]
    public float pRadius; //individual payload stats
    public float pForce;
    public float pFalloff;


    public override IEnumerator Trigger(PlayerAbilityHandler handler)
    {
        activeFlare = Instantiate(flarePrefab, handler.cam.position, Quaternion.LookRotation(handler.cam.forward), null);
        activeFlare.GetComponent<Rigidbody>().AddForce(handler.cam.forward * throwForce, ForceMode.Impulse);                //later look into being able to aim it (with impact outline and everything)
        Physics.IgnoreCollision(handler.GetComponent<Collider>(), activeFlare.GetComponent<Collider>());
        activeFlare.GetComponent<AirstrikeFlare>().Triggered += Activate;
        yield return null;
    }

    private void Activate(object sender, EventArgs e)
    {
        for (int i = 0; i < nPayloads; i++)
        {
            Vector3 p = UnityEngine.Random.insideUnitCircle * radius;
            p = new Vector3(p.x + activeFlare.transform.position.x, activeFlare.transform.position.y + travelSpeed * travelTime, p.y + activeFlare.transform.position.z);
            Instantiate(payload, p, Quaternion.identity).GetComponent<AirstrikePayload>().Init(totalDamage / nPayloads, pRadius, travelSpeed * UnityEngine.Random.Range(1f, 1f + variance), pForce, pFalloff);
        }
    }
}