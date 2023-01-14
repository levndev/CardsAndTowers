using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinPanelProvider : MonoBehaviour
{
    public WinPanelSO winPanel;

    public TextMeshProUGUI Pack;
    public TextMeshProUGUI Card;
    public TextMeshProUGUI GoldAmount;

    private void Awake()
    {
        winPanel.Pack = Pack;
        winPanel.Card = Card;
        winPanel.GoldAmount = GoldAmount;

    }
}
