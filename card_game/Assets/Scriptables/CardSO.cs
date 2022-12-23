using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Card", order = 1)]
public class CardSO : ResourceSO
{
    public int Cost;
    public string Name;
    public Sprite Art;
    public TowerSO Tower;
    public Type type;
    public CardSO(int Cost, string Name, Sprite Art, Type type)
    {
        this.Cost = Cost;
        this.Name = Name;
        this.Art = Art;
        this.type = type;
    }

    public static CardSO LoadfromFile(string filename)
    {
        var card = Resources.Load<CardSO>("Cards/" + filename);
        return card;
    }

    public enum Type
    {
        Tower,
        Spell
    }
}
