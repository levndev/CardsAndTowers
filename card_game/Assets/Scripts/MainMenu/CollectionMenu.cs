using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
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
    public TextMeshProUGUI deckNameFieldPlaceholder;

    public UnityEngine.UI.Button ConfirmChangesButton;
    public UnityEngine.UI.Button DeleteDeckButton;

    public static Action<object, DeckEventArgs> DeckAction;


    public LocalizedString deckNameEnter;
    public LocalizedString deckNameInvalid;
    public LocalizedString deckCreateNew;

    private string localDeckNameEnter;
    private string localDeckNameInvalid;
    private string localDeckCreateNew;

    void Start()
    {
        //FillCollectionWithUserCards();

    }

    private void OnEnable()
    {
        FillCollectionWithUserCards();
        FillDropdownWithUserDecks();
        ShowDeckByName(SaveDataManager.Instance.CurrentDeck);
        SignUpForLocalStrings(true);
    }


    private void OnDisable()
    {
        //Deck.SaveDeck(WorkingDeck);
        //Clear everything
        ClearDeckUI();
        ClearCollectionUI();
        ClearDeckSelectionDropdown();
        SignUpForLocalStrings(false);
    }

    private void SignUpForLocalStrings(bool flag)
    {
        if (flag)
        {
            deckNameEnter.StringChanged += ChangeDeckNameEnterText;
            deckCreateNew.StringChanged += ChangeDeckCreateNewText;
            deckNameInvalid.StringChanged += ChangeDeckNameInvalidText;
        }
        else
        {
            deckNameEnter.StringChanged -= ChangeDeckNameEnterText;
            deckCreateNew.StringChanged -= ChangeDeckCreateNewText;
            deckNameInvalid.StringChanged -= ChangeDeckNameInvalidText;
        }
    }

    private void Reload()
    {
        ClearDeckUI();
        ClearCollectionUI();
        ClearDeckSelectionDropdown();
        FillCollectionWithUserCards();
        FillDropdownWithUserDecks();
        //ShowDeckByName(SaveDataManager.Instance.CurrentDeck);
        ShowSelectedDeck();
    }


    private void FillDropdownWithUserDecks()
    {
        var dropdown = deckSelectionDropdown.GetComponent<TMP_Dropdown>();

        var list = new List<TMP_Dropdown.OptionData>();
        foreach ((var deckName, var deck) in SaveDataManager.Instance.UserDecks)
        {
            list.Add(new TMP_Dropdown.OptionData(deckName));
        }
        list.Add(new TMP_Dropdown.OptionData(localDeckCreateNew));
        dropdown.AddOptions(list);

        if (SaveDataManager.Instance.CurrentDeck != null)
        {
            //dropdown.value = list.FindIndex(data => data.text == SaveDataManager.Instance.CurrentDeck);
            dropdown.SetValueWithoutNotify(list.FindIndex(data => data.text == SaveDataManager.Instance.CurrentDeck));
            OnDeckSelectionValueChange(dropdown.value);
        }
        else
        {
            //dropdown.value = 0;
            dropdown.SetValueWithoutNotify(0);
            OnDeckSelectionValueChange(0);
        }


    }

    private void FillCollectionWithUserCards()
    {
        ClearCollectionUI();

        var cards = new List<(CardSO, CardSaveData)>();
        foreach ((var cardSO, var cardData) in SaveDataManager.Instance.UserCards)
        {
            cards.Add((cardSO, cardData));
        }
        cards.Sort((first, second) => first.Item1.Cost - second.Item1.Cost);
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
        if (sender.CurrentCardState == UICardController.CardState.inCollection && WorkingDeck.Name != null)
        {
            if (WorkingDeck.CanAddToDeck(sender.card))
            {
                WorkingDeck.AddToList(sender.card);
                ClearDeckUI();

                ShowDeckUI(WorkingDeck);
                ConfirmChangesButton.gameObject.SetActive(true);

            }
        }
        if (sender.CurrentCardState == UICardController.CardState.inDeck)
        {
            WorkingDeck.RemoveFromDeck(sender.card);
            RemoveCardFromDeckUI(sender);
            ConfirmChangesButton.gameObject.SetActive(true);

        }
    }

    public void OnConfirmChangesButtonClick()
    {
        ConfirmChangesButton.gameObject.SetActive(false);
        var args = new DeckEventArgs(WorkingDeck.Name, WorkingDeck.Clone(), global::DeckAction.Changed);
        DeckAction?.Invoke(this, args);
        //show message that save was successful 
    }

    public void OnConfirmRemovalButtonClick()
    {
        var args = new DeckEventArgs(WorkingDeck.Name, WorkingDeck.Clone(), global::DeckAction.Removed);
        WorkingDeck = null;
        ConfirmChangesButton.gameObject.SetActive(false) ;
        DeckAction?.Invoke(this, args);
        Reload();
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
            DeleteDeckButton.gameObject.SetActive(true);
        }
        else
        {
            WorkingDeck = new Deck(null, new());

            deckNameInputField.text = "";
            deckNameFieldPlaceholder.text = localDeckNameEnter;
            DeleteDeckButton.gameObject.SetActive(false);
            ConfirmChangesButton.gameObject.SetActive(false);
        }
    }

    public void OnDeckNameEndEdit(string newName)
    {

        if (WorkingDeck == null)
        {
            WorkingDeck = new Deck(null, new());
        }

        
        if(newName == WorkingDeck.Name || (string.IsNullOrWhiteSpace(newName) && WorkingDeck.Name != null))
        {
            deckNameInputField.text = WorkingDeck.Name;
        }

        if (string.IsNullOrWhiteSpace(newName))
        {
            deckNameInputField.text = localDeckNameInvalid;
            return;
        }

        if (SaveDataManager.Instance.UserDecks.ContainsKey(newName))
        {
            Debug.Log("Deck with this name already exists. Enter different name");
            deckNameInputField.text = localDeckNameInvalid;
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

    public void OnDeckNameInputFieldSelection(string newName)
    {
        deckNameInputField.text = "";
    }


    private void ShowSelectedDeck()
    {
        var dropdown = deckSelectionDropdown.GetComponent<TMP_Dropdown>();
        if (dropdown.value != dropdown.options.Count - 1)
        {
            ShowDeckByName(dropdown.options[dropdown.value].text);
        }
    }


    public void ShowDeckByName(string name)
    {
        ClearDeckUI();
        if (name == null)
            return;

        var deck = SaveDataManager.Instance.GetDeck(name);
        if (deck == null)
        {
            throw new Exception("You dont have this deck");
        }


        foreach (var cardSO in deck.GetDeckList(true))
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

    private void ShowDeckUI(Deck deck)
    {
        foreach (var card in deck.GetDeckList(true))
        {
            AddCardToDeckUI(card);
        }
    }


    private void ChangeDeckNameEnterText(string s)
    {
        localDeckNameEnter = s;
    }

    private void ChangeDeckCreateNewText(string s)
    {
        localDeckCreateNew = s;
        Reload();
    }

    private void ChangeDeckNameInvalidText(string s)
    {
        localDeckNameInvalid = s;
    }
}
