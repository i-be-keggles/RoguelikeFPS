using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.EventSystems;

public class AirstrikeFlare : MonoBehaviour
{
    public EventHandler Triggered;

    private void OnCollisionEnter(Collision collision)
    {
        Trigger(this, EventArgs.Empty);
        GetComponent<Rigidbody>().isKinematic = true;
        Destroy(gameObject, 5f);
    }

    public void Trigger(object sender, EventArgs e)
    {
        Triggered?.Invoke(sender, e);
    }
}
