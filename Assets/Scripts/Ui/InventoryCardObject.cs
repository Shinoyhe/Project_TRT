using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCardObject : MonoBehaviour
{
    [SerializeField] private InventoryCard _card;

    [SerializeField] private TMP_Text _cardNameText;
    [SerializeField] private Image _cardSpriteImage;
    [SerializeField] private TMP_Text _cardDescriptionText;

    public string CardName;
    public string CardDescription;
    public string CardID;

    // Start is called before the first frame update
    void Start()
    {
        if (_card != null)
        {
            SetData(_card);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetData(InventoryCard newCard)
    {
        if (newCard == null) return;
        _card = newCard;
        
        _cardNameText.text = _card.CardName;
        _cardSpriteImage.sprite = _card.Sprite;
        _cardDescriptionText.text = _card.Description;

        CardName = _card.CardName;
        CardDescription = _card.Description;
        CardName = _card.ID;
    }
}
