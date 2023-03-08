using System;
using UnityEngine;

public class RecoveryMissionManager : MissionManager
{
    public GameObject recoveryItem;
    public Transform dropOff;

    class RecoveryObjective : Objective
    {
        Interactable item;        //physical object to pickup
        Interactable target;       //where it needs to be taken

        public bool carryingObject = false;

        public EnemySpawning spawnManager;

        public RecoveryObjective(string s, bool c, float r, float t, Vector3 position, GameObject obj, Interactable tr, EnemySpawning sp) : base(s, c, r, t)
        {
            name = s;
            completed = c;
            rewards = r;
            time = t;
            target = tr;

            item = Instantiate(obj, position, Quaternion.identity).GetComponent<Interactable>();
            item.interactedWith += Pickup;
            target.interactedWith += Place;

            spawnManager = sp;
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
                Complete(time, spawnManager);
            }
        }
    }
}