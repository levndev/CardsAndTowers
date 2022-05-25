using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class UICardController : MonoBehaviour
{
    public Card card;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI nameText;
    public Image artImage;
    public int HandIndex;
    public Button Button;
    public GameManager GameManager;
    public bool InHand;
    void Awake()
    {
        //costText = transform.Find("CostText").GetComponent<TextMeshProUGUI>();
        //nameText = transform.Find("NameText").GetComponent<TextMeshProUGUI>();
        //artImage = transform.Find("Art").GetComponent<UnityEngine.UI.Image>();
    }

    void Start()
    {
        
        Button.onClick.AddListener(OnClick);
        SetFromCard(card);
    }

    public void OnClick()
    {
        if(InHand)
        {
            if (GameManager == null)
            {
                GameManager = GameManager.Instance;
            }
            GameManager.CardClick(HandIndex);

        }
    }

    public void SetFromCard(Card card)
    {
        this.card = card;
        costText.text = card.Cost.ToString();
        nameText.text = card.Name;
        artImage.sprite = card.Art;
    }
}
