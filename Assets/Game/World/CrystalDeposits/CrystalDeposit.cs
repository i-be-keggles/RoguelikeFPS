using UnityEngine;
using System;

public class CrystalDeposit : MonoBehaviour
{
    private Interactable interactable;

    public GameObject extractorPrefab;
    public GameObject extractor;

    public float dropTime;

    public float maxValue;
    public float curValue;

    [Tooltip("Amount of value lost per unit of damage.")]
    public float damageValueMultiplier = 1f;

    public float extractionTime;
    public float strikeInterval;
    private float timeToStrike;

    private EnemyTarget lifecycle;

    public float initialScoreValue = 100;
    private ScoreManager scoreManager;

    public int phase = 0; //0 = idle, 1 = pod dropping, 2 = waiting for extract, 3 = extracting, 4 = depleted


    private void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();

        interactable = GetComponent<Interactable>();
        interactable.interactedWith += HandlePhaseChange;
        lifecycle = GetComponent<EnemyTarget>();
        lifecycle.onTakeDamage += TakeDamage;
        lifecycle.enabled = false;

        curValue = maxValue;
    }

    private void Update()
    {
        if(phase == 3)
        {
            timeToStrike -= Time.deltaTime;
            if(timeToStrike <= 0)
            {
                timeToStrike = strikeInterval;
                Strike();
            }
        }
    }

    private void HandlePhaseChange(object sender, EventArgs e)
    {
        if(phase == 0)
        {
            interactable.active = false;
            interactable.enabled = false;

            CallPod();

            scoreManager.AddScore(initialScoreValue);
        }
        else if(phase == 1)
        {
            interactable.active = true;
            interactable.enabled = true;
            interactable.promptText = "Start extraction";
        }
        else if(phase == 2)
        {
            interactable.active = false;
            interactable.enabled = false;

            StartExtraction();
        }
        else if(phase == 3)
        {
            lifecycle.Deactivate();
            enabled = false;
        }

        phase++;
    }

    private void CallPod()
    {
        extractor = Instantiate(extractorPrefab, transform.position + new Vector3(0, extractorPrefab.GetComponent<OrbitalDrop>().speed * dropTime, 0), Quaternion.identity);
        extractor.transform.SetParent(transform);
        OrbitalDrop pod = extractor.GetComponent<OrbitalDrop>();
        pod.PromptHeight(transform.position.y);
        pod.landed += HandlePhaseChange;
    }

    private void StartExtraction()
    {
        lifecycle.enabled = true;
    }

    private void Strike()
    {
        //play animation
        int c = Mathf.RoundToInt(Math.Min(curValue, maxValue * strikeInterval / extractionTime));
        TakeDamage(this, c);
        scoreManager.AddCrystal(c);
    }

    public void TakeDamage(object sender, int damage)
    {
        curValue -= damage * damageValueMultiplier;
        if (curValue <= 0) HandlePhaseChange(this, EventArgs.Empty);
    }
}