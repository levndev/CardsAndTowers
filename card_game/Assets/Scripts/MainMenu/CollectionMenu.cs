using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using YG;

public class CollectionMenu : MonoBehaviour
{
    public Deck WorkingDeck;

    public List<UICardController> DeckUICards = new List<UICardController>();
    public List<UICardController> CollectionUICards = new List<UICardController>();

    public GameObject CardsScrollViewContent;

    public GameObject DeckScrollViewContent;


    public GameObject CollectionCardPrefab;
    public GameObject CollectionCardSlotPrefab;


    public GameObject DeckCardPrefab;
    public GameObject DeckCardSlotPrefab;

    //private List<Deck> UserDecks;
    public GameObject deckSelectionDropdown;

    public TMP_InputField deckNameInputField;

    public UnityEngine.UI.Button ConfirmChangesButton;

    public static Action<object, DeckEventArgs> DeckAction;

    void Start()
    {
        //FillCollectionWithUserCards();

    }

    private void OnEnable()
    {
        FillCollectionWithUserCards();
        FillDropdownWithUserDecks();
        ShowDeckByName(SaveDataManager.Instance.CurrentDeck);
    }


    private void OnDisable()
    {
        //Deck.SaveDeck(WorkingDeck);
        //Clear everything
        ClearDeckUI();
        ClearCollectionUI();
        ClearDeckSelectionDropdown();
    }

    private void Reload()
    {
        ClearDeckUI();
        ClearCollectionUI();
        ClearDeckSelectionDropdown();
        FillCollectionWithUserCards();
        FillDropdownWithUserDecks();
        ShowDeckByName(SaveDataManager.Instance.CurrentDeck);
    }


    private void FillDropdownWithUserDecks()
    {
        var dropdown = deckSelectionDropdown.GetComponent<TMP_Dropdown>();
        var list = new List<TMP_Dropdown.OptionData>();
        foreach ((var deckName, var deck) in SaveDataManager.Instance.UserDecks)
        {
            list.Add(new TMP_Dropdown.OptionData(deckName));
        }
        list.Add(new TMP_Dropdown.OptionData("Create New Deck"));
        dropdown.AddOptions(list);
        dropdown.SetValueWithoutNotify(list.FindIndex(data => data.text == SaveDataManager.Instance.CurrentDeck));
        

    }

    private void FillCollectionWithUserCards()
    {
        ClearCollectionUI();

        var cards = SaveDataManager.Instance.UserCards;
        foreach ((var cardSO, var cardData) in cards)
        {
            //Change CardSlotPrefab to have level and amount
            //then here set data from cardData
            var cardSlot = Instantiate(CollectionCardSlotPrefab);
            cardSlot.transform.SetParent(CardsScrollViewContent.transform);
            cardSlot.transform.localScale = Vector2.one;



            var uiCard = Instantiate(CollectionCardPrefab, cardSlot.transform);
            var uiCardController = uiCard.GetComponent<UICardController>();
            uiCardController.CurrentCardState = UICardController.CardState.inCollection;
            uiCardController.collectionScreen = this;
            uiCardController.SetFromCard(cardSO);
            CollectionUICards.Add(uiCardController);

        }
    }


    private void ClearCollectionUI()
    {
        for (var i = CollectionUICards.Count - 1; i >= 0; i--)
        {
            Destroy(CollectionUICards[i].transform.parent.gameObject);
        }
        CollectionUICards.Clear();
    }


    private void AddCardToDeckUI(CardSO card)
    {
        var deckCardSlot = Instantiate(DeckCardSlotPrefab);
        deckCardSlot.transform.SetParent(DeckScrollViewContent.transform);
        //DeckCardSlots.Add(deckCardSlot);
        deckCardSlot.transform.localScale = Vector2.one;

        var uiDeckCard = Instantiate(DeckCardPrefab, deckCardSlot.transform);
        var uiCardController = uiDeckCard.GetComponent<UICardController>();
        DeckUICards.Add(uiCardController);
        uiCardController.CurrentCardState = UICardController.CardState.inDeck;
        //uiCardController.deck = CurrentDeck;
        uiCardController.collectionScreen = this;
        uiCardController.SetFromCard(card);
    }

