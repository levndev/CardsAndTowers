using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.IO;
using System.Linq;
using TMPro;
using Unity.Mathematics;

public class PacksMenu : MonoBehaviour
{
    //public ScrollView AvailablePacks;

    //public GameObject PacksScrollView;
    public GameObject PacksScrollViewContent;

    public GameObject OpenedCardsPanel;
    //public GameObject OpenedCardsScrollView;
    public UnityEngine.UI.Button OpenedCardsPanelButton;

    public GameObject PackSlotPrefab;
    public GameObject PackPrefab;

    public GameObject CardPrefab;
    public GameObject CardSlotPrefab;

    public TextMeshProUGUI HelpMessage;

    public List<UIPackController> UIPackList = new List<UIPackController>();
    public Dictionary<PackSO, int> AllPacks = new Dictionary<PackSO, int>();

    private string availablePacksFileName = "packs.txt";


    public void onPackClick(UIPackController sender)
    {
        var newCards = sender.GenerateCards();
        if (newCards != null)
        {
            foreach (var card in newCards)
            {
                var uiCardSlot = Instantiate(CardSlotPrefab, OpenedCardsPanel.transform);
                var uiCard = Instantiate(CardPrefab, uiCardSlot.transform);

                var uiCardController = uiCard.GetComponent<UICardController>();
                uiCardController.CurrentCardState = UICardController.CardState.inPack;
                uiCardController.packsScreen = this;
                uiCardController.SetFromCard(card);
                //uiCardController.transform.SetSiblingIndex(0);
            }
        }
        AllPacks[sender.pack] -= 1;
        sender.packSlot.DecreseAmount();
        if (sender.packSlot.Amount <= 0)
        {
            AllPacks.Remove(sender.pack);
            Destroy(sender.gameObject.transform.parent.gameObject);
        }
        SaveUnusedPacks();
        HelpMessage.gameObject.SetActive(false);
    }

    public void onCardClick(UICardController sender)
    {

    }

    public void OnOpenedCardsPanelClick()
    {
        foreach (Transform child in OpenedCardsPanel.transform)
        {
            Destroy(child.gameObject);
        }
        HelpMessage.gameObject.SetActive(true);
    }


    public List<PackSO> LoadPacks()
    {
        var packs = new List<PackSO>();
        try
        {
            var path = Application.persistentDataPath + "/" + availablePacksFileName;
            if (!File.Exists(path))
            {
                string[] text =
                {
                    "Classic 2"
                };
                File.WriteAllLines(path, text);
            }
            var lines = File.ReadAllLines(path);
            foreach (var line in lines.Select(str => str.Split(' ')))
            {
                for (var i = 0; i < int.Parse(line[1]); i++)
                {
                    packs.Add(PackSO.LoadFromFile(line[0]));
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("Something wrong loading packs. SUSSY");
            Debug.Log(e.Message);
        }
        return packs;
    }

    public void SaveUnusedPacks()
    {
        using (StreamWriter sw = File.CreateText(Application.persistentDataPath + "/" + availablePacksFileName))
        {
            foreach (var (pack, amount) in AllPacks)
            {
                sw.WriteLine(pack.Name + " " + amount);
            }
        }
    }

    private void OnEnable()
    {
        var loadedPacks = LoadPacks();
        foreach (var loadedPack in loadedPacks)
        {
            if (!AllPacks.Keys.Contains(loadedPack))
            {
                AllPacks.Add(loadedPack, 1);

                var packSlot = Instantiate(PackSlotPrefab, PacksScrollViewContent.transform);
                var uiPackSlot = packSlot.GetComponent<PackSlot>();
                uiPackSlot.IncreaseAmount();
                uiPackSlot.pack = loadedPack;

                var pack = Instantiate(PackPrefab, packSlot.transform);
                var uiPack = pack.GetComponent<UIPackController>();
                uiPack.packsScreen = this;
                uiPack.SetFromPack(loadedPack);
                uiPack.packSlot = uiPackSlot;
                uiPack.transform.SetSiblingIndex(0);
                UIPackList.Add(uiPack);

            }
            else
            {
                AllPacks[loadedPack] += 1;
                var uiPack = UIPackList.Where(uiPack => uiPack.pack == loadedPack).ElementAt(0);
                uiPack.packSlot.IncreaseAmount();
            }
        }
    }

    private void OnDisable()
    {
        SaveUnusedPacks();
        OnOpenedCardsPanelClick();
        ResetPacks();
    }

    private void ResetPacks()
    {
        AllPacks.Clear();
        UIPackList.Clear();
        foreach (Transform child in PacksScrollViewContent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
