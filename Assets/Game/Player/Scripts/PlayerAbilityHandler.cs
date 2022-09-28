using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilityHandler : MonoBehaviour
{

    public Transform cam;

    public float cooldownTimer;
    public int curCharges;
    public PlayerAbility ability;

    private bool triggeredFirstStage;


    private void Start()
    {
        curCharges = ability.charges;
    }

    void Update()
    {
        if(cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0 && curCharges < ability.charges) curCharges++; 
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if(curCharges > 0)
            {
                triggeredFirstStage = true;
                StartCoroutine(ability.Trigger(this));
                if (!ability.twoStage)
                {
                    curCharges--;
                    cooldownTimer = ability.cooldown;
                }
            }
            else
            {
                //player feedback (audio & visual)
            }
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            if (ability.twoStage && triggeredFirstStage)
            {
                StartCoroutine(ability.SecondaryTrigger(this));
                curCharges--;
                cooldownTimer = ability.cooldown;
            }
            triggeredFirstStage = false;
        }
    }
}
