using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField]
    private double current;
    [SerializeField]
    private double max;
    public GameObject healthbar;
    private float healthbarWidth;
    private bool alive = true;
    public bool DestroyAutomatically = true;
    public double Max
    {
        get => max;
        set
        {
            max = value;
            current = value;
        }
    }
    public double Current
    {
        get => current;
        private set
        {
            current = value;
            if (healthbar != null)
            {
                var x = (float)(healthbarWidth * current / max);
                if (float.IsNormal(x))
                {
                    healthbar.transform.localScale = new Vector3(
                        x,
                        healthbar.transform.localScale.y,
                        healthbar.transform.localScale.z);
                }
            }
        }
    }

    public event Action Death;
    public event Action Damaged;

    void Start()
    {
        current = max;
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
