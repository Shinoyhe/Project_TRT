using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class PlayerCardSlot : MonoBehaviour 
{
    public DisplayCard Card => _card;
    private RectTransform _rectTransform;
    private DisplayCard _card = null;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public Rect GetWorldRect()
    {
        return RectTransformExtensions.GetWorldRect(_rectTransform);
    }

    public void SetCard(DisplayCard card)
    {
        _card = card;
    }
}
