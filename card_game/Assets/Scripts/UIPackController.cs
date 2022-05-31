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
    public PacksScreen packsScreen;
    //public GameObject PackPrefab;
    //public GameObject PackSlotPrefab;

    void Start()
    {
        PackButton.onClick.AddListener(onButtonClick);


    }

    public Card[] GenerateCards()
    {
        return null;
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
