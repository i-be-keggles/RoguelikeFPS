using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerAbility : ScriptableObject
{
    public string name;
    [TextArea] public string description;
    public float cooldown;
    [Min(1)]public int charges;
    [Tooltip("Logic on key press and release")]
    public bool twoStage;


    public virtual IEnumerator Trigger(PlayerAbilityHandler handler)
    {
        yield return null;
        Debug.Log("How the fuyck");
    }

    public virtual IEnumerator SecondaryTrigger(PlayerAbilityHandler handler)
    {
        yield return null;
        Debug.Log("How the fuyck er");
    }
}
