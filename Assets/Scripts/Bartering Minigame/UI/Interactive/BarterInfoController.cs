using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BarterInfoController : MonoBehaviour
{
    [SerializeField, Tooltip("The button used to open the menu.")]
    private Button openMenuButton;
    [SerializeField, Tooltip("The rect transform that holds our info-presenting UI.")]
    private RectTransform infoMenu;

    [Header("'Rounds Left' Label")]
    [SerializeField, Tooltip("The label that displays the number of rounds before we can open the info menu..")]
    private TMP_Text roundsLabel;
    [SerializeField, Tooltip("Text added BEFORE the number of rounds left, written to roundsLabel.")]
    private string unreadyPrefix = "";
    [SerializeField, Tooltip("Text added AFTER the number of rounds left, written to roundsLabel.")]
    private string unreadySuffix = " Turns Left";
    [SerializeField, Tooltip("Text written to roundsLabel when the button is good to go.")]
    private string readyMessage = "<color=#007000>Ready!</color>";

    private void Start()
    {
        SetUnlocked(false);
    }

    public void SetUnlocked(bool unlocked)
    {
        openMenuButton.interactable = unlocked;
    }

    public void SetMenuActive(bool menuActive)
    {
        infoMenu.gameObject.SetActive(menuActive);
    }

    public void SetUIRounds(int rounds)
    {
        if (rounds == 0) {
            roundsLabel.text = readyMessage;
        } else {
            roundsLabel.text = unreadyPrefix + rounds + unreadySuffix;
        }
    }
}