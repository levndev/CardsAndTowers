using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CollectionScreen : MenuScreen
{
    public UnityEngine.UI.Button TestButton;
    public Card[] AllCards;
    public Deck CurrentDeck;

    public GameObject CardsViewport;
    public GameObject CardsScrollViewContent;

    public GameObject DeckViewport;
    public GameObject DeckScrollViewContent;

    public List<GameObject> CardSlots = new List<GameObject>();
    public GameObject CollectionCardPrefab;
    public GameObject CollectionCardSlotPrefab;

    public List<GameObject> DeckCardSlots = new List<GameObject>();
    public GameObject DeckCardPrefab;
    public GameObject DeckCardSlotPrefab;
    private int CardsInRow = 5;
    private int verticalSpacing = 100;

    void Start()
    {
        TestButton.onClick.AddListener(onTestButtonClick);
        AllCards = Resources.LoadAll<Card>("Cards");

        CurrentDeck = new Deck();

        for (var i = 0; i < AllCards.Length / CardsInRow + 1; i++)
        {
            for(var j = 0; j < CardsInRow+1; j++)
            {
                var cardSlot = Instantiate(CollectionCardSlotPrefab);
                cardSlot.transform.SetParent(CardsScrollViewContent.transform);
                CardSlots.Add(cardSlot);
                cardSlot.transform.localScale = Vector2.one;
            }
        }
        var viewportRectTransform = CardsViewport.GetComponent<RectTransform>();
        var viewportHeight = viewportRectTransform.rect.height;
        var viewportWidth = viewportRectTransform.rect.width;

        var gridLayout = CardsScrollViewContent.GetComponent<GridLayoutGroup>();
        gridLayout.spacing.Set((viewportHeight - 5 * 291) / 6, verticalSpacing); // 5 cards with 291 width  x = 6 spaces ,  ÐÀÍÄÎÌÍÎÅ ÷èñëî ïî y 

        for (var i = 0; i < AllCards.Length; i++)
        {
            var card = AllCards[i];

            var uiCard = Instantiate(CollectionCardPrefab, CardSlots[i].transform);
            var uiCardController = uiCard.GetComponent<UICardController>();
            uiCardController.CurrentCardState = UICardController.CardState.inCollection;
            uiCardController.collectionScreen = this;
            uiCardController.deck = CurrentDeck;
            uiCardController.SetFromCard(card);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void onTestButtonClick()
    {
        Debug.Log("Nice Test!");
    }

    public void onCardAddedToDeck(Card card)
    {
        var deckCardSlot = Instantiate(DeckCardSlotPrefab);
        deckCardSlot.transform.SetParent(DeckScrollViewContent.transform);
        DeckCardSlots.Add(deckCardSlot);
        deckCardSlot.transform.localScale = Vector2.one;

        var uiDeckCard = Instantiate(DeckCardPrefab, deckCardSlot.transform);
        var uiCardController = uiDeckCard.GetComponent<UICardController>();
        uiCardController.CurrentCardState = UICardController.CardState.inDeck;
        uiCardController.deck = CurrentDeck;
        uiCardController.SetFromCard(card);
    }
}
