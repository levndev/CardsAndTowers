using System;
using UnityEngine;

public class Health
{
    private double current;
    private GameObject healthbar;
    private readonly float healthbarWidth;

    public double Max { get; private set; }
    public double Current
    {
        get => current;
        private set
        {
            current = value;
            healthbar.transform.localScale = new Vector3(
                (float)(healthbarWidth * current / Max),
                healthbar.transform.localScale.y,
                healthbar.transform.localScale.z);
        }
    }

    public event Action Death;

    public Health(double maxHealth, GameObject healthbar)
    {
        Max = maxHealth;
        current = Max;
        this.healthbar = healthbar;
        healthbarWidth = healthbar.transform.localScale.x;
    }

    public void TakeDamage(double damage)
    {
        Current -= damage;
        if (Current <= 0)
            Death?.Invoke();
    }
}
