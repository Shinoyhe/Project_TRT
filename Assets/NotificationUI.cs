using UnityEngine;
using TMPro;
using DG.Tweening;

public class NotificationUI : MonoBehaviour
{
    #region ======== [ VARIABLES ] ========

    [Header("Object Assignment")]
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private Vector2 startOffset;
    [SerializeField] private float displayDuration = 5f;
    [SerializeField] private Ease showEase;
    [SerializeField] private Ease hideEase;
    [SerializeField] private float showDuration = 0.25f;
    [SerializeField] private float hideDuration = 0.25f;

    private Vector2 _textPosition;
    private float _displayTimer;

    #endregion

    #region ======== [ PUBLIC METHODS ] ========

    public void Notify(string message)
    {
        Show();
        notificationText.text = message;
    }

    #endregion

    #region ======== [ PRIVATE METHODS ] ========

    /// <summary>
    /// Start displaying the notification
    /// </summary>
    private void Show()
    {
        // Prepare Showing
        notificationText.DOKill();
        _displayTimer = displayDuration + showDuration;

        // Start Showing the Notification
        notificationText.rectTransform.localPosition = _textPosition + startOffset;
        notificationText.DOFade(1, showDuration).SetEase(showEase);
        notificationText.rectTransform.DOLocalMove(_textPosition, showDuration)
            .SetEase(showEase);
    }


    /// <summary>
    /// Start hiding the notification
    /// </summary>
    private void Hide()
    {
        notificationText.DOFade(0, hideDuration).SetEase(hideEase);
    }

    #endregion

    #region ======== [ BUILT-IN UNITY METHODS ] ========

    void Start()
    {
        _textPosition = notificationText.rectTransform.localPosition;
    }

    void Update()
    {
        if (displayDuration <= 0) return;

        _displayTimer -= Time.deltaTime;

        // If the displayTimer concludes,
        // then start hiding the notification
        if (_displayTimer <= 0)
        {
            Hide();
        }
    }

    #endregion
}
