using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryCardObject : MonoBehaviour {
    #region ======== [ OBJECT REFERENCES ] ========

    [Header("Data")]
    public bool IsPreviewCard = false;
    [SerializeField] public InventoryCardData Card;

    [Header("Deactivated Layout")]
    [SerializeField, BoxGroup("Deactive Layout")] private GameObject deactiveObject;
    [SerializeField, BoxGroup("Deactive Layout")] private Button deactiveButton;

    [Header("Deactivated Preview")]
    [SerializeField, BoxGroup("Deactive Layout")] private GameObject deactivePreviewObject;
    [SerializeField, BoxGroup("Deactive Layout")] private Button deactivePreviewButton;
    //test
    [Header("Item Layout")]
    [SerializeField, BoxGroup("Item Layout")] private GameObject itemLayoutObject;
    [SerializeField, BoxGroup("Item Layout")] private Button itemLayoutButton;
    [SerializeField, BoxGroup("Item Layout")] private TMP_Text itemNameText;
    [SerializeField, BoxGroup("Item Layout")] private Image itemSpriteImage;
    [SerializeField, BoxGroup("Item Layout")] private TMP_Text itemDescriptionText;

    [Header("Item Preview Layout")]
    [SerializeField, BoxGroup("Item Preview Layout")] private GameObject itemPreviewLayoutObject;
    [SerializeField, BoxGroup("Item Preview Layout")] private Button itemPreviewLayoutButton;
    [SerializeField, BoxGroup("Item Preview Layout")] private TMP_Text itemPreviewNameText;
    [SerializeField, BoxGroup("Item Preview Layout")] private Image itemPreviewSpriteImage;

    [Header("Info Layout")]
    [SerializeField, BoxGroup("Info Layout")] private GameObject infoLayoutObject;
    [SerializeField, BoxGroup("Info Layout")] private Button infoLayoutButton;
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
    [HideInInspector] public Button CurrentActiveButton;

    private int _index;
    private AutoScrollGrid _scroller;
    private InventoryAction _onSelectAction = null;

    public enum CurrentState {
        DEACTIVE, INFO, ITEM, ITEMPREVIEW, DEACTIVEPREVIEW
    }

    #endregion

    #region ======== [ INIT METHODS ] ========

    // Start is called before the first frame update
    void Start() {

        if (Card != null && IsPreviewCard == false) {
            SetData(Card);
        }
    }

    /// <summary>
    /// Creates an empty inventory card for a InventoryGridController
    /// </summary>
    public void InitalizeToGrid(int indexInGrid, AutoScrollGrid gridAutoScroller, InventoryAction onSelectAction, bool usingPreviewSize) {
        _index = indexInGrid;
        _scroller = gridAutoScroller;
        _onSelectAction = onSelectAction;

        SetCardToEmpty(usingPreviewSize);
    }

    #endregion

    #region ======== [ PUBLIC METHODS ] ========

    /// <summary>
    /// Sets the data of this UI object to the card given
    /// </summary>
    /// <param name="newCard">The cardData to fill</param>
    /// <returns></returns>
    public void SetData(InventoryCardData newCard, bool UseLargeItem = false)
    {
        if (newCard == null) return;
        
        Card = newCard;

        if (Card.Type == GameEnums.CardTypes.ITEM) {
            // Disable the info layout and enable the item

            if (UseLargeItem) {
                SwapState(CurrentState.ITEM);

                itemNameText.text = Card.CardName;
                itemSpriteImage.sprite = Card.Sprite;
                itemDescriptionText.text = Card.Description;
            } else {
                SwapState(CurrentState.ITEMPREVIEW);

                itemPreviewNameText.text = Card.CardName;
                itemPreviewSpriteImage.sprite = Card.Sprite;
            }
        } else if (Card.Type == GameEnums.CardTypes.INFO) {
            InventoryCard cardWrapper = GameManager.Inventory.GetCardFromData(Card);

            // disable the item layout and enable the info
            SwapState(CurrentState.INFO);

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
    public void SetCardToEmpty(bool usingPreviewSize) {

        if (usingPreviewSize) {
            SwapState(CurrentState.DEACTIVEPREVIEW);
        } else {
            SwapState(CurrentState.DEACTIVE);
        }
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
            return;
        }

        InventoryAction.ActionContext ctx = new InventoryAction.ActionContext();
        ctx.cardData = Card;
        _onSelectAction.ActionOnClick(ctx);

    }

    public void SwapState(CurrentState stateToEnter) {

        switch (stateToEnter) {
            case CurrentState.DEACTIVE:
                itemLayoutObject.SetActive(false);
                infoLayoutObject.SetActive(false);
                itemPreviewLayoutObject.SetActive(false);
                deactiveObject.SetActive(true);
                deactivePreviewObject.SetActive(false);

                CurrentActiveButton = deactiveButton;
                break;
            case CurrentState.INFO:
                itemLayoutObject.SetActive(false);
                infoLayoutObject.SetActive(true);
                itemPreviewLayoutObject.SetActive(false);
                deactiveObject.SetActive(false);
                deactivePreviewObject.SetActive(false);


                CurrentActiveButton = infoLayoutButton;
                break;
            case CurrentState.ITEM:
                itemLayoutObject.SetActive(true);
                infoLayoutObject.SetActive(false);
                itemPreviewLayoutObject.SetActive(false);
                deactiveObject.SetActive(false);
                deactivePreviewObject.SetActive(false);


                CurrentActiveButton = itemLayoutButton;
                break;
            case CurrentState.ITEMPREVIEW:
                itemLayoutObject.SetActive(false);
                infoLayoutObject.SetActive(false);
                itemPreviewLayoutObject.SetActive(true);
                deactiveObject.SetActive(false);
                deactivePreviewObject.SetActive(false);

                CurrentActiveButton = itemPreviewLayoutButton;
                break;
            case CurrentState.DEACTIVEPREVIEW:
                itemLayoutObject.SetActive(false);
                infoLayoutObject.SetActive(false);
                itemPreviewLayoutObject.SetActive(false);
                deactiveObject.SetActive(false);
                deactivePreviewObject.SetActive(true);

                CurrentActiveButton = deactivePreviewButton;
                break;

        }

    }

    #endregion
}
