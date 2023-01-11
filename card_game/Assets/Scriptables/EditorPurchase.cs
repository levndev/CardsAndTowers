using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "YandexGame/Editor purchase")]
public class EditorPurchase : ScriptableObject
{
    public string Id;
    public string Title;
    public string Description;
    public string Price;
    public string IconURL;
}
