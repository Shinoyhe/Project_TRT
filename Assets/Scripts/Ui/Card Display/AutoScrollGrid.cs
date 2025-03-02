using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AutoScrollGrid : MonoBehaviour {

    #region ======== [ OBJECT REFERENCES ] ========

    public GameObject gridContentObject;
    public GridLayoutGroup gridLayout;

    #endregion

    #region ======== [ PRIVATE PROPERTIES ] ========

    private int _itemsPerRow;
    private float _cardHeight;
    private float _cardOffset;
    private RectTransform _contentTransform;

    #endregion

    #region ======== [ INIT METHODS ] ========

    private void Start() {
        _itemsPerRow = gridLayout.constraintCount;
        _cardHeight = gridLayout.cellSize.y;
        _cardOffset = gridLayout.padding.bottom;

        _contentTransform = gridContentObject.GetComponent<RectTransform>();
    }

    #endregion


    #region ======== [ PUBLIC METHODS ] ========

    /// <summary>
    /// Takes a card index in a GridLayout and positions grid to not crop out card.
    /// </summary>
    /// <param name="indexOfCard"> Cards index in Row Major order </param>
    public void FrameCardInGrid(int indexOfCard) {
        
        // Get card information
        int cardRow = indexOfCard / _itemsPerRow;

        float cardLowerBound = cardRow * (_cardHeight + _cardOffset);
        float cardUpperBound = (cardRow + 1) * (_cardHeight + _cardOffset);

        float contentHeightOffset = gridContentObject.GetComponent<RectTransform>().anchoredPosition.y;

        // If card out of content canvas, call moveGrid
        if (contentHeightOffset > cardLowerBound) {
            MoveGrid(cardLowerBound);
        }

        if (contentHeightOffset < cardUpperBound) {
            MoveGrid(cardLowerBound);
        }
    }

    #endregion

    #region ======== [ PRIVATE METHODS ] ========

    /// <summary>
    /// Start a DOTween animation to move content grid to a given pos.
    /// </summary>
    /// <param name="yPos"></param>
    private void MoveGrid(float yPos) {
        float x = _contentTransform.anchoredPosition.x;
        _contentTransform.DOAnchorPos(new Vector2(x, yPos), 1f, false);

    }

    #endregion
}
