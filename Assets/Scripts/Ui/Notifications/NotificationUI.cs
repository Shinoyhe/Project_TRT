using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;

public class NotificationUI : MonoBehaviour
{
    #region ======== [ VARIABLES ] ========

    [Header("Object Assignment")]
    [SerializeField] private GameObject notifPrefab;

    [Header("Parameters")]
    [SerializeField, Tooltip("The base position where we spawn notifications at.")] 
    private RectTransform baseCopy;
    [SerializeField, Tooltip("The offset of the notifs at the start of the show animation.")] 
    private Vector2 fadeOffset;
    [SerializeField, Tooltip("The vertical space, in screen units, between notifications.")] 
    private float spaceBetweenNotifs = 70;
    [SerializeField, Tooltip("The amount of time, in seconds, notifications appear for.")]
    private float displayDuration = 5f;
    [SerializeField, Tooltip("The ease type for the show animation.")]
    private Ease showEase;
    [SerializeField, Tooltip("The ease type for the hide animation.")]
    private Ease hideEase;
    [SerializeField, Tooltip("The duration, in seconds, of the show animation.")]
    private float showDuration = 0.25f;
    [SerializeField, Tooltip("The duration, in seconds, of the hide animation.")]
    private float hideDuration = 0.25f;



    private Vector2 _textPosition;
    private PrefabPool _notifPool;
    private Dictionary<GameObject, TextMeshProUGUI> _notifToText;
    private Dictionary<GameObject, float> _notifToDuration;

    #endregion

    #region ======== [ BUILT-IN UNITY METHODS ] ========

    void Start()
    {
        _notifPool = new(notifPrefab, transform, 2);
        _notifToText = new();
        _notifToDuration = new();
    }

    void Update()
    {
        foreach (GameObject notif in _notifToDuration.Keys.ToList()) {
            if (_notifToDuration[notif] <= 0) continue;

            _notifToDuration[notif] -= Time.deltaTime;

            // If the displayTimer concludes,
            // then start hiding the notification
            if (_notifToDuration[notif] <= 0) {
                Hide(notif);
            }
        }
    }

    #endregion

    #region ======== [ PUBLIC METHODS ] ========

    /// <summary>
    /// Spawn a notification with a given message.
    /// </summary>
    /// <param name="message">string - the message to display.</param>
    public void Notify(string message)
    {
        Show(message);
    }

    #endregion

    #region ======== [ PRIVATE METHODS ] ========

    /// <summary>
    /// Start displaying the notification
    /// </summary>
    private void Show(string message)
    {
        if (_notifPool == null) {
            return;
        }

        // Request a notif from our pool.
        GameObject notif = _notifPool.Request();
        if (!_notifToText.ContainsKey(notif)) {
            TextMeshProUGUI notifText = notif.GetComponentInChildren<TextMeshProUGUI>();
            if (notifText == null) {
                Debug.LogError("NotificationUI Error: Show failed. notifPrefab does not have a "
                             + "TextMeshProUGUI component.");
            }

            _notifToText[notif] = notifText;
        }

        // Prepare Showing
        _notifToDuration[notif] = displayDuration + showDuration;
        TextMeshProUGUI text = _notifToText[notif];
        text.DOKill();

        // Set text
        text.text = message;

        // Set position
        float yOffset = spaceBetweenNotifs*(_notifPool.NumActive-1);
        Vector2 mainPosition = (Vector2)baseCopy.localPosition + 
                               new Vector2(_textPosition.x, _textPosition.y+yOffset);
        Vector2 fadePosition = mainPosition + fadeOffset;
        
        // Start Showing the Notification
        text.rectTransform.localPosition = fadePosition;
        text.DOFade(1, showDuration).SetEase(showEase);
        text.rectTransform.DOLocalMove(mainPosition, showDuration).SetEase(showEase);
    }


    /// <summary>
    /// Start hiding the notification
    /// </summary>
    private void Hide(GameObject notif)
    {
        _notifToText[notif].DOFade(0, hideDuration).SetEase(hideEase).onComplete = () => {
            _notifPool.Deactivate(notif);
        };
    }

    #endregion
}
