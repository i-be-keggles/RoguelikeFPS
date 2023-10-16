using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurveyPod : MonoBehaviour
{
    public GameObject marker;
    public float range;
    public Animator anim;
    private Interactable interactable;

    private void Start()
    {
        interactable = GetComponent<Interactable>();
        interactable.interactedWith += Scan;

        anim = GetComponentInChildren<Animator>();
        anim.enabled = false;
    }

    private void Scan(object sender, EventArgs e)
    {
        anim.enabled = true;
        interactable.promptText = "Already scanned.";
        interactable.promptOnly = true;

        LevelGenerator lg = GetComponentInParent<LevelGenerator>();
        if (lg == null) throw new Exception("Level generator not found.");
        List<GameObject> POIs = lg.POIs;

        int r = 0;
        for(int i = 0; i < POIs.Count - r; i++)
            if (Vector3.Distance(POIs[i - r].transform.position, transform.position) > range)
                POIs.RemoveAt(r++);

        marker = Instantiate(marker, POIs[UnityEngine.Random.Range(0, POIs.Count)].transform.position, Quaternion.identity, transform);
    }
}
