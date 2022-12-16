using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [Tooltip("Higher = more enemies spawn relative to other spawn points.")]
    [field: SerializeField] public int Weight { get; private set; }
}
