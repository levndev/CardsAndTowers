using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPackController : MonoBehaviour
{
    public PackSO pack;
    public PackSlot packSlot;
    public TextMeshProUGUI Name;
    public Image ArtImage;
    //public Button PackButton;
    public PacksMenu packsScreen;
    //public GameObject PackPrefab;
    //public GameObject PackSlotPrefab;

    void Start()
    {
        //PackButton.onClick.AddListener(onButtonClick);
    }

    public CardSO[] GenerateCards()
    {
        var openedCards = new CardSO[6];
        var allCards = SaveDataManager.Instance.AllCards.Values;
        
        for(var i = 0; i < 6; i ++)
        {
            
            openedCards[i] = allCards.ChooseRandom(card=>1);
        }
        return openedCards;
    }

    public void onButtonClick()
    {
        packsScreen.onPackClick(this);
    }

    public void SetFromPack(PackSO pack)
    {
        this.ArtImage.sprite = pack.Art;
        this.pack = pack;
        this.Name.text = pack.Name;
    }
}
