using System;
using UnityEngine;

public class RecoveryMissionManager : MissionManager
{
    public GameObject recoveryItem;
    public Interactable dropOff;

    [Header("Testing only")]
    public Transform itemSpawns;

    private void Start()
    {
        int i = UnityEngine.Random.Range(0, itemSpawns.childCount);
        objectives.Add(new RecoveryObjective("Collect item", false, 100, time, this, itemSpawns.GetChild(i).position, recoveryItem, dropOff));
    }

    private void Update()
    {
        time += Time.deltaTime;
    }

    [Serializable]
    public class RecoveryObjective : Objective
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
            print("carrying");
        }

        public void Place(object sender, EventArgs e)
        {
            if (carryingObject)
            {
                carryingObject = false;
                //Complete(time, spawnManager);
                manager.CompleteObjective(manager.objectives.IndexOf(this));
            }
        }
    }
}