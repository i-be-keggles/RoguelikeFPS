using System;
using UnityEngine;
using UnityEngine.AI;

public class EscortMissionManager : MissionManager
{
    public GameObject payloadPrefab;
    public EscortPayload payload;

    [Header("Testing only")]
    Transform path; //child[0] = start, child[1] = destination

    private void Start()
    {
        objectives.Add(new EscortObjective("Escort payload", false, 100, time, this, payloadPrefab, path.GetChild(0).position, path.GetChild(1).position));
    }


    class EscortObjective : Objective
    {
        EscortPayload payload;
        Vector3 destination;

        float repairCutoff;         //distance to final destination where repair pod is no longer called

        public bool carryingObject = false;

        public EscortObjective(string s, bool c, float r, float t, MissionManager m, GameObject obj, Vector3 pos, Vector3 dest) : base(s, c, r, t, m)
        {
            name = s;
            completed = c;
            rewards = r;
            time = t;
            manager = m;

            repairCutoff = 50;
            destination = dest;

            payload = Instantiate(obj, pos, Quaternion.identity).GetComponent<EscortPayload>();
            payload.repairRequest += RepairRequest;
        }

        public void RepairRequest(object sender, EventArgs e)
        {
            if (Vector3.Distance(destination, payload.transform.position) <= repairCutoff)
            {
                print("Payload has suffered some serious damage. Calling down repair pod.");
                //call in pod
                RecoveryObject repair = new RecoveryObjective("Repair payload", false, mbox.time, m);
                repair.onCompleted += FinishRepair;
                manager.objectves.Add(repair);
                //pause payload
            }
            else print("Dropoff is just around the corner. Hang in there.");
        }

        public IEnumerator FinishRepair(object sender, EventArgs e)
        {
            //see if you can make this an ienumerator
            yield return new WaitForSeconds(20f);
            print("Repaired!");
            payload.curHealth += payload.maxHealth * 0.5f;
            //resume payload
        }
    }
}