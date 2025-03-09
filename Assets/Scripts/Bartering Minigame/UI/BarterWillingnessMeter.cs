using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BarterWillingnessMeter : MonoBehaviour
{
    // Parameters and Publics =====================================================================

    [SerializeField, Tooltip("The BarterDirector that we monitor the Willingness on.")]
    private BarterDirector director;

    [Header("Bar Images")]
    [SerializeField, Tooltip("The image representing the front layer of the Willingness bar.")]
    private Image barFront;

    [Header("Level Marker")]
    [SerializeField, Tooltip("The transform that our marker exists under.")]
    private RectTransform markerParent;

    [Header("Percentage Label")]
    [SerializeField, Tooltip("The text object used to display our percentage label.")]
    private TMP_Text textObject;
    [SerializeField, Tooltip("The number of decimal places to display on the percentage.\n\nDefault: 1")]
    private uint decimalPlaces = 1;
    [SerializeField, Tooltip("A prefix added to the start of our printed string.")]
    private string prefix;
    [SerializeField, Tooltip("A suffix added to the end of our printed string.")]
    private string suffix;

    // Misc Internal Variables ====================================================================

    // The horizontal range of our BarFront image, used to position the marker.
    private Vector2 _barFrontBounds;
    // Used to track when DecimalPlaces changes, so that we don't reconstruct the format string
    // every single frame.
    private uint _lastDecimalPlaces;
    // The format string used with our text object!
    private string _formatString;

    // Initializers ===============================================================================

    private void Awake()
    {
        // Calculate the minimum and maximum values of our bounds object.
        // TODO: Test at different resolutions.
        _barFrontBounds = new(barFront.rectTransform.offsetMin.x, barFront.rectTransform.offsetMax.x);

        // Init our format string.
        SetFormatString();
        _lastDecimalPlaces = decimalPlaces;
    }

    private void SetFormatString()
    {
                            // This string constructor produces '0' repeated DecimalPlaces times.
                            // v
        _formatString = "0." + new string('0', (int)decimalPlaces);
                     // ^
                     // Adding it to this '0.' makes a format string that, when used on a number,
                     // prints to DecimalPlaces decimal places.
    }

    // Update Methods =============================================================================

    private void Update()
    {
        // Cache Willingness for code length reasons!
        float willingness = director.GetWillingness();

        // Bar fill
        barFront.fillAmount = director.GetWillingness() / 100f;

        // Marker position
        float xPosition = Mathf.Lerp(_barFrontBounds.x, _barFrontBounds.y, willingness/100f);
        markerParent.anchoredPosition = new Vector2(xPosition, markerParent.anchoredPosition.y);

        // Check if DecimalPlaces has changed. If so, reset the format string.
        if (_lastDecimalPlaces != decimalPlaces) {
            _lastDecimalPlaces = decimalPlaces;
            SetFormatString();
        }

        // Percentage label
        textObject.text = prefix + willingness.ToString(_formatString) + suffix;

        if(willingness > 83) {
            textObject.alignment = TextAlignmentOptions.Left;
        } else {
            textObject.alignment = TextAlignmentOptions.Right;
        }
    }
}