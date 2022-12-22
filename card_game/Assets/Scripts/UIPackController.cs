using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPackController : MonoBehaviour
{
    public Pack pack;
    public PackSlot packSlot;
    public TextMeshProUGUI Name;
    public Image ArtImage;
    public Button PackButton;
    public PacksMenu packsScreen;
    //public GameObject PackPrefab;
    //public GameObject PackSlotPrefab;

    void Start()
    {
        PackButton.onClick.AddListener(onButtonClick);
    }

    public Card[] GenerateCards()
    {
        var cards = new Card[6];
        for(var i = 0; i < 5; i += 2)
        {
            cards[i] = Card.LoadfromFile("Basic");
            cards[i + 1] = Card.LoadfromFile("Sniper");
        }
        return cards;
    }

    public void onButtonClick()
    {
        packsScreen.onPackClick(this);
    }

    public void SetFromPack(Pack pack)
    {
        this.ArtImage.sprite = pack.Art;
        this.pack = pack;
        this.Name.text = pack.Name;
    }
}
