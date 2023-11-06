using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDMarker : MonoBehaviour
{
    public enum DisplayType { AlwaysActive, RemoveOnApproach, HideOnApproach, ShowOnApproach };

    public Vector3 worldPos;
    public DisplayType type;

    private RectTransform rt;
    private Image image; 

    public float distanceCutoff;
    public float distanceToPlayer;

    [HideInInspector] public UIManager manager;
    [HideInInspector] public Camera cam;

    private void Start()
    {
        rt = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    public void Update()
    {
        bool display = true;
        rt.position = Camera.main.WorldToScreenPoint(worldPos);
        if (Vector3.Dot(worldPos - cam.transform.position, cam.transform.forward) < 0) display = false;

        distanceToPlayer = Vector3.Distance(worldPos, cam.transform.position);

        switch (type)
        {
            case DisplayType.AlwaysActive:
                break;
            case DisplayType.RemoveOnApproach:
                if (distanceToPlayer < distanceCutoff) manager.RemoveHUDMarker(this);
                break;
            case DisplayType.HideOnApproach:
                if (distanceToPlayer < distanceCutoff) display = false;
                break;
            case DisplayType.ShowOnApproach:
                if (distanceToPlayer > distanceCutoff) display = false;
                break;
        }

        image.enabled = display;
    }
}
