using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PackSlot : MonoBehaviour
{
    public int Amount;
    public TextMeshProUGUI AmountText;
    public Pack pack;

    void Start()
    {
        
    }

    public void IncreaseAmount()
    {
        Amount++;
        UpdateDisplayedAmount();
    }

    public void DecreseAmount()
    {
        Amount--;
        UpdateDisplayedAmount();
    }
    
    public void UpdateDisplayedAmount()
    {
        AmountText.text = Amount.ToString();
    }
}
