using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Interactor : MonoBehaviour
{
    public UIManager uiManager;
    public Transform cam;
    public KeyCode interactKey;

    public Interactable target;

    [Space]
    public float range;
    public float pickupRadius;
    public LayerMask mask;

    void Update()
    {
        RaycastHit hit;
        if (Physics.SphereCast(cam.position, pickupRadius, cam.forward, out hit, range, mask))
        {
            if (target == null || !hit.transform.IsChildOf(target.transform))
            {
                target = hit.transform.GetComponentInParent<Interactable>();
            }
        }
        else target = null;

        if (target != null && target.active)
        {
            if (target.promptOnly) uiManager.UpdateInteractText(target.promptText);
            else
            {
                uiManager.UpdateInteractText("[" + interactKey.ToString() + "] " + target.promptText);
                if (Input.GetKeyDown(interactKey))
                {
                    print("Triggered");
                    target.Trigger(this, EventArgs.Empty);
                }
            }

        }
        else uiManager.UpdateInteractText();
    }
}
