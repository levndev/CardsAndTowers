using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public List<GameObject> HandPositions;
    public List<CardSO> Hand = new List<CardSO>();
    public Queue<CardSO> Deck = new Queue<CardSO>();
    private int HandSize;
    public GameState GameState = GameState.None;
    private int CurrentCardSelected = -1;
    public GameObject CardPrefab;
    public GameObject BasicTurretPrefab;
    public InputManager InputManager;
    public MapManager MapManager;
    public Reward Reward;
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
    public GameObject Base;
    public float TimeToWin;
    public float TimeToWinLeft;
    public TMPro.TextMeshProUGUI WinTimerText;

    public GameObject WinPanel;
    public WinPanelSO WinPanelSO;

    public GameObject LosePanel;
    public GameObject TowerPrefab;
    public GameObject WallPrefab;
    public float WallCost;
    public GameObject TowerPlacementEffect;

    public float MaxMatter;
    public float CurrentMatter;
    public float MatterGenerationRate;
    public BarSO MatterBar;

    private float totalWallEnergyCost;

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
        try
        {
            //var deckList = global::Deck.LoadFromFile("deck1").GetDeckList();
            var deckList = SaveDataManager.Instance.GetDeck(SaveDataManager.Instance.CurrentDeck).GetDeckList();
            foreach (var card in deckList)
            {
                Deck.Enqueue(card);
            }
        }
        catch
        {
            Deck.Enqueue(Resources.Load<CardSO>("Cards/Basic"));
            Deck.Enqueue(Resources.Load<CardSO>("Cards/Machinegun"));
            Deck.Enqueue(Resources.Load<CardSO>("Cards/Sniper"));
            Deck.Enqueue(Resources.Load<CardSO>("Cards/Flame"));
            Deck.Enqueue(Resources.Load<CardSO>("Cards/Fireball"));
            Deck.Enqueue(Resources.Load<CardSO>("Cards/AcidBomb"));
        }
        for (var i = 0; i < HandSize; i++)
        {
            Hand.Add(null);
        }
    }

    void Start()
    {
        InputManager.Touched += OnTap;
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
        Time.timeScale = 1.0f;
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

    public void Win()
    {
        SetPause(true);
        WinPanel.SetActive(true);
        WinPanelSO.GoldAmount.text = Reward.Gold.ToString();
        foreach (var pack in Reward.Packs)
        {
            WinPanelSO.Pack.text += pack.Name + " X" + pack.Amount.ToString() + "\n";
        }
        foreach (var card in Reward.Cards)
        {
            WinPanelSO.Card.text = SaveDataManager.Instance.AllCards[Reward.Cards[0].UID].Name + " X" + Reward.Cards[0].Amount.ToString() + "\n";
        }
        Reward.GetReward();
    }

    public void SetPause(bool pause)
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
            Win();
        }

        GenerateEnergy();
        GenerateMatter();

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
        UpdateMatterDisplay();
        UpdateCardsOutlines();


        void GenerateEnergy()
        {
            CurrentEnergy += EnergyGenerationRate * Time.deltaTime;
            if (CurrentEnergy >= MaxEnergy)
            {
                CurrentEnergy = MaxEnergy;
            }
        }

        void GenerateMatter()
        {
            if (CurrentEnergy == MaxEnergy)
            {
                CurrentMatter += MatterGenerationRate * Time.deltaTime;
            }
            if (CurrentMatter >= MaxMatter)
            {
                Win();
            }
        }
    }

    public void UpdateCardsOutlines()
    {
        for (var i = 0; i < HandPositions.Count; i++)
        {
            foreach (Transform cardUI in HandPositions[i].transform)
            {
                if (cardUI.gameObject.TryGetComponent<Outline>(out var outline))
                {
                    if (Hand[i] != null)
                    {
                        if (CurrentEnergy >= Hand[i].Cost)
                            outline.enabled = true;
                        else
                            outline.enabled = false;
                    }
                }
            }
        }
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
#if UNITY_WEBGL
            if (touch.RightButton)
            {
                SetCardPlayingMode(false);
                return;
            }
#endif
            if (BuildingGhosts.Count > 0)
            {
                foreach (var oldGhost in BuildingGhosts)
                {
                    Destroy(oldGhost);
                }
                BuildingGhosts.Clear();
            }
            var card = Hand[CurrentCardSelected];
            if (card.type == CardSO.Type.Spell)
            {
                if (CurrentEnergy >= card.Cost)
                {
                    CurrentEnergy -= card.Cost;

                    Instantiate(card.SpellEffect, touch.Position, new Quaternion());
                    Hand[CurrentCardSelected] = null;
                    Deck.Enqueue(card);
                    Destroy(HandPositions[CurrentCardSelected].transform.GetChild(0).gameObject);
                    SetCardPlayingMode(false);
                }
                return;
            }
            var buildingSize = TowerPrefab.GetComponent<SizeData>().Size;

            var ghostPosition = MapManager.GetBuildingCenter(touch.Position, buildingSize);
            var ghost = Instantiate(TowerPrefab, ghostPosition, new Quaternion());
            ghost.GetComponent<TowerController>().SetTower(card.Tower);
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
#if UNITY_WEBGL
            if (touch.RightButton)
            {
                SetCardPlayingMode(false);
                return;
            }
            var buildingSize = WallPrefab.GetComponent<SizeData>().Size;
            List<Vector2> positions = new List<Vector2>();
            for (var t = 0f; t < 1f; t += 0.01f)
                positions.Add(Vector3.Lerp(touch.StartPosition, touch.Position, t));
            var oldGhostPosition = Vector3.forward;
            foreach (var position in positions)
            {
                if (MapManager.GetGhost(position))
                    continue;
                if (totalWallEnergyCost >= CurrentEnergy)
                    continue;
                var ghostPosition = MapManager.GetBuildingCenter(position, buildingSize);
                if (ghostPosition == oldGhostPosition)
                    continue;
                oldGhostPosition = ghostPosition;
                var ghost = Instantiate(WallPrefab, ghostPosition, new Quaternion());
                totalWallEnergyCost += WallCost;
                var ghostMode = ghost.GetComponent<GhostMode>();
                ghostMode.Enable();
                var ghostState = MapManager.IsValidPlacement(position, buildingSize)
                    ? GhostMode.States.ViablePosition
                    : GhostMode.States.WrongPosition;
                ghostMode.SetState(ghostState);
                BuildingGhosts.Add(ghost);
                MapManager.SetGhost(position, true);
            }
            ConfirmBuildingButton.gameObject.SetActive(true);
#endif
#if UNITY_ANDROID
            if (BuildingGhosts.Count > 0)
            {
                foreach (var oldGhost in BuildingGhosts)
                {
                    Destroy(oldGhost);
                }
                BuildingGhosts.Clear();
            }
            var buildingSize = WallPrefab.GetComponent<SizeData>().Size;
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
                var ghost = Instantiate(WallPrefab, ghostPosition, new Quaternion());
                var ghostMode = ghost.GetComponent<GhostMode>();
                ghostMode.Enable();
                var ghostState = MapManager.IsValidPlacement(position, buildingSize)
                    ? GhostMode.States.ViablePosition
                    : GhostMode.States.WrongPosition;
                ghostMode.SetState(ghostState);
                BuildingGhosts.Add(ghost);
            }
            ConfirmBuildingButton.gameObject.SetActive(true);
