using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pack", menuName = "ScriptableObjects/Pack", order = 2)]
public class PackSO : ResourceSO
{
    public Sprite Art;
    public string Name;

    public PackSO(string Name, Sprite Art)
    {
        this.Name = Name;
        this.Art = Art;
    }

    public static PackSO LoadFromFile(string fileName)
    {
        var pack = Resources.Load<PackSO>("Packs/" + fileName);
        return pack;
    }
}
