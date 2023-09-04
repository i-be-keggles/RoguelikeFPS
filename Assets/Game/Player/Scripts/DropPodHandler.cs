using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPodHandler : MonoBehaviour
{
    public Pod[] pods;

    public KeyCode podKey; //Hold to select, tap to call in

    public float dropTime;

    public UIManager uiManager;
    public ScoreManager scoreManager;

    public float timeHeld;
    public float holdTime = 0.3f;

    public int selectedPod;

    public bool callingPod;
    public GameObject podMarker;
    public LayerMask mask;
    public float range;


    [System.Serializable]
    public struct Pod
    {
        public string name;
        [TextArea]
        public string description;
        public GameObject prefab;
        public int cost;

        public Pod(string n, string d, GameObject g, int c)
        {
            name = n;
            description = d;
            prefab = g;
            cost = c;
        }
    }

    private void Update()
    {
        if (Input.GetKey(podKey))
        {
            timeHeld += Time.deltaTime;
            if(timeHeld >= holdTime && !uiManager.selectingPod) uiManager.TogglePodSelector();
        }

        if (Input.GetKeyUp(podKey))
        {
            if (uiManager.selectingPod) uiManager.TogglePodSelector();
            else if (callingPod) callingPod = false;
            else TryCallPod();
            timeHeld = 0;
        }

        if (callingPod)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, range, mask))
            {
                podMarker.SetActive(true);
                podMarker.transform.position = hit.point;
                podMarker.transform.parent = hit.transform;
            }
            if (Input.GetButtonDown("Fire1")) CallPod();
        }
        else podMarker.SetActive(false);
    }

    public bool TryCallPod()
    {
        if (scoreManager.crystal < pods[selectedPod].cost)
        {
            print("Not enough crystal to call down " + pods[selectedPod].name);
            return false;
        }

        callingPod = true;
        return true;
    }

    public void CallPod()
    {
        callingPod = false;
        if (pods[selectedPod].prefab == null)
        {
            throw new System.NotImplementedException("Pod prefab missing.");
        }
        Instantiate(pods[selectedPod].prefab, podMarker.transform.position + new Vector3(0, pods[selectedPod].prefab.GetComponent<OrbitalDrop>().HeightFromTime(dropTime)), Quaternion.Euler(0,UnityEngine.Random.Range(0f,360f),0), podMarker.transform.parent);
        scoreManager.UseCrystal(pods[selectedPod].cost);
    }
}
