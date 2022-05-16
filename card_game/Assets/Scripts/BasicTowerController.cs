using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTowerController : MonoBehaviour
{
    public double AttackRange;
    public double AttackDamage;
    public double HealthPoints;
    public GameObject Turret;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Turret.transform.Rotate(new Vector3(0, 0, 1), 1);
    }
}
