using UnityEngine;
using System;
using UnityEngine.Animations;

public class CrystalDeposit : MonoBehaviour
{
    private Interactable interactable;

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

    private CameraShake shake;


    private void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        shake = scoreManager.uiManager.mouseLook.GetComponent<CameraShake>();

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
        shake.Shake(transform.position, 20f, 0.2f, 0.1f);
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
        anim = e.GetComponentInChildren<Animator>();
    }
}