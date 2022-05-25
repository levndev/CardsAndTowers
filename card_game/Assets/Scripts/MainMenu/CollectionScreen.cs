using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CollectionScreen : MenuScreen
{
    public UnityEngine.UI.Button TestButton;
    public GameObject CardSlotPrefab;
    public Card[] AllCards;
    //public ScrollView CardsScrollView;
    public GameObject ScrollViewContent;
    public List<GameObject> CardSlots = new List<GameObject>();
    public GameObject CardPrefab;
    public GameObject ContentLayout;
    private int CardsInRow = 5;

    void Start()
    {
        TestButton.onClick.AddListener(onTestButtonClick);
        AllCards = Resources.LoadAll<Card>("Cards");

        for (var i = 0; i < AllCards.Length / CardsInRow + 1; i++)
        {
            for(var j = 0; j < CardsInRow+1; j++)
            {
                var cardSlot = Instantiate(CardSlotPrefab);
                //cardSlot.transform.SetParent((Transform)CardsScrollView.contentContainer.transform);
                cardSlot.transform.SetParent(ScrollViewContent.transform);
                CardSlots.Add(cardSlot);
                cardSlot.transform.localScale = Vector2.one;
                //float d = 10;//длина промежутка между картами в горизонтали
                //float j = 10; //длина промежутка между слотами в вертикали
                //var cardSize = cardSlot.GetComponent<RectTransform>();
                //cardSlot.transform.localPosition.Set()
                //cardSlot.transform

            }

        }
        var gridLayout = ContentLayout.GetComponent<GridLayoutGroup>();

        gridLayout.spacing.Set((2000 - 5 * 291) / 6, (2000 - 5 * 239) / 5); // 5 карт по 291 по x = 6 spaces ,  –јЌƒќћЌќ≈ число по y 

        for (var i = 0; i < AllCards.Length; i++)
        {
            var card = AllCards[i];

            var uiCard = Instantiate(CardPrefab, CardSlots[i].transform);
            var uiCardController = uiCard.GetComponent<UICardController>();
            uiCardController.InHand = false;
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


}
