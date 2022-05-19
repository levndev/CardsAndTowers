using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameManager _instance;
    public List<GameObject> HandPositions;
    public List<Card> Hand = new List<Card>();
    public Queue<Card> Deck = new Queue<Card>();
    private int HandSize;
    public bool CardPlayingMode;
    private int CurrentCardSelected = -1;
    public GameObject CardPrefab;
    public GameObject BasicTurretPrefab;
    public InputManager InputManager;
    public Grid MapGrid;
    public GridController GridController;
    public GameObject VisibleGrid;
    public float CurrentEnergy;
    public float MaxEnergy;
    public float EnergyGenerationRate;
    public TMPro.TextMeshProUGUI EnergyText;
    public UnityEngine.UI.Slider EnergyBar;
    public float DeckDrawCoolDown;
    private bool deckDrawTimerEnabled;
    private float deckDrawTimeRemaining;
    public GameManager Instance
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

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            throw new System.Exception("GameManager already exists");
        }
        HandSize = HandPositions.Count;
        Deck.Enqueue(Resources.Load<Card>("Cards/Basic"));
        Deck.Enqueue(Resources.Load<Card>("Cards/Sniper"));
        Deck.Enqueue(Resources.Load<Card>("Cards/Basic"));
        Deck.Enqueue(Resources.Load<Card>("Cards/Sniper"));
        Deck.Enqueue(Resources.Load<Card>("Cards/Basic"));
        for (var i = 0; i < HandSize; i++)
        {
            Hand.Add(null);
        }
    }

    void Start()
    {
        InputManager.TapRegistered += OnTap;
        InvokeRepeating("GenerateEnergy", 0, 1);
    }

    void Update()
    {
        for (var i = 0; i < HandSize; i++)
        {
            if (Deck.Count == 0 || deckDrawTimerEnabled)
                break;
            if (Hand[i] == null)
            {
                var card = Deck.Dequeue();
                Hand[i] = card;
                var uiCard = Instantiate(CardPrefab, HandPositions[i].transform);
                var uiCardController = uiCard.GetComponent<UICardController>();
                uiCardController.card = card;
                uiCardController.GameManager = this;
                uiCardController.HandIndex = i;
                deckDrawTimerEnabled = true;
                deckDrawTimeRemaining = DeckDrawCoolDown;
            }
        }
        if (deckDrawTimerEnabled)
        {
            if (deckDrawTimeRemaining > 0)
            {
                deckDrawTimeRemaining -= Time.deltaTime;
            }
            else
            {
                deckDrawTimeRemaining = 0;
                deckDrawTimerEnabled = false;
            }
        }
        UpdateEnergyDisplay();
    }

    public void CardClick(int HandIndex)
    {
        SetCardPlayingMode(CurrentCardSelected != HandIndex, HandIndex);
    }

    private void OnTap(object sender, TapEventArgs args)
    {
        if (CardPlayingMode)
        {
            var card = Hand[CurrentCardSelected];
            if (CurrentEnergy >= card.Cost)
            {
                CurrentEnergy -= card.Cost;
                Hand[CurrentCardSelected] = null;
                var touchPosition = args.Position;
                GridController.InstantiateObject(card.SpawnedObject, touchPosition);
                Deck.Enqueue(card);
                Destroy(HandPositions[CurrentCardSelected].transform.GetChild(0).gameObject);
            }
            SetCardPlayingMode(false);
        }
    }

    private void SetCardPlayingMode(bool mode, int handIndex = -1)
    {
        CardPlayingMode = mode;
        if (mode)
        {
            CurrentCardSelected = handIndex;
            VisibleGrid.SetActive(true);
        }
        else
        {
            CurrentCardSelected = -1;
            VisibleGrid.SetActive(false);
        }
    }

    private void GenerateEnergy()
    {
        CurrentEnergy += EnergyGenerationRate;
        if (CurrentEnergy >= MaxEnergy)
        {
            CurrentEnergy = MaxEnergy;
        }
    }

    private void UpdateEnergyDisplay()
    {
        EnergyText.text = CurrentEnergy.ToString("N0");
        EnergyBar.value = CurrentEnergy / MaxEnergy;
    }
}
