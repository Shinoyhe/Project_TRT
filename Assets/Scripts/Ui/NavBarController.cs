using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavBarController : MonoBehaviour
{
    #region ======== [ OBJECT REFERENCES ] ========

    [Header("Dependencies")]
    public List<Image> Nodes;

    #endregion

    #region ======== [ PARAMETERS ] ========

    public Color SelectedTint = Color.grey;
    public float DeadZoneTillInput = 0.25f;
    public float DelayBetweenControllerInputs = 0.125f;

    #endregion

    #region ======== [ PRIVATE PROPERTIES ] ========

    private int _index;
    private bool _readyToMove = true;

    #endregion

    #region ======== [ PUBLIC METHODS ] ========

    public void Update() {

        Vector3 input = GameManager.PlayerInput.GetControlInput();

        if(input == Vector3.zero) {
            _readyToMove = true;
        }

        if (_readyToMove == false) return;

        int directionToMove = (int) Mathf.Sign(input.x);

        if(Mathf.Abs(input.x) >= DeadZoneTillInput) {
            SetNavBarSelection(_index + 1 * directionToMove);
            _readyToMove = false;
            StartCoroutine("DelayInput");
        }
    }

    public void SetNavBarSelection(int index) {

        // Wrap index around if needed
        index = index % Nodes.Count;
        if (index < 0) index += Nodes.Count;

        // Flip selection
        Nodes[_index].color = Color.white;
        Nodes[index].color = SelectedTint;

        // Set new index
        _index = index;      
    }

    #endregion

    #region ======== [ PRIVATE METHODS ] ========

    IEnumerator DelayInput() {
        yield return new WaitForSeconds(DelayBetweenControllerInputs);
        _readyToMove = true;
    }

    #endregion
}
