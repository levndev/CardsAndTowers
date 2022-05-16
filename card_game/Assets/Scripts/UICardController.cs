using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UICardController : MonoBehaviour
{
    public Card card;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI nameText;
    public UnityEngine.UI.Image artImage;

    void Awake()
    {
        //costText = transform.Find("CostText").GetComponent<TextMeshProUGUI>();
        //nameText = transform.Find("NameText").GetComponent<TextMeshProUGUI>();
        //artImage = transform.Find("Art").GetComponent<UnityEngine.UI.Image>();
    }

    void Start()
    {
        costText.text = card.Cost.ToString();
        nameText.text = card.Name;
        artImage.sprite = card.Art;
    }
}
