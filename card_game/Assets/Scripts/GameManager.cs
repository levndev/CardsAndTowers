using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private GameManager _instance;
    public List<GameObject> HandPositions;
    public List<Card> Hand = new List<Card>();
    public Queue<Card> Deck = new Queue<Card>();
    private int HandSize;
    public GameObject CardPrefab;
    public bool TurretSpawningMode;
    public GameObject BasicTurretPrefab;
    public Camera camera;
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
        Deck.Enqueue(Resources.Load<Card>("Cards/Basic"));
        Deck.Enqueue(Resources.Load<Card>("Cards/Basic"));
        Deck.Enqueue(Resources.Load<Card>("Cards/Basic"));
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
        
        if (Deck.Count > 0)
        {
            for (var i = 0; i < HandSize; i++)
            {
                if (Hand[i] == null)
                {
                    var card = Deck.Dequeue();
                    Hand[i] = card;
                    var uiCard = Instantiate(CardPrefab, HandPositions[i].transform);
                    var uiCardButton = uiCard.GetComponent<Button>();
                    uiCardButton.onClick.AddListener(CardClick);
                    var uiCardController = uiCard.GetComponent<UICardController>();
                    uiCardController.card = card;
                }
            }
        }
    }

    void CardClick()
    {
        Debug.Log("sususus");
        TurretSpawningMode = true;
    }

    private void OnTap(object sender, TapEventArgs args)
    {
        if (TurretSpawningMode)
        {
            var touchPosition = args.Position;
            Instantiate(BasicTurretPrefab, touchPosition, new Quaternion());
            TurretSpawningMode = false;
        }
    }
}
