using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class UICardController : MonoBehaviour
{
    public Card card;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI nameText;
    public Image artImage;
    public int HandIndex;
    public Button Button;
    public GameManager GameManager;
    public Deck deck;
    public CardState CurrentCardState;
    public CollectionScreen collectionScreen;
    public enum CardState
    {
        inHand = 0,
        inCollection = 1,
        inDeck = 2,
    };

    void Awake()
    {
        //costText = transform.Find("CostText").GetComponent<TextMeshProUGUI>();
        //nameText = transform.Find("NameText").GetComponent<TextMeshProUGUI>();
        //artImage = transform.Find("Art").GetComponent<UnityEngine.UI.Image>();
    }

    void Start()
    {
        
        Button.onClick.AddListener(OnClick);
        //SetFromCard(card);
    }

    public void OnClick()
    {
        if (CurrentCardState == CardState.inHand)
        {
            if (GameManager == null)
            {
                GameManager = GameManager.Instance;
            }
            GameManager.CardClick(HandIndex);
        }
        if(CurrentCardState == CardState.inCollection || CurrentCardState == CardState.inDeck)
        {
            //if(deck == null)
            //{
            //    Debug.Log("no deck attached to card");
            //}
            //else
            //{
            //    if (deck.CanAddToDeck(card))
            //    {
            //        deck.AddToList(card);
            //        collectionScreen.onCardAddedToDeck(card);
            //    }
            //}
            collectionScreen.onCardClick(this);
        }
        //if(CurrentCardState == CardState.inDeck)
        //{
        //    deck.RemoveFromDeck(card);


        //}
    }

    public void onHold()
    {

    }

    public void SetFromCard(Card card)
    {
        this.card = card;
        costText.text = card.Cost.ToString();
        nameText.text = card.Name;
        artImage.sprite = card.Art;
    }
}
