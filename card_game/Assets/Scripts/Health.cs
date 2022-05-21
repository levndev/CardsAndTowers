using System;

public class Health
{
    public double Max { get; private set; }
    public double Current { get; private set; }

    public event Action Death;

    public Health(double maxHealth)
    {
        Max = maxHealth;
        Current = Max;
    }

    public void TakeDamage(double damage)
    {
        Current -= damage;
        if (Current <= 0)
            Death?.Invoke();
    }
}
