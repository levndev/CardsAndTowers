using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.IO;
using System.Linq;

public class PacksScreen : MenuScreen
{
    public ScrollView AvailablePacks;

    public GameObject PacksScrollView;
    public GameObject PacksScrollViewContent;

    public GameObject OpenedCardsPanel;
    public UnityEngine.UI.Button OpenedCardsPanelButton;

    public GameObject PackSlotPrefab;
    public GameObject PackPrefab;

    public GameObject CardPrefab;
    public GameObject CardSlotPrefab;

    public List<UIPackController> UIPackList = new List<UIPackController>();
    public Dictionary<Pack,int> AllPacks = new Dictionary<Pack, int>();

    private string availablePacksFileName = "packs.txt";

    void Start()
    {
        OpenedCardsPanelButton = OpenedCardsPanel.GetComponent<UnityEngine.UI.Button>();
        OpenedCardsPanelButton.onClick.AddListener(onOpenedCardsPanelClick);

        var loadedPacks = LoadPacks();
        foreach(var loadedPack in loadedPacks)
        {
            if (!AllPacks.Keys.Contains(loadedPack))//нет такого типа пакетов 
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
                UIPackList.Add(uiPack);
            }
            else // уже есть такой вид пакета
            {
                AllPacks[loadedPack] += 1;
                var uiPack = UIPackList.Where(uiPack => uiPack.pack == loadedPack).ElementAt(0);
                uiPack.packSlot.IncreaseAmount();
            }
        }
    }

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
                uiCardController.CurrentCardState = UICardController.CardState.inCollection;
                uiCardController.packsScreen = this;
                uiCardController.SetFromCard(card);
            }
        }
        AllPacks[sender.pack] -= 1;
        sender.packSlot.DecreseAmount();
        if(sender.packSlot.Amount <= 0)
        {
            Destroy(sender.gameObject.transform.parent);
        }
        ///сгенерировать карты
        ///отрисовать карты
        ///добавить карты в коллекцию
        ///удалить отрисованные карты
    }

    public void onCardClick(UICardController sender)
    {

    }

    public void onOpenedCardsPanelClick()
    {
        foreach (Transform child in OpenedCardsPanel.transform)
        {
            Destroy(child);
        }
    }


    public List<Pack> LoadPacks()
    {
        var packs = new List<Pack>();
        try
        {
            var lines = File.ReadAllLines(Application.persistentDataPath + "/" + availablePacksFileName);
            foreach (var line in lines.Select(str => str.Split(' ')))
            {
                for(var i = 0; i < int.Parse(line[1]); i++)
                {
                    packs.Add(Pack.LoadFromFile(line[0]));
                }
            }
        }
        catch(System.Exception e)
        {
            Debug.Log("Something wrong with file. SUSSY");
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

    public override void onScreenLeave()
    {
        SaveUnusedPacks();
    }
}
