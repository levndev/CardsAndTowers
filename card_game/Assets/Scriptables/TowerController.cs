using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    [SerializeField] private TowerSO Tower;
    public TowerState State;

    private void Start()
    {
    }
    void Update()
    {
        Tower.OnUpdate(this);
    }

    private void FixedUpdate()
    {
        Tower.OnFixedUpdate(this);
    }

    public void SetTower(TowerSO towerSO)
    {
        Tower = towerSO;
        Tower.Init(this);
    }
}
