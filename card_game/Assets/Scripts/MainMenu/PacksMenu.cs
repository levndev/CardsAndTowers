using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.IO;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using System;
using UnityEngine.Events;
using Unity.VisualScripting;

public class PacksMenu : MonoBehaviour
{
    public static Action<object, PackSO, CardSO[]> PackOpened;

    public GameObject PacksScrollViewContent;

    public GameObject OpenedCardsPanel;
    public UnityEngine.UI.Button OpenedCardsPanelButton;

    public GameObject PackSlotPrefab;
    public GameObject PackPrefab;

    public GameObject CardPrefab;
    public GameObject CardSlotPrefab;

    public TextMeshProUGUI HelpMessage;

    public List<UIPackController> UIPackList = new List<UIPackController>();


    public void onPackClick(UIPackController sender)
    {
        if (!SaveDataManager.Instance.UserPacks.ContainsKey(sender.pack))
        {
            throw new Exception("You dont have this pack on your account. SUSSY");
        }
        var newCards = sender.GenerateCards();
        HelpMessage.gameObject.SetActive(false);

        PackOpened?.Invoke(this, sender.pack, newCards);

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

        sender.packSlot.DecreseAmount();
        if (sender.packSlot.Amount <= 0)
        {
            Destroy(sender.gameObject.transform.parent.gameObject);
        }
    }

    public void OnCardClick(UICardController sender)
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


    private void OnEnable()
    {
        var loadedPacks = SaveDataManager.Instance.UserPacks;
        foreach(var loadedPack in loadedPacks)
        {
            var packSlot = Instantiate(PackSlotPrefab, PacksScrollViewContent.transform);
            var uiPackSlot = packSlot.GetComponent<PackSlot>();
            uiPackSlot.SetAmount((int)loadedPack.Value);

            var pack = Instantiate(PackPrefab, packSlot.transform);
            var uiPack = pack.GetComponent<UIPackController>();
            uiPack.packsScreen = this;
            uiPack.SetFromPack(loadedPack.Key);
            uiPack.packSlot = uiPackSlot;
            uiPack.transform.SetSiblingIndex(0);
            UIPackList.Add(uiPack);
        }
    }

    private void OnDisable()
    {
        OnOpenedCardsPanelClick();
        ResetPacks();
    }

    private void ResetPacks()
    {
        //AllPacks.Clear();
        UIPackList.Clear();
        foreach (Transform child in PacksScrollViewContent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
