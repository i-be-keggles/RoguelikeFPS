using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public MouseLook mouseLook;

    [Space]
    public TextMeshProUGUI ammoText;
    public RectTransform healthBar;
    public TextMeshProUGUI interactText;

    [Space]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI crystalText;

    [Space]
    public bool selectingPod = false;
    public GameObject podSelector;
    public DropPodHandler podHandler;
    private RectTransform[] segments;
    private int selectedSegment;


    private void Start()
    {
        InitPodSelector();
    }

    public void UpdateAmmoText(int cur, int tot)
    {
        ammoText.text = cur + " | " + tot;
    }
    
    public void ClearAmmoText()
    {
        ammoText.text = "-- | --";
    }

    public void UpdateHealthBar(int health)
    {
        healthBar.sizeDelta = new Vector2(health, healthBar.sizeDelta.y);
    }

    public void UpdateInteractText(string text = "")
    {
        interactText.text = text;
    }

    public void UpdateScoreText(float score, float crystal)
    {
        scoreText.text = "Score: " + Mathf.FloorToInt(score);
        crystalText.text = "Crystal: " + Mathf.FloorToInt(crystal);
    }

    private void InitPodSelector()
    {
        segments = new RectTransform[podHandler.pods.Length];
        for (int i = 0; i < podHandler.pods.Length; i++)
        {
            RectTransform segment = Instantiate(podSelector.transform.GetChild(0).GetChild(0).gameObject, podSelector.transform.GetChild(0).transform).GetComponent<RectTransform>();
            segment.localPosition = Vector3.zero;
            float gap = 0.02f;
            float fill = 1f / (podHandler.pods.Length) - gap;
            segment.GetComponent<Image>().fillAmount = fill;
            float r = ((fill + gap) * i) - (fill * 0.5f);
            segment.rotation = Quaternion.Euler(0, 0, -r * 360f);
            segment.GetChild(0).RotateAround(segment.position, Vector3.forward, -360f * (fill * 0.5f));
            segment.GetChild(0).eulerAngles = Vector3.zero;
            segment.GetChild(0).GetComponent<TextMeshProUGUI>().text = podHandler.pods[i].name;
            segments[i] = segment;

            /*
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            int j = i;
            entry.callback.AddListener((eventData) => { OnSelectUpdate(j); });
            EventTrigger trigger = segment.gameObject.AddComponent<EventTrigger>();
            trigger.triggers.Add(entry);*/

        }
        Destroy(podSelector.transform.GetChild(0).GetChild(0).gameObject);
    }

    public void TogglePodSelector()
    {
        selectingPod = !selectingPod;
        podSelector.SetActive(selectingPod);
        mouseLook.enabled = !selectingPod;
        Cursor.visible = selectingPod;
        Cursor.lockState = selectingPod ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void Update()
    {
        if (selectingPod)
        {
            Vector3 mousePos = Input.mousePosition - new Vector3(Screen.width/2f, Screen.height/2f);
            float angle = Vector3.Angle(Vector3.up, mousePos);
            if (mousePos.x < 0) angle = 360f - angle;
            angle += (360 / segments.Length) * 0.5f;
            if(angle > 360) angle = angle - 360;

            int selected = Mathf.FloorToInt(segments.Length * angle / 360f);
            if(selected != selectedSegment) OnSelectUpdate(selected);
        }
    }

    public void OnSelectUpdate(int index)
    {
        print("Selecting " + index);
        segments[selectedSegment].GetChild(0).GetComponent<TextMeshProUGUI>().text = podHandler.pods[selectedSegment].name;
        selectedSegment = index;
        segments[selectedSegment].GetChild(0).GetComponent<TextMeshProUGUI>().text = podHandler.pods[selectedSegment].description;
    }
}
