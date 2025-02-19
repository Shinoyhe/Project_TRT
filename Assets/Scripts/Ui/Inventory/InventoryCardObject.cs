using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCardObject : MonoBehaviour
{
    [SerializeField] private InventoryCardData _card;

    [SerializeField, BoxGroup("Item Layout")] private GameObject itemLayoutObject;
    [SerializeField, BoxGroup("Item Layout")] private TMP_Text itemNameText;
    [SerializeField, BoxGroup("Item Layout")] private Image itemSpriteImage;
    [SerializeField, BoxGroup("Item Layout")] private TMP_Text itemDescriptionText;

    [SerializeField, BoxGroup("Info Layout")] private GameObject infoLayoutObject;
    [SerializeField, BoxGroup("Info Layout")] private TMP_Text infoNameText;
    [SerializeField, BoxGroup("Info Layout")] private TMP_Text infoDescriptionText;
    [SerializeField, BoxGroup("Info Layout")] private Image infoNPC1Sprite;
    [SerializeField, BoxGroup("Info Layout")] private TMP_Text infoNPC1Context;
    [SerializeField, BoxGroup("Info Layout")] private Image infoNPC2Sprite;
    [SerializeField, BoxGroup("Info Layout")] private TMP_Text infoNPC2Context;
    [SerializeField, BoxGroup("Info Layout")] private Image infoNPC3Sprite;
    [SerializeField, BoxGroup("Info Layout")] private TMP_Text infoNPC3Context;

    [HideInInspector] public string CardName;
    [HideInInspector] public string CardDescription;
    [HideInInspector] public string CardID;

    // Start is called before the first frame update
    void Start()
    {
        if (_card != null) {
            SetData(_card);
        }
    }

    public void SetData(InventoryCardData newCard)
    {
        if (newCard == null) return;
        
        _card = newCard;
        
        if (_card.Type == GameEnums.CardTypes.ITEM) {
            // Disable the info layout and enable the item
            itemLayoutObject.SetActive(true);
            infoLayoutObject.SetActive(false);

            itemNameText.text = _card.CardName;
            itemSpriteImage.sprite = _card.Sprite;
            itemDescriptionText.text = _card.Description;
        } else if (_card.Type == GameEnums.CardTypes.INFO) {
            InventoryCard cardWrapper = GameManager.Inventory.GetCardFromData(_card);

            // disable the item layout and enable the info
            infoLayoutObject.SetActive(true);
            itemLayoutObject.SetActive(false);

            infoNameText.text = _card.CardName;
            infoDescriptionText.text = _card.Description;

            // Ensure all of the contexts are set
            if (cardWrapper.ContextData.Count != System.Enum.GetValues(typeof(GameEnums.ContextOrigins)).Length) {
                Debug.LogError($"InventoryCardObject: SetData: Not all contexts are set in card: {_card.CardName}");
                return;
            }

            // TODO: Set sprites to relevant npc sprites
            // Associate with ContextData[?].origin? it is a GameEnums.ContextOrigins

            // infoNPC1Sprite.sprite = null;
            if (cardWrapper.KnowsContext(cardWrapper.ContextData[0].origin))
            {
                infoNPC1Context.text = cardWrapper.ContextData[0].contextInfo.context;
            } else {
                infoNPC1Context.text = "";
            }

            // infoNPC2Sprite.sprite = null;
            if (cardWrapper.KnowsContext(cardWrapper.ContextData[1].origin))
            {
                infoNPC2Context.text = cardWrapper.ContextData[1].contextInfo.context;
            }
            else
            {
                infoNPC2Context.text = "";
            }

            //infoNPC2Sprite.sprite = null;
            if (cardWrapper.KnowsContext(cardWrapper.ContextData[2].origin))
            {
                infoNPC3Context.text = cardWrapper.ContextData[2].contextInfo.context;
            }
            else
            {
                infoNPC3Context.text = "";
            }
        }

        CardName = _card.CardName;
        CardDescription = _card.Description;
        CardName = _card.ID;
    }
}
