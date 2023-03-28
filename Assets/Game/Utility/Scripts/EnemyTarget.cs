using UnityEngine;
using System.Collections;
using System.Generic;
using System;

public class EnemyTarget : MonoBehaviour
{
    public float priority;

    public event EventHandler<float> onTakeDamage;

    public event EventHandler died;

    public void TakeDamage(float damage)
    {
        onTakeDamage?.Invoke(this, damage);
    }

    public float GetPriority()
    {
        return enabled ? priority : 0f;
    }

    public void Die()
    {
        died?.Invoke(self, EventArgs.Empty);
    }
}