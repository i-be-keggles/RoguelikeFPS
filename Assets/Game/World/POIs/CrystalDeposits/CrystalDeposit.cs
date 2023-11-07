using UnityEngine;
using System;
using UnityEngine.Animations;

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

    public int phase = 0; //0 = idle, 1 = waiting for extract, 2 = extracting, 3 = depleted

    private Animator anim;


    private void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();

        //interactable = GetComponent<Interactable>();
        //interactable.interactedWith += HandlePhaseChange;
        lifecycle = GetComponent<EnemyTarget>();
        lifecycle.onTakeDamage += TakeDamage;
        lifecycle.enabled = false;

        curValue = maxValue;
    }

    private void Update()
    {
        if(phase == 1)
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
        if (phase == 0)
        {
            interactable.active = false;
            interactable.enabled = false;

            StartExtraction();
        }
        else if(phase == 1)
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
        anim = pod.GetComponentInChildren<Animator>();
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
        anim.SetTrigger("Strike");
    }

    public void TakeDamage(object sender, int damage)
    {
        curValue -= damage * damageValueMultiplier;
        if (curValue <= 0) HandlePhaseChange(this, EventArgs.Empty);
    }

    public void ExtractorLanded(Interactable e)
    {
        interactable = e;
        interactable.interactedWith += HandlePhaseChange;
        anim = e.GetComponent<Animator>();
    }
}