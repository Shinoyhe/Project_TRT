using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoScrollGrid : MonoBehaviour
{
    public GameObject gridContentObject;
    public GridLayoutGroup gridLayout;

    private int _itemsPerRow;
    private float _cardHeight;
    private float _cardOffset;

    /*
     * Goal:
     * - On card creation in UICore pass a index
     * - When a card is selected it calls a autoAdjust(index)
     * - CardsPerRow = known # of items per row
     * - RowIndex = Floor(Index / RowCount)
     * - CardHeight = RowIndex * 310;
     * - ContentHeightOffset = Y of gridContentObject
     * - If ContentHeightOffset > CardHeight -> Then card is out of frame
     * - Set Y of gridContentObject to CardHeight
     */

    private void Start() {
        _itemsPerRow = gridLayout.constraintCount;
        _cardHeight = gridLayout.preferredHeight;
        _cardOffset = gridLayout.padding.bottom;
    }

    public void FrameCardInGrid(int indexOfCard) {

        int cardIndex = indexOfCard / _itemsPerRow;

        float cardHeight = cardIndex * (_cardHeight + _cardOffset);

        float contentHeightOffset = gridContentObject.GetComponent<RectTransform>().anchoredPosition.y;

        if(contentHeightOffset > cardHeight) {
            float contentX = gridContentObject.GetComponent<RectTransform>().anchoredPosition.x;
            gridContentObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(contentX,cardHeight);
        }

    }

}
