using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Interactable : MonoBehaviour
{
    public event EventHandler interactedWith;

    public string promptText;
    public bool active;
    public bool promptOnly;

    private void Awake()
    {
        active = true;
    }

    public void Trigger(object sender, EventArgs e)
    {
        interactedWith.Invoke(sender, e);
    }
}