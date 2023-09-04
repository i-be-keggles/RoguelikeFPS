using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPodHandler : MonoBehaviour
{
    public Pod[] pods;

    public KeyCode podKey; //Hold to select, tap to call in

    public UIManager uiManager;

    public float timeHeld;
    public float holdTime = 0.3f;

    [System.Serializable]
    public struct Pod
    {
        public string name;
        [TextArea]
        public string description;
        public GameObject prefab;

        public Pod(string n, string d, GameObject g)
        {
            name = n;
            description = d;
            prefab = g;
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
            timeHeld = 0;
        }
    }
}
