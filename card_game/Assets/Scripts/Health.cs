using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public double current;
    public GameObject healthbar;
    private float healthbarWidth;
    public double Max;
    private bool alive = true;
    public bool DestroyAutomatically = true;
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
    public event Action Damaged;

    void Start()
    {
        current = Max;
        if (healthbar != null)
            healthbarWidth = healthbar.transform.localScale.x;
    }

    public double TakeDamage(double damage)
    {
        if (!alive)
            return Current;
        Current -= damage;
        if (Current <= 0)
        {
            alive = false;
            Death?.Invoke();
            if (DestroyAutomatically)
                Destroy(gameObject);
        }
        else
        {
            Damaged?.Invoke();
        }
        return Current;
    }
}
