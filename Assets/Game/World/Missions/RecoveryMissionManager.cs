using System;
using UnityEngine;

public class RecoveryMissionManager : MissionManager
{
    public GameObject recoveryItem;
    public Interactable dropOff;

    [Header("Testing only")]
    Transform[] itemSpawns;

    private void Start()
    {
        foreach(Transform t in itemSpawns)
        {
            objectives.Add(new RecoveryObjective("Collect item", false, 100, time, this, t.position, recoveryItem, dropOff));
        }
    }


    class RecoveryObjective : Objective
    {
        Interactable item;          //physical object to pickup
        Interactable target;        //where it needs to be taken

        public bool carryingObject = false;

        public RecoveryObjective(string s, bool c, float r, float t, MissionManager m, Vector3 position, GameObject obj, Interactable tr) : base(s, c, r, t, m)
        {
            name = s;
            completed = c;
            rewards = r;
            time = t;
            target = tr;
            manager = m;

            item = Instantiate(obj, position, Quaternion.identity).GetComponent<Interactable>();
            item.interactedWith += Pickup;
            target.interactedWith += Place;
        }

        public void Pickup(object sender, EventArgs e)
        {
            carryingObject = true;
        }

        public void Place(object sender, EventArgs e)
        {
            if (carryingObject)
            {
                carryingObject = false;
                //Complete(time, spawnManager);
                manager.CompleteObjective(manager.objectives.getIndexOf(this));
            }
        }
    }
}