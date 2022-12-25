using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using YG;

public class SaveDataManager : MonoBehaviour
{
    private static SaveDataManager _instance;
    // Start is called before the first frame update
    public Dictionary<string, CardSO> AllCards = new();
    public Dictionary<string, PackSO> AllPacks = new();

    //public List<Deck> UserDecks = new();
    public Dictionary<string, Deck> UserDecks = new();
    //public List<CardSaveData> UserCards = new();
    public Dictionary<CardSO, CardSaveData> UserCards = new();
    public Dictionary<PackSO, uint> UserPacks = new();

    public string CurrentDeck;

    public static SaveDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("Sussy");
            }
            return _instance;
        }
    }


    void Start()
    {
        YandexGame.GetDataEvent += OnDataGet;
        PacksMenu.PackOpened += OnPackOpen;
        CollectionMenu.DeckAction += OnDeckAction;
        //PacksMenu.CardsAcquired += OnCardsAcquired;
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            throw new System.Exception("SaveDataManager already exists");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnDataGet()
    {
        AllCards = Resources.LoadAll<CardSO>("Cards").ToDictionary(i => i.UID);
        AllPacks = Resources.LoadAll<PackSO>("Packs").ToDictionary(pack => pack.Name);
        LoadUserCards();
        LoadUserDecks();
        LoadUserPacks();
        CurrentDeck = YandexGame.savesData.LastUsedDeck != null ? YandexGame.savesData.LastUsedDeck : null;
    }

    public Deck GetDeck(string name)
    {
        return UserDecks.ContainsKey(name) ? UserDecks[name].Clone() : null;
    }

    private void LoadUserCards()
    {
        foreach (var cardSave in YandexGame.savesData.Cards)
        {
            UserCards.Add(AllCards[cardSave.UID], cardSave);
        }
    }

    private void LoadUserDecks()
    {
        foreach (var deckSave in YandexGame.savesData.Decks)
        {
            var deck = new Deck();
            deck.Name = deckSave.Name;
            foreach (var cardID in deckSave.Cards)
            {
                deck.AddToList(AllCards[cardID]);
            }
            UserDecks[deck.Name] = deck;
        }
        if(UserDecks.Count == 0)
        {
            var deckSave = new DeckSaveData
            {
                Name = "Default",
                Cards = new List<string>
                {
                    "b5025eb8-1d44-44cc-8f80-002014ab15fc",
                    "58afd0e9-c88c-4092-baf3-f57d9a8c647f",
                },
            };
            var deck = new Deck();
            deck.Name = deckSave.Name;
            foreach (var cardID in deckSave.Cards)
            {
                deck.AddToList(AllCards[cardID]);
            }
            UserDecks[deck.Name] = deck;
        }
    }

    private void LoadUserPacks()
    {
        var packs = new List<PackSO>();

        foreach (var packSave in YandexGame.savesData.Packs)
        {
            UserPacks.Add(AllPacks[packSave.Name], packSave.Amount);
        }
    }

    private void OnDeckAction(object sender, DeckEventArgs args)
    {
        switch (args.DeckAction)
        {
            case DeckAction.Removed:
                UserDecks.Remove(args.Name);
                break;

            case DeckAction.Created:
                UserDecks.Add(args.Name, args.Deck.Clone());
                break;

            case DeckAction.Changed:
                UserDecks.Remove(args.Name);
                UserDecks.Add(args.Name, args.Deck.Clone());
                break;

            case DeckAction.Renamed:
                UserDecks.Add(args.Name, args.Deck.Clone());
                break;
        }
        CurrentDeck = args.Name;
        UpdateUserDeckSaves();

    }

    private void OnPackOpen(object sender, PackSO pack, CardSO[] acquiredCards)
    {
        if (!UserPacks.ContainsKey(pack) || acquiredCards == null)
        {
            Debug.Log($"User Does not have pack {pack.Name} in their inventory or cards were null KEK");
            return;
        }

        UserPacks[pack] -= 1;
        if (UserPacks[pack] == 0)
        {
            UserPacks.Remove(pack);
        }


        foreach (var acquiredCard in acquiredCards)
        {
            if (UserCards.ContainsKey(acquiredCard))
            {
                UserCards[acquiredCard].Amount += 1;
            }
            else
            {
                UserCards.Add(acquiredCard, new CardSaveData() { Amount = 1, UID = acquiredCard.UID, Level = 1 });
            }
        }

        UpdateUserCardsPacks();
    }

    private void UpdateUserCardsPacks()
    {
        YandexGame.savesData.Cards = GetUserCardsSave();
        YandexGame.savesData.Packs = GetUserPacksSaves();
        YandexGame.SaveProgress();
        //YandexGame.LoadProgress();
    }

    private void UpdateUserDeckSaves()
    {
        YandexGame.savesData.Decks = GetUserDecksSaves();
        YandexGame.savesData.LastUsedDeck = CurrentDeck;
        YandexGame.SaveProgress();
        //YandexGame.LoadProgress();
    }

    private List<PackSaveData> GetUserPacksSaves()
    {
        var saves = new List<PackSaveData>();
        foreach ((var packSO, var packAmount) in UserPacks)
        {
            saves.Add(new PackSaveData() { Amount = packAmount, Name = packSO.Name });
        }
        return saves;
    }

    private List<CardSaveData> GetUserCardsSave()
    {
        var saves = new List<CardSaveData>();
        foreach ((var cardSO, var cardSave) in UserCards)
        {
            saves.Add(cardSave);
        }
        return saves;
    }

    private List<DeckSaveData> GetUserDecksSaves()
    {
        var saves = new List<DeckSaveData>();
        foreach ((var deckName, var deck) in UserDecks)
        {
            var save = new DeckSaveData();
            save.Name = deckName;
            save.Cards = new();
            foreach (var card in deck.deckList)
            {
                save.Cards.Add(card.UID);
            }
            saves.Add(save);
        }
        return saves;
    }


}

public enum DeckAction
{
    Created = 0,
    Removed = 1,
    Changed = 2,
    Renamed = 3
}

public class DeckEventArgs : EventArgs
{
    public string Name;
    public Deck Deck;
    public DeckAction DeckAction;
    public DeckEventArgs(string name, Deck deck, DeckAction deckAction) : base()
    {
        this.Deck = deck;
        this.DeckAction = deckAction;
        this.Name = name;
    }
}
