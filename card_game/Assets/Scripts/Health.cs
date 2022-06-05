using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public double current;
    public GameObject healthbar;
    private float healthbarWidth;
    public double Max;
    public double Current
    {
        get => current;
        private set
        {
            current = value;
            if (healthbar != null)
            {
                healthbar.transform.localScale = new Vector3(
                    (float)(healthbarWidth * current / Max),
                    healthbar.transform.localScale.y,
                    healthbar.transform.localScale.z);
            }
        }
    }

    public event Action Death;

    void Start()
    {
        current = Max;
        if (healthbar != null)
            healthbarWidth = healthbar.transform.localScale.x;
    }

    public double TakeDamage(double damage)
    {
        Current -= damage;
        if (Current <= 0)
        {
            Death?.Invoke();
            Destroy(gameObject);
        }
        return Current;
    }
}
