using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public List<GameObject> HandPositions;
    public List<Card> Hand = new List<Card>();
    public Queue<Card> Deck = new Queue<Card>();
    private int HandSize;
    public GameState GameState = GameState.None;
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
    public UnityEngine.UI.Button BuildWallButton;
    private List<GameObject> BuildingGhosts = new List<GameObject>();
    public CameraTarget cameraTarget;
    public Vector2 DragCoefficient;
    private GameObject wallPrefab;
    public GameObject Base;
    public float TimeToWin;
    private float TimeToWinLeft;
    public TMPro.TextMeshProUGUI WinTimerText;
    public GameObject WinPanel;
    public GameObject LosePanel;
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
        Time.timeScale = 1;
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
        Deck.Enqueue(Resources.Load<Card>("Cards/Machinegun"));
        Deck.Enqueue(Resources.Load<Card>("Cards/Sniper"));
        Deck.Enqueue(Resources.Load<Card>("Cards/Flame"));
        Deck.Enqueue(Resources.Load<Card>("Cards/AcidBomb"));
        Deck.Enqueue(Resources.Load<Card>("Cards/Basic"));
        Deck.Enqueue(Resources.Load<Card>("Cards/Machinegun"));
        Deck.Enqueue(Resources.Load<Card>("Cards/Sniper"));
        Deck.Enqueue(Resources.Load<Card>("Cards/Flame"));
        Deck.Enqueue(Resources.Load<Card>("Cards/Fireball"));
        for (var i = 0; i < HandSize; i++)
        {
            Hand.Add(null);
        }
        wallPrefab = Resources.Load<GameObject>("Wall");
    }

    void Start()
    {
        InputManager.Touched += OnTap;
        InvokeRepeating("GenerateEnergy", 0, 1);
        ConfirmBuildingButton.onClick.AddListener(OnConfirmBuildingButtonClick);
        BuildWallButton.onClick.AddListener(OnWallBuildingButtonClick);
        Base.GetComponent<Health>().Death += BaseDestroyed;
        TimeToWinLeft = TimeToWin;
    }

    private void BaseDestroyed()
    {
        SetPause(true);
        LosePanel.SetActive(true);
    }

    public void Continue()
    {
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        SceneManager.LoadScene(0);
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Pause()
    {
        SetPause(GameState != GameState.Paused);
    }

    private void SetPause(bool pause)
    {
        GameState = pause ? GameState.Paused : GameState.None;
        Time.timeScale = pause ? 0 : 1;
    }

    void Update()
    {
        if (GameState == GameState.Paused)
            return;
        if (TimeToWinLeft > 0)
        {
            TimeToWinLeft -= Time.deltaTime;
            WinTimerText.text = TimeToWinLeft.ToString("N0");
        }
        else
        {
            SetPause(true);
            WinPanel.SetActive(true);
        }
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
        if (GameState != GameState.Paused)
            SetCardPlayingMode(CurrentCardSelected != HandIndex, HandIndex);
    }

    //private void OnDrag(TapEventArgs args)
    //{
    //    if (GameState)
    //    {
    //        var delta = args.Delta;
    //        var resolution = Screen.currentResolution;
    //        var orthoSize = 10 * 2;
    //        var aspectRatio = resolution.width / (float)resolution.height;
    //        var horizontalOrthoSize = orthoSize * aspectRatio;
    //        delta = new Vector2(delta.x / resolution.width, delta.y / resolution.height);
    //        delta = new Vector2(delta.x * horizontalOrthoSize, delta.y * orthoSize);
    //        dragDeltaAcc += delta;
    //        foreach (var ghost in BuildingGhosts)
    //        {

    //            if (dragDeltaAcc.magnitude > 1f)
    //            {
    //                var lastPosition = ghost.transform.position;
    //                var size = ghost.GetComponent<SizeData>().Size;
    //                var anchor = ghost.transform.position + new Vector3(dragDeltaAcc.x - size.x / 2 + 0.5f, dragDeltaAcc.y - size.y / 2 + 0.5f, 0);
    //                ghost.transform.position = MapManager.GetBuildingCenter(anchor, ghost.GetComponent<SizeData>().Size);
    //                if (Math.Abs(ghost.transform.position.x - lastPosition.x) > 0)
    //                    dragDeltaAcc.x -= ghost.transform.position.x - lastPosition.x;
    //                if (Math.Abs(ghost.transform.position.y - lastPosition.y) > 0)
    //                    dragDeltaAcc.y -= ghost.transform.position.y - lastPosition.y;
    //                if (MapManager.IsValidPlacement(anchor, size))
    //                    ghost.GetComponent<GhostMode>().SetState(GhostMode.States.ViablePosition);
    //                else
    //                    ghost.GetComponent<GhostMode>().SetState(GhostMode.States.WrongPosition);
    //            }
    //        }
    //    }
    //}


    private void OnTap(TapEventArgs touch)
    {
        if (GameState == GameState.CardPlaying)
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
            var buildingSize = card.SpawnedObject.GetComponent<SizeData>().Size;

            var ghostPosition = MapManager.GetBuildingCenter(touch.Position, buildingSize);
            var ghost = Instantiate(card.SpawnedObject, ghostPosition, new Quaternion());
            var ghostMode = ghost.GetComponent<GhostMode>();
            ghostMode.Enable();
            var ghostState = MapManager.IsValidPlacement(touch.Position, buildingSize)
                ? GhostMode.States.ViablePosition
                : GhostMode.States.WrongPosition;
            ghostMode.SetState(ghostState);
            BuildingGhosts.Add(ghost);
            ConfirmBuildingButton.gameObject.SetActive(true);
        }
        else if (GameState == GameState.WallBuilding)
        {
            if (BuildingGhosts.Count > 0)
            {
                foreach (var oldGhost in BuildingGhosts)
                {
                    Destroy(oldGhost);
                }
                BuildingGhosts.Clear();
            }
            var buildingSize = wallPrefab.GetComponent<SizeData>().Size;
            List<Vector2> positions = new List<Vector2>();
            for (var t = 0f; t < 1f; t += 0.01f) 
                positions.Add(Vector3.Lerp(touch.StartPosition, touch.Position, t));
            var oldGhostPosition = Vector3.forward;
            foreach (var position in positions)
            {
                var ghostPosition = MapManager.GetBuildingCenter(position, buildingSize);
                if (ghostPosition == oldGhostPosition)
                    continue;
                oldGhostPosition = ghostPosition;
                var ghost = Instantiate(wallPrefab, ghostPosition, new Quaternion());
                var ghostMode = ghost.GetComponent<GhostMode>();
                ghostMode.Enable();
                var ghostState = MapManager.IsValidPlacement(position, buildingSize)
                    ? GhostMode.States.ViablePosition
                    : GhostMode.States.WrongPosition;
                ghostMode.SetState(ghostState);
                BuildingGhosts.Add(ghost);
            }
            ConfirmBuildingButton.gameObject.SetActive(true);
        }
    }

    private void SetCardPlayingMode(bool mode, int handIndex = -1)
    {
        cameraTarget.Enabled = !mode;
        if (mode)
        {
            GameState = GameState.CardPlaying;
            CurrentCardSelected = handIndex;
            VisibleGrid.SetActive(true);
        }
        else
        {
            GameState = GameState.None;
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
        if (GameState == GameState.CardPlaying)
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
            }
        }
        else if (GameState == GameState.WallBuilding)
        {
            for (var i = BuildingGhosts.Count - 1; i >= 0; i--)
            {
                var ghost = BuildingGhosts[i];
                MapManager.InstantiateObject(wallPrefab, ghost.transform.position, false);
                Destroy(ghost);
                BuildingGhosts.RemoveAt(i);
            }
            MapManager.RegeneratePaths();
        }
        SetCardPlayingMode(false);
    }

    private void OnWallBuildingButtonClick()
    {
        if (GameState == GameState.Paused)
            return;
        if (GameState == GameState.WallBuilding)
        {
            GameState = GameState.None;
            cameraTarget.Enabled = true;
        }
        else
        {
            GameState = GameState.WallBuilding;
            cameraTarget.Enabled = false;
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

public enum GameState : int
{
    None = 0,
    CardPlaying = 1,
    WallBuilding = 2,
    Paused = 3
}
