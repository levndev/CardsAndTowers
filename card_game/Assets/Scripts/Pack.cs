using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pack", menuName = "ScriptableObjects/Pack", order = 2)]
public class Pack : ScriptableObject
{
    public Sprite Art;
    public string Name;

    public Pack(string Name, Sprite Art)
    {
        this.Name = Name;
        this.Art = Art;
    }

    public static Pack LoadFromFile(string fileName)
    {
        var pack = Resources.Load<Pack>("Packs/" + fileName);
        return pack;
    }
}
