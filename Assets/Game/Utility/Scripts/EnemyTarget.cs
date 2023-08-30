using UnityEngine;
using System;

public class EnemyTarget : MonoBehaviour
{
    [Min(1)] public float priority;

    public event EventHandler<float> onTakeDamage;

    public event EventHandler died;

    public bool active;

    private void Start()
    {
        active = true;
    }

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
        died?.Invoke(this, EventArgs.Empty);
        active = false;
        enabled = false;
    }

    public void Deactivate()
    {
        active = false;
        Die();
    }
}