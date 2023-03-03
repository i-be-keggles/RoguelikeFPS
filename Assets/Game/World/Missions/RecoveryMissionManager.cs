using UnityEngine;

public class RecoveryMissionManager : MissionManager
{
    public GameObject recoveryItem;
    public Transform dropOff;

    class RecoveryObjective : MissionManager.Objective
    {
        Interactable item;        //physical object to pickup
        Interactable target;       //where it needs to be taken

        public bool carryingObject = false;

        public RecoveryObjective(string s, bool c, float r, float t, Vector3 position, GameObject obj, Interactable tr)
        {
            name = s;
            completed = c;
            rewards r;
            time = t;
            target = tr;

            item = Instantiate(obj, position, Quaternion.Identity).GetComponent<Interactable>();
            item.InteractedWith += Pickup;
            target.InteractedWith += Place;
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
                Complete(e.t);
            }
        }
    }
}