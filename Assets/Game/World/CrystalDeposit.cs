using UnityEngine;
using System.Collections;
using System.Generic;
using System;

public class CrystalDeposit : MonoBehaviour
{
    private Interacteable interacteable;

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

    public EnemyTarget lifecycle;

    public int phase = 0; //0 = idle, 1 = pod dropping, 2 = waiting for extract, 3 = extracting, 4 = depleted


    private void Start()
    {
        interacteable = GetComponent<Interacteable>();
        interacteable.OnTriggered += HandlePhaseChange;
        lifecycle.onTakeDamage += TakeDamage;
        lifecycle.enabled = false;
    }

    private void Update()
    {
        if(phase == 3)
        {
            timeToStrike -= time.DeltaTime;
            if(timeToStrike <= 0)
            {
                timeToStrike = strikeInterval;
                Strike();
            }

            if (curValue <= 0)
            {
                HandlePhaseChange();
            }
        }
    }

    private void HandlePhaseChange(object sender, EventArgs e)
    {
        if(phase == 0)
        {
            interacteable.enabled = false;

            CallPod();
        }
        else if(phase == 1)
        {
            interacteable.enabled = true;
            interacteable.promptText = "Start extraction";
        }
        else if(phase == 2)
        {
            StartExtraction();
        }
        else if(phase == 3)
        {
            lifecycle.Die();
        }

        phase++;
    }

    private void CallPod()
    {
        extractor = Instantiate(extractorPrefab, transform);
        OrbitalDrop pod = extractor.GetComponent<OrbitalDrop>();
        extractor.transform.position += new Vector3(0, pod.speed * dropTime, 0);
        pod.landed += HandlePhaseChange;
    }

    private void StartExtraction()
    {
        lifecycle.enabled = true;
    }

    private void Strike()
    {
        //play animation
        curValue -= maxValue * strikeInterval / extractionTime;
    }

    public void TakeDamage(object sender, float damage)
    {
        curValue -= damage * damageValueMultiplier;
    }
}