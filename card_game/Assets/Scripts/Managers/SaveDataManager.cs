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

    public List<Deck> UserDecks = new();
    public List<CardSaveData> UserCards = new();
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
        LoadAllCards();
        LoadAllPacks();
        LoadUserDecks();
        LoadUserPacks();
        CurrentDeck = YandexGame.savesData.LastUsedDeck != null ? YandexGame.savesData.LastUsedDeck : null;
    }

    private void LoadUserDecks()
    {
        foreach (var deckSave in YandexGame.savesData.Decks)
        {
            var deck = new Deck();
            deck.Name = deckSave.Name;
            foreach (var cardName in deckSave.Cards)
            {
                deck.AddToList(AllCards[cardName]);
            }
            UserDecks.Add(deck);
        }
    }

    private void LoadAllCards()
    {
        AllCards = Resources.LoadAll<CardSO>("Cards").ToDictionary(i => i.UID);
    }

    private void LoadAllPacks()
    {
        AllPacks = Resources.LoadAll<PackSO>("Packs").ToDictionary(pack => pack.Name);
    }

    public void LoadUserPacks()
    {
        var packs = new List<PackSO>();

        foreach (var packSave in YandexGame.savesData.Packs)
        {
            UserPacks.Add(AllPacks[packSave.Name], packSave.Amount);
        }
    }
}
