using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurveyPod : MonoBehaviour
{
    public GameObject marker;
    public float range;
    public Transform pointer;

    private void Start()
    {
        GetComponent<OrbitalDrop>().landed += Scan;
    }

    private void Scan(object sender, EventArgs e)
    {
        LevelGenerator lg = GetComponentInParent<LevelGenerator>();
        if (lg == null) throw new Exception("Level generator not found.");
        List<GameObject> POIs = lg.POIs;

        int r = 0;
        for(int i = 0; i < POIs.Count - r; i++)
            if (Vector3.Distance(POIs[i - r].transform.position, transform.position) > range)
                POIs.RemoveAt(r--);

        marker = Instantiate(marker, POIs[UnityEngine.Random.Range(0, POIs.Count)].transform.position, Quaternion.identity, transform);

        StartCoroutine(Point());
    }

    private void Update()
    {
        if(range == -1)
            pointer.rotation = Quaternion.RotateTowards(Quaternion.Euler(pointer.forward), Quaternion.Euler(marker.transform.position - pointer.position), Time.deltaTime);
    }

    private IEnumerator Point()
    {
        range = -1;
        yield return new WaitForSeconds(10f);
        range = 0;
    }
}
