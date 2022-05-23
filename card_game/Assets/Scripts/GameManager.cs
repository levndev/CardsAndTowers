using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public List<GameObject> HandPositions;
    public List<Card> Hand = new List<Card>();
    public Queue<Card> Deck = new Queue<Card>();
    private int HandSize;
    public bool CardPlayingMode;
    private int CurrentCardSelected = -1;
    public GameObject CardPrefab;
    public GameObject BasicTurretPrefab;
    public InputManager InputManager;
    public MapManager MapManager;
    public GameObject VisibleGrid;
    public float CurrentEnergy;
    public float MaxEnergy;
    public float EnergyGenerationRate;
    public TMPro.TextMeshProUGUI EnergyText;
    public UnityEngine.UI.Slider EnergyBar;
    public float DeckDrawCooldown;
    private bool deckDrawTimerEnabled = false;
    private float deckDrawTimeRemaining = 0;
    public UICardController DeckTop;
    public UnityEngine.UI.Slider DrawCooldownBar;
    public UnityEngine.UI.Button ConfirmBuildingButton;
    private List<GameObject> BuildingGhosts = new List<GameObject>();
    public CameraTarget cameraTarget;
    private Vector2 dragDeltaAcc;
    public Vector2 DragCoefficient;

    public static GameManager Instance
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
        InputManager.TouchMoved += OnDrag;
        InvokeRepeating("GenerateEnergy", 0, 1);
        ConfirmBuildingButton.onClick.AddListener(OnConfirmBuildingButtonClick);
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
                deckDrawTimeRemaining = DeckDrawCooldown;
                if (Deck.Count > 0)
                {
                    DeckTop.gameObject.SetActive(true);
                    DeckTop.SetFromCard(Deck.Peek());
                }
                else
                {
                    DeckTop.gameObject.SetActive(false);
                }
            }
        }
        if (deckDrawTimerEnabled)
        {
            if (deckDrawTimeRemaining > 0)
            {
                deckDrawTimeRemaining -= Time.deltaTime;
                DrawCooldownBar.value = 1 - (deckDrawTimeRemaining / DeckDrawCooldown);
            }
            else
            {
                deckDrawTimeRemaining = 0;
                deckDrawTimerEnabled = false;
            }
        }
        else
        {
            DrawCooldownBar.value = 1;
        }
        UpdateEnergyDisplay();
    }

    public void CardClick(int HandIndex)
    {
        SetCardPlayingMode(CurrentCardSelected != HandIndex, HandIndex);
    }

    private void OnDrag(object sender, TapEventArgs args)
    {
        Debug.Log(args.Delta);
        if (CardPlayingMode)
        {
            var delta = args.Delta;
            var resolution = Screen.currentResolution;
            var orthoSize = 10 * 2;
            var aspectRatio = resolution.width / resolution.height;
            var horizontalOrthoSize = orthoSize * aspectRatio;
            delta = new Vector2(delta.x / resolution.width, delta.y / resolution.height);
            delta = new Vector2(delta.x * horizontalOrthoSize, delta.y * orthoSize);
            dragDeltaAcc += delta;
            //dragDeltaAcc += new Vector2(args.Delta.x*10 /2340, args.Delta.y*10 /1080);
            //Debug.Log(dragDeltaAcc);
            foreach (var ghost in BuildingGhosts)
            {

                if (dragDeltaAcc.magnitude > 1f)
                {
                    var lastPosition = ghost.transform.position;
                    var size = ghost.GetComponent<SizeData>().Size;
                    var anchor = ghost.transform.position + new Vector3(dragDeltaAcc.x - size.x / 2 + 0.5f, dragDeltaAcc.y - size.y / 2 + 0.5f, 0);
                    ghost.transform.position = MapManager.GetBuildingCenter(anchor, ghost.GetComponent<SizeData>().Size);
                    if (Math.Abs(ghost.transform.position.x - lastPosition.x) > 0)
                        dragDeltaAcc.x = 0;
                    if (Math.Abs(ghost.transform.position.y - lastPosition.y) > 0)
                        dragDeltaAcc.y = 0;

                    if (MapManager.IsValidPlacement(anchor, size))
                        ghost.GetComponent<GhostMode>().SetState(GhostMode.States.ViablePosition);
                    else
                        ghost.GetComponent<GhostMode>().SetState(GhostMode.States.WrongPosition);
                }


            }
        }
    }


    private void OnTap(object sender, TapEventArgs args)
    {
        
        //Debug.Log(args.Position);
        if (CardPlayingMode)
        {
            if (BuildingGhosts.Count > 0)
            {
                foreach (var oldGhost in BuildingGhosts)
                {
                    Destroy(oldGhost);
                }
                BuildingGhosts.Clear();
            }
            var card = Hand[CurrentCardSelected];
            var touchPosition = args.Position;
            var buildingSize = card.SpawnedObject.GetComponent<SizeData>().Size;
            if (MapManager.IsValidPlacement(touchPosition, buildingSize))
            {
                var ghostPosition = MapManager.GetBuildingCenter(touchPosition, buildingSize);
                var ghost = Instantiate(card.SpawnedObject, ghostPosition, new Quaternion());
                ghost.GetComponent<GhostMode>().Enable();
                BuildingGhosts.Add(ghost);
                ConfirmBuildingButton.gameObject.SetActive(true);
            }
        }
    }

    private void SetCardPlayingMode(bool mode, int handIndex = -1)
    {
        CardPlayingMode = mode;
        cameraTarget.Enabled = !mode;
        if (mode)
        {
            CurrentCardSelected = handIndex;
            VisibleGrid.SetActive(true);
        }
        else
        {
            CurrentCardSelected = -1;
            VisibleGrid.SetActive(false);
            ConfirmBuildingButton.gameObject.SetActive(false);
            foreach (var ghost in BuildingGhosts)
            {
                Destroy(ghost);
            }
            BuildingGhosts.Clear();
        }
    }

    private void OnConfirmBuildingButtonClick()
    {
        for (var i = BuildingGhosts.Count - 1; i >= 0; i--)
        {
            var ghost = BuildingGhosts[i];
            var card = Hand[CurrentCardSelected];
            if (CurrentEnergy >= card.Cost)
            {
                CurrentEnergy -= card.Cost;
                Hand[CurrentCardSelected] = null;
                MapManager.InstantiateObject(card.SpawnedObject, ghost.transform.position);
                Deck.Enqueue(card);
                Destroy(HandPositions[CurrentCardSelected].transform.GetChild(0).gameObject);
                Destroy(ghost);
                BuildingGhosts.RemoveAt(i);
            }
            SetCardPlayingMode(false);
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
