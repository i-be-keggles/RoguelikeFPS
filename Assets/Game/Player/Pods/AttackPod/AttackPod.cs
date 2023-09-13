using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPod : MonoBehaviour
{
    private OrbitalDrop orbitalDrop;
    public AttackDrone drone;

    private void Start()
    {
        orbitalDrop = GetComponent<OrbitalDrop>();
        orbitalDrop.landed += ActivateDrone;
        drone.enabled = false;
    }

    private void ActivateDrone(object sender, System.EventArgs e)
    {
        drone.player = FindObjectOfType<PlayerLifeCycleHandler>().transform;
        drone.GetComponent<Rigidbody>().isKinematic = false;
        drone.enabled = true;
    }
}
