using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryCardObject : MonoBehaviour, ISelectHandler {
    #region ======== [ OBJECT REFERENCES ] ========

    [Header("Data")]
    [SerializeField] public InventoryCardData Card;

    [Header("Global Dependencies")]
    [SerializeField] private Image backCardImage;

    [Header("Item Layout")]
    [SerializeField, BoxGroup("Item Layout")] private GameObject itemLayoutObject;
    [SerializeField, BoxGroup("Item Layout")] private TMP_Text itemNameText;
    [SerializeField, BoxGroup("Item Layout")] private Image itemSpriteImage;
    [SerializeField, BoxGroup("Item Layout")] private TMP_Text itemDescriptionText;

    [Header("Info Layout")]
    [SerializeField, BoxGroup("Info Layout")] private GameObject infoLayoutObject;
    [SerializeField, BoxGroup("Info Layout")] private TMP_Text infoNameText;
    [SerializeField, BoxGroup("Info Layout")] private TMP_Text infoDescriptionText;
    [SerializeField, BoxGroup("Info Layout")] private Image infoNPC1Sprite;
    [SerializeField, BoxGroup("Info Layout")] private TMP_Text infoNPC1Context;
    [SerializeField, BoxGroup("Info Layout")] private Image infoNPC2Sprite;
    [SerializeField, BoxGroup("Info Layout")] private TMP_Text infoNPC2Context;
    [SerializeField, BoxGroup("Info Layout")] private Image infoNPC3Sprite;
    [SerializeField, BoxGroup("Info Layout")] private TMP_Text infoNPC3Context;

    #endregion

    #region ======== [ INTERNAL PROPERTIES ] ========

    [HideInInspector] public string CardName;
    [HideInInspector] public string CardDescription;
    [HideInInspector] public string CardID;

    private int _index;
    private AutoScrollGrid _scroller;
    private InventoryAction _onSelectAction = null;


    #endregion

    #region ======== [ INIT METHODS ] ========

    // Start is called before the first frame update
    void Start() {

        if (Card != null) {
            SetData(Card);
        } else {
            SetCardToEmpty();
        }
    }

    /// <summary>
    /// Creates an empty inventory card for a InventoryGridController
    /// </summary>
    public void InitalizeToGrid(int indexInGrid, AutoScrollGrid gridAutoScroller, InventoryAction onSelectAction) {
        _index = indexInGrid;
        _scroller = gridAutoScroller;
        _onSelectAction = onSelectAction;

        itemLayoutObject.SetActive(false);
        infoLayoutObject.SetActive(false);

        backCardImage.color = Color.gray;

        SetCardToEmpty();
    }

    #endregion

    #region ======== [ PUBLIC METHODS ] ========

    /// <summary>
    /// Sets the data of this UI object to the card given
    /// </summary>
    /// <param name="newCard">The cardData to fill</param>
    /// <returns></returns>
    public void SetData(InventoryCardData newCard)
    {
        if (newCard == null) return;
        
        Card = newCard;
        backCardImage.color = Color.white;

        if (Card.Type == GameEnums.CardTypes.ITEM) {
            // Disable the info layout and enable the item
            itemLayoutObject.SetActive(true);
            infoLayoutObject.SetActive(false);

            itemNameText.text = Card.CardName;
            itemSpriteImage.sprite = Card.Sprite;
            itemDescriptionText.text = Card.Description;
        } else if (Card.Type == GameEnums.CardTypes.INFO) {
            InventoryCard cardWrapper = GameManager.Inventory.GetCardFromData(Card);

            // disable the item layout and enable the info
            infoLayoutObject.SetActive(true);
            itemLayoutObject.SetActive(false);

            infoNameText.text = Card.CardName;
            infoDescriptionText.text = Card.Description;

            // Ensure all of the contexts are set
            if (cardWrapper.ContextData.Count != System.Enum.GetValues(typeof(GameEnums.ContextOrigins)).Length) {
                Debug.LogError($"InventoryCardObject: SetData: Not all contexts are set in card: {Card.CardName}");
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

        CardName = Card.CardName;
        CardDescription = Card.Description;
        CardName = Card.ID;
    }

    /// <summary>
    /// Sets card to empty!
    /// </summary>
    public void SetCardToEmpty() {
        itemLayoutObject.SetActive(false);
        infoLayoutObject.SetActive(false);

        backCardImage.color = Color.gray;
    }

    /// <summary>
    /// When user hovers over this card.
    /// </summary>
    public void OnSelect(BaseEventData eventData) {
        if (_scroller != null) {
            _scroller.FrameCardInGrid(_index);
        } else {
            Debug.Log("Scroller is found to be null!");
        }
    }

    /// <summary>
    /// When user chooses this card.
    /// </summary>
    public void OnPress() {

        if (_onSelectAction == null) {
            Debug.LogError("Card has no OnSelectAction set!");
            return;
        }

        InventoryAction.ActionContext ctx = new InventoryAction.ActionContext();
        ctx.cardData = Card;
        _onSelectAction.ActionOnClick(ctx);

    }

    #endregion
}
