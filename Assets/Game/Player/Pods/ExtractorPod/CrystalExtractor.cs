using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CrystalExtractor : MonoBehaviour
{
    public float range;

    private OrbitalDrop orbitalDrop;
    private Interactable interactable;

    private void Start()
    {
        orbitalDrop = GetComponent<OrbitalDrop>();
        orbitalDrop.landed += OnLanded;
        interactable = GetComponent<Interactable>();
    }

    public void OnLanded(object sender, EventArgs e)
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, range);
        foreach(Collider col in cols)
        {
            CrystalDeposit c = col.GetComponentInParent<CrystalDeposit>();
            if (c != null)
            {
                c.ExtractorLanded(interactable);
                return;
            }
        }
        interactable.promptOnly = true;
        interactable.promptText = "No crystal deposit in range";
    }
}
