using UnityEngine;

[RequireComponent(typeof(Health))]
public class HealthBurn : MonoBehaviour
{
    [SerializeField] private float lifeDuration;

    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void Update()
    {
        health.TakeDamage(health.Max / lifeDuration * Time.deltaTime);
    }
}
