using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavBarController : MonoBehaviour {
    #region ======== [ OBJECT REFERENCES ] ========

    [Header("Dependencies")]
    public List<Image> Nodes;
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
        index = index % Nodes.Count;
        if (index < 0) index += Nodes.Count;

        Nodes[_index].color = Color.white;
        Nodes[index].color = SelectedTint;

        // Set new index
        _index = index;
    }

    #endregion

    #region ======== [ PRIVATE METHODS ] ========

    private void SetNavBarSelection(int index) {

        if (index < 0 || index >= Nodes.Count) return;

        // Flip selection
        Nodes[_index].color = Color.white;
        Nodes[index].color = SelectedTint;

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
