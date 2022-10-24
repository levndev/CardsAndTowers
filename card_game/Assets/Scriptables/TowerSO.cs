using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TowerSO : ScriptableObject
{
    [SerializeField] protected TowerBaseSO towerBase;

    public virtual void Init(TowerController sender)
    {
        towerBase.InitializeTower(sender.gameObject);
    }
    public abstract void OnUpdate(TowerController sender);
    public abstract void OnFixedUpdate(TowerController sender);
}
