using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityTracker : MonoBehaviour
{
    [Flags]
    public enum TargetType
    {
        Enemy, 
        Tower,
        Wall
    }
    private List<GameObject> entitiesInRange = new List<GameObject>();
    public TargetType TypeFilter;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var entity = other.gameObject;
        if (TypeFilter.HasFlag(TargetType.Enemy) && entity.GetComponent<BasicEnemy>() ||
            TypeFilter.HasFlag(TargetType.Tower) && entity.GetComponent<TowerController>())
        {
            entitiesInRange.Add(entity);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var entity = other.gameObject;
        if (entity != null)
            entitiesInRange.Remove(entity);
    }

    public GameObject GetClosestEntity()
    {
        if (entitiesInRange.Count == 0)
            return null;
        var minDistance = float.MaxValue;
        GameObject closestEntity = null;
        foreach (var entity in entitiesInRange)
        {
            if (entity == null)
                continue;
            var distance = Vector3.Distance(transform.position, entity.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestEntity = entity;
            }
        }
        return closestEntity;
    }

    public IEnumerable<GameObject> GetEntitiesInRange()
    {
        return entitiesInRange;
    }
}