#endif
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
#if UNITY_WEBGL
                MapManager.SetGhost(ghost.transform.position, false);
#endif
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
                if (ghost.GetComponent<GhostMode>().GetState() != GhostMode.States.ViablePosition)
                {
                    Destroy(ghost);
                    BuildingGhosts.RemoveAt(i);
                    continue;
                }
                var card = Hand[CurrentCardSelected];
                if (CurrentEnergy >= card.Cost)
                {
                    CurrentEnergy -= card.Cost;
                    Hand[CurrentCardSelected] = null;

                    var tower = Instantiate(TowerPrefab, ghost.transform.position, new Quaternion());
                    tower.GetComponent<TowerController>().SetTower(card.Tower);
                    Instantiate(TowerPlacementEffect, tower.transform.position, new Quaternion());
                    MapManager.AddObject(tower, ghost.transform.position);
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
                if (ghost.GetComponent<GhostMode>().GetState() == GhostMode.States.ViablePosition)
                {
                    var wall = Instantiate(WallPrefab);
                    CurrentEnergy -= WallCost;
                    MapManager.AddObject(wall, ghost.transform.position, false);
                    MapManager.RegeneratePaths();
                }
#if UNITY_WEBGL
                MapManager.SetGhost(ghost.transform.position, false);
#endif
                Destroy(ghost);
                BuildingGhosts.RemoveAt(i);
            }
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
            VisibleGrid.SetActive(false);
            totalWallEnergyCost = 0;
        }
        else
        {
            GameState = GameState.WallBuilding;
            cameraTarget.Enabled = false;
            VisibleGrid.SetActive(true);
            totalWallEnergyCost = 0;
        }
    }

    private void UpdateEnergyDisplay()
    {
        EnergyText.text = CurrentEnergy.ToString("N0");
        EnergyBar.value = CurrentEnergy / MaxEnergy;
    }

    private void UpdateMatterDisplay()
    {
        MatterBar.Text.text = CurrentMatter.ToString("N0");
        MatterBar.Slider.value = CurrentMatter / MaxMatter;
    }
}

public enum GameState : int
{
    None = 0,
    CardPlaying = 1,
    WallBuilding = 2,
    Paused = 3
}
