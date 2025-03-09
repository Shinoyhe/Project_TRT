using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NavBarController : MonoBehaviour {
    #region ======== [ OBJECT REFERENCES ] ========

    [Header("Dependencies")]
    public List<Image> ButtonOutlines;
    public List<TMP_Text> ButtonTexts;
    public InGameUi InGameUi;

    #endregion

    #region ======== [ PARAMETERS ] ========

    public Color SelectedTint = Color.grey;

    #endregion

    #region ======== [ PRIVATE PROPERTIES ] ========

    private int _index = 0;

    #endregion

    #region ======== [ PUBLIC METHODS ] ========

    public void InitNavBar(int index) {
        // Wrap index around if needed
        index = index % ButtonOutlines.Count;
        if (index < 0) index += ButtonOutlines.Count;

        ButtonOutlines[_index].color = Color.white;
        ButtonOutlines[index].color = SelectedTint;
        ButtonTexts[_index].fontStyle = FontStyles.Normal;
        ButtonTexts[index].fontStyle = FontStyles.UpperCase;

        // Set new index
        _index = index;
    }

    #endregion

    #region ======== [ PRIVATE METHODS ] ========

    private void SetNavBarSelection(int index) {

        if (index < 0 || index >= ButtonOutlines.Count) return;

        // Flip selection
        ButtonOutlines[_index].color = Color.white;
        ButtonOutlines[index].color = SelectedTint;

        // This is hard coding (Better solutions exist I promise)
        switch (index) {

            case 0:
                InGameUi.MoveToJournal();
                break;
            case 1:
                InGameUi.MoveToInventory();
                break;
            case 2:
                InGameUi.MoveToPause();
                break;

        }

        // Set new index
        _index = index;
    }

    #endregion
}
