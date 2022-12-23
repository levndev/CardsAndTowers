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
    public ScrollView AvailablePacks;

    public GameObject PacksScrollView;
    public GameObject PacksScrollViewContent;

    public GameObject OpenedCardsPanel;
    public GameObject OpenedCardsScrollView;
    public UnityEngine.UI.Button OpenedCardsPanelButton;

    public GameObject PackSlotPrefab;
    public GameObject PackPrefab;

    public GameObject CardPrefab;
    public GameObject CardSlotPrefab;

    public TextMeshProUGUI HelpMessage;

    public List<UIPackController> UIPackList = new List<UIPackController>();
    public Dictionary<Pack, int> AllPacks = new Dictionary<Pack, int>();

    private string availablePacksFileName = "packs.txt";

    void Start()
    {
        OpenedCardsPanelButton = OpenedCardsScrollView.GetComponent<UnityEngine.UI.Button>();
        OpenedCardsPanelButton.onClick.AddListener(onOpenedCardsPanelClick);
        //if (HelpMessage != null)
        //{
        //    StartCoroutine(FadeUnfade());
        //}
    }

    IEnumerator FadeUnfade()
    {
        var time = 0f;
        while (true)
        {

            time += Time.deltaTime;
            float alpha = 127.5f + 127.5f * Mathf.Cos(time);
            float r = HelpMessage.color.r;
            float g = HelpMessage.color.g;
            float b = HelpMessage.color.b;
            HelpMessage.color = new Color(r, g, b, alpha);
            yield return new WaitForSeconds(0.1f);

        }
    }

    private void Update()
    {
        float alpha = HelpMessage.color.a;
        float r = HelpMessage.color.r;
        float g = HelpMessage.color.g;
        float b = HelpMessage.color.b;
        //HelpMessage.color = new Color(r, g, b, alpha);
        HelpMessage.color = Color.Lerp(new Color(r,g,b,0), new Color(r,g,b,255), Mathf.Abs(Mathf.Sin(Time.deltaTime *5)));

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
                uiCardController.CurrentCardState = UICardController.CardState.inPack;
                uiCardController.packsScreen = this;
                uiCardController.SetFromCard(card);
            }
        }
        AllPacks[sender.pack] -= 1;
        sender.packSlot.DecreseAmount();
        if (sender.packSlot.Amount <= 0)
        {
            AllPacks.Remove(sender.pack);
            Destroy(sender.gameObject.transform.parent.gameObject);
        }

    }

    public void onCardClick(UICardController sender)
    {

    }

    public void onOpenedCardsPanelClick()
    {
        foreach (Transform child in OpenedCardsPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }


    public List<Pack> LoadPacks()
    {
        var packs = new List<Pack>();
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
                    packs.Add(Pack.LoadFromFile(line[0]));
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
            sw.Flush();
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
        onOpenedCardsPanelClick();
    }
}
