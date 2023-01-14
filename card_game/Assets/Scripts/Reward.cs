using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reward : MonoBehaviour 
{
    public List<PackSaveData> Packs;
    public List <CardSaveData> Cards;
    public uint Gold;


    public static Action<object, List<PackSaveData>,List<CardSaveData>, uint > RewardClaimed;

    public void GetReward()
    {
        RewardClaimed?.Invoke(this, Packs, Cards, Gold);
       
    }
}

