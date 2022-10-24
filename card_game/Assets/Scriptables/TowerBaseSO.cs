using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardsAndTowers/Tower Base")]
public class TowerBaseSO : ScriptableObject
{
    [SerializeField] public Vector2Int size;
    [SerializeField] private GameObject entityShadowPrefab;

    public void InitializeTower(GameObject tower)
    {
        //var sizeData = tower.AddComponent<SizeData>();
        //sizeData.Size = size;
        var collider = tower.AddComponent<BoxCollider2D>();
        collider.size = size;
        var shadow = Instantiate(entityShadowPrefab);
        shadow.transform.parent = tower.transform;
    }
}
