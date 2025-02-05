using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BarterWillingnessMeter : MonoBehaviour
{
    // Parameters and Publics =====================================================================

    [SerializeField, Tooltip("The BarterDirector that we monitor the Willingness on.")]
    private BarterDirector Director;

    [Header("Bar Images")]
    [SerializeField, Tooltip("The image representing the front layer of the Willingness bar.")]
    private Image BarFront;

    [Header("Level Marker")]
    [SerializeField, Tooltip("The transform that our marker exists under.")]
    private RectTransform MarkerParent;

    [Header("Percentage Label")]
    [SerializeField, Tooltip("The text object used to display our percentage label.")]
    private TMP_Text TextObject;
    [SerializeField, Tooltip("The number of decimal places to display on the percentage.\n\nDefault: 1")]
    private uint DecimalPlaces = 1;
    [SerializeField, Tooltip("A prefix added to the start of our printed string.")]
    private string Prefix;
    [SerializeField, Tooltip("A suffix added to the end of our printed string.")]
    private string Suffix;

    // Misc Internal Variables ====================================================================

    // The horizontal range of our BarFront image, used to position the marker.
    private Vector2 BarFrontBounds;
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
        BarFrontBounds = new(BarFront.rectTransform.offsetMin.x, BarFront.rectTransform.offsetMax.x);

        // Init our format string.
        SetFormatString();
        _lastDecimalPlaces = DecimalPlaces;
    }

    private void SetFormatString()
    {
                            // This string constructor produces '0' repeated DecimalPlaces times.
                            // v
        _formatString = "0." + new string('0', (int)DecimalPlaces);
                     // ^
                     // Adding it to this '0.' makes a format string that, when used on a number,
                     // prints to DecimalPlaces decimal places.
    }

    // Update Methods =============================================================================

    private void Update()
    {
        // Cache Willingness for code length reasons!
        float willingness = Director.GetWillingness();

        // Bar fill
        BarFront.fillAmount = Director.GetWillingness() / 100f;

        // Marker position
        float xPosition = Mathf.Lerp(BarFrontBounds.x, BarFrontBounds.y, willingness/100f);
        MarkerParent.anchoredPosition = new Vector2(xPosition, MarkerParent.anchoredPosition.y);

        // Check if DecimalPlaces has changed. If so, reset the format string.
        if (_lastDecimalPlaces != DecimalPlaces) {
            _lastDecimalPlaces = DecimalPlaces;
            SetFormatString();
        }

        // Percentage label
        TextObject.text = Prefix + willingness.ToString(_formatString) + Suffix;
    }
}