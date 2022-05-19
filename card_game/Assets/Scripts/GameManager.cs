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
        Deck.Enqueue(Resources.Load<Card>("Cards/Basic2"));
        Deck.Enqueue(Resources.Load<Card>("Cards/Basic"));
        Deck.Enqueue(Resources.Load<Card>("Cards/Basic2"));
        Deck.Enqueue(Resources.Load<Card>("Cards/Basic"));
        for (var i = 0; i < HandSize; i++)
        {
            Hand.Add(null);
        }
    }

    void Start()
    {
        InputManager.TapRegistered += OnTap;
    }

    void Update()
    {
        for (var i = 0; i < HandSize; i++)
        {
            if (Deck.Count <= 0)
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
            }
        }
    }

    public void CardClick(int HandIndex)
    {
        Debug.Log("Current Card: " + CurrentCardSelected.ToString() + " Hand index: " + HandIndex.ToString());
        CardPlayingMode = CurrentCardSelected != HandIndex;
        if (CardPlayingMode)
        {
            CurrentCardSelected = HandIndex;
        }
        else
        {
            CurrentCardSelected = -1;
        }
    }

    private void OnTap(object sender, TapEventArgs args)
    {
        if (CardPlayingMode)
        {
            var touchPosition = args.Position;

            Instantiate(Hand[CurrentCardSelected].SpawnedObject, touchPosition, new Quaternion());
            CardPlayingMode = false;
            CurrentCardSelected = -1;
        }
    }
}
