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
    public CollectionMenu collectionScreen;
    public PacksMenu packsScreen;

    public enum CardState
    {
        inHand = 0,
        inCollection = 1,
        inDeck = 2,
        inPack = 3,
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
        SetFromCard(card);
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
            collectionScreen.onCardClick(this);
        }
        if(CurrentCardState == CardState.inPack)
        {
            packsScreen.onCardClick(this);
        }
    }

    public void OnHold()
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
