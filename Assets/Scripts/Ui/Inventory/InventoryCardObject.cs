using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryCardObject : MonoBehaviour, ISelectHandler {
    #region ======== [ OBJECT REFERENCES ] ========
    [Header("Data")]
    [SerializeField] private InventoryCardData _card;

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

    [HideInInspector] public InventoryUiCore Parent;

    private bool inspected = false;
    private int index;
    private AutoScrollGrid scroller;
    private Navigation None;
    private Navigation All;

    #endregion

    #region ======== [ INIT METHODS ] ========

    // Start is called before the first frame update
    void Start() {

        // Create Navigation instances
        None = new Navigation();
        None.mode = Navigation.Mode.None;

        All = new Navigation();
        All.mode = Navigation.Mode.Automatic;

        if (_card != null) {
            SetData(_card);
        } else {
            SetCardToEmpty();
        }
    }

    private void OnDisable() {
        gameObject.GetComponent<Button>().navigation = All;
        inspected = false;
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
        
        _card = newCard;
        backCardImage.color = Color.white;

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

    public void SetIndex(int index) {
        this.index = index;
    }

    public void SetScroller(AutoScrollGrid scroller) {
        this.scroller = scroller;
    }

    /// <summary>
    /// Sets card to empty!
    /// </summary>
    public void SetCardToEmpty() {
        itemLayoutObject.SetActive(false);
        infoLayoutObject.SetActive(false);

        backCardImage.color = Color.gray;
    }
    public void OnSelect(BaseEventData eventData) {
        if (scroller != null) {
            scroller.FrameCardInGrid(index);
        } else {
            Debug.Log("Scroller is found to be null!");
        }
    }
    public void OnSelected() {

        if (_card == null) return;

        if (inspected == false) {

            // Show card
            Parent.InspectCard(_card);

            // Disable navigation
            gameObject.GetComponent<Button>().navigation = None;
            inspected = true;

        } else {
            
            // Hide card
            Parent.StopInspecting();

            // Enable navigation
            gameObject.GetComponent<Button>().navigation = All;
            inspected = false;
        }

    }

    #endregion
}
