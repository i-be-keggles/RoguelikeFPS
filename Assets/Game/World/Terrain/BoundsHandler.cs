using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsHandler : MonoBehaviour
{
    public PlayerLifeCycleHandler player;
    public UIManager manager;

    public float warnRange;
    string message = "WARNING: Lethal radiation detected.";

    private void Update()
    {
        float h = player.transform.position.y - transform.position.y;

        if(h < warnRange)
        {
            manager.UpdateInteractText(message);
            if (h < 0) player.Die();
        }
        else if (manager.interactText.text.CompareTo(message) == 0) manager.UpdateInteractText("");
    }
}
