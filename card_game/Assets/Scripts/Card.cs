using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Card", order = 1)]
public class Card : ScriptableObject
{
    public int Cost;
    public string Name;
    public Sprite Art;
    public GameObject SpawnedObject;

    public Card(int Cost, string Name, Sprite Art)
    {
        this.Cost = Cost;
        this.Name = Name;
        this.Art = Art;
    }
}
