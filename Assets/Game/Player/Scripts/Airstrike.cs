using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;

public class EnemySpawning : Airstrike      //for ease of implementation, it's called airstrike. Flavour it as an "orbital bombardment"
{
    public float totalDamage;
    public int nPayloads;
    public float travelTime; //time between ability activating (marker hitting ground) and the payloads hitting the ground
    public float travelSpeed; //m/s
    public float radius;

    public GameObject flarePrefab;
    private GameObject activeFlare;
    public GameObject payload; //make these like a bunch of laser/energy blasts - think mini gibraltar ult

    public float throwForce;

    [Space]
    float pRadius, pForce, pFalloff;

    public override IEnumerator Trigger(PlayerAbililtyHandler handler)
    {
        activeFlare = Instantiate(flarePrefab, handler.cam.position, Quaternion.LookRotation(cam.forward), null);
        activeFlare.GetComponent<Rigidbody>().addForce(cam.forward * throwForce, ForceMode.Impulse);                //later look into being able to aim it (with impact outline and everything)
    }

    //if you cant directly steal this from flare, have a script on flare to just call it here
    private void activeFlare.OnCollisionEnter()
    {
        Activate();
    }

    private void Activate()
    {
        for (int i = 0; i < nPayloads; i++)
        {
            Vector2 p = Random.insideUnitCircle * radius;
            Instantiate(payload, new Vector3(p.x, activeFlare.position.y + travelSpeed / travelTime, p.z).GetComponent<AirstrikePayload>().Init(totalDamage / nPayloads, pRadius, travelSpeed, pForce, pFalloff);
        }
    }
}