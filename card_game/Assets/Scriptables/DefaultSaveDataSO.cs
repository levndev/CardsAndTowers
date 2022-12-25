using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardsAndTowers/Default Save Data")]
public class DefaultSaveDataSO : ScriptableObject
{
    public List<CardSO> StarterCards;
    public List<CardSO> StarterDeck;
    public List<PackSO> StarterPacks;
}
