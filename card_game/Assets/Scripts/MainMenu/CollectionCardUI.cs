using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectionCardUI : MonoBehaviour
{
    public Card card;
    public Button CardButton;
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI CostText;
    public Image ArtImage;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onCardClick()
    {

    }

    public void SetFromCard(Card card)
    {
        this.card = card;
        CostText.text = card.Cost.ToString();
        NameText.text = card.Name;
        ArtImage.sprite = card.Art;
    }
}