    public void OnCardClick(UICardController sender)
    {
        if (sender.CurrentCardState == UICardController.CardState.inCollection)
        {
            if (WorkingDeck.CanAddToDeck(sender.card))
            {
                WorkingDeck.AddToList(sender.card);
                AddCardToDeckUI(sender.card);
            }
        }
        if (sender.CurrentCardState == UICardController.CardState.inDeck)
        {
            WorkingDeck.RemoveFromDeck(sender.card);
            RemoveCardFromDeckUI(sender);
        }
        ConfirmChangesButton.gameObject.SetActive(true);
    }

    public void OnConfirmChangesButtonClick()
    {
        ConfirmChangesButton.gameObject.SetActive(false);
        var args = new DeckEventArgs(WorkingDeck.Name, WorkingDeck.Clone(), global::DeckAction.Changed);
        DeckAction?.Invoke(this, args);
        //show message that save was successful 
    }


    

    private void ClearDeckSelectionDropdown()
    {
        var dropDown = deckSelectionDropdown.GetComponent<TMP_Dropdown>();
        dropDown.ClearOptions();
    }

    public void OnDeckSelectionValueChange(int id)
    {
        ClearDeckUI();
        if (id != deckSelectionDropdown.GetComponent<TMP_Dropdown>().options.Count - 1)
        {
            ShowSelectedDeck();

        }
        else
        {
            WorkingDeck = new Deck(null, new());

            deckNameInputField.text = "Enter deck name here";
        }


        //var dropdown = deckSelectionDropdown.GetComponent<TMP_Dropdown>();
        //ShowDeckByName(dropdown.options[id].text); 
    }

    public void OnDeckNameEndEdit(string newName)
    {
        if (SaveDataManager.Instance.UserDecks.ContainsKey(newName))
        {
            Debug.Log("Deck with this name already exists. Enter differnt name");
            deckNameInputField.text = "Invalid name";
            return;
        }

        WorkingDeck.Name = newName;
        DeckEventArgs args;

        if (WorkingDeck.Name == null)
            args = new DeckEventArgs(WorkingDeck.Name, WorkingDeck.Clone(), global::DeckAction.Created);
        else
            args = new DeckEventArgs(WorkingDeck.Name, WorkingDeck.Clone(), global::DeckAction.Renamed);

        DeckAction?.Invoke(this, args);
        Reload();
    }



    //public void ShowDeckByID(int id)
    //{
    //    var dropdown = deckSelectionDropdown.GetComponent<TMP_Dropdown>();

    //    ShowDeckByName(dropdown.options[id].text);
    //}


    private void ShowSelectedDeck()
    {
        var dropdown = deckSelectionDropdown.GetComponent<TMP_Dropdown>();
        ShowDeckByName(dropdown.options[dropdown.value].text);
    }


    public void ShowDeckByName(string name)
    {
        ClearDeckUI();
        var deck = SaveDataManager.Instance.GetDeck(name);
        if (deck == null)
        {
            throw new Exception("You dont have this deck");
        }

        foreach (var cardSO in deck.GetDeckList())
        {
            AddCardToDeckUI(cardSO);
        }
        deckNameInputField.text = name;
        WorkingDeck = deck;
    }

    private void ClearDeckUI()
    {
        for (int i = DeckUICards.Count - 1; i >= 0; i--)
        {
            Destroy(DeckUICards[i].transform.parent.gameObject);
        }
        DeckUICards.Clear();
    }

    private void RemoveCardFromDeckUI(UICardController card)
    {
        DeckUICards.Remove(card);
        Destroy(card.transform.parent.gameObject);
    }
}
