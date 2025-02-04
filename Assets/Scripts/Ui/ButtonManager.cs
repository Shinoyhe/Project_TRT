using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private Button _buttonComponent;
    [SerializeField] private TMP_Text _buttonText;
    [SerializeField] private RectTransform _rectTransform;

    public void SetText(string text) {
        _buttonText.text = text;
    }

    public void SetLocalPos(Vector3 pos) {
        _rectTransform.localPosition = pos;
    }

    public void SetOnClick(UnityAction call) {
        _buttonComponent.onClick.AddListener(call);
    }

}
