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
    public Type type;
    public Card(int Cost, string Name, Sprite Art, Type type)
    {
        this.Cost = Cost;
        this.Name = Name;
        this.Art = Art;
        this.type = type;
    }

    public static Card LoadfromFile(string filename)
    {
        var card = Resources.Load<Card>("Cards/" + filename);
        return card;
    }

    public enum Type
    {
        Tower,
        Spell
    }
}
