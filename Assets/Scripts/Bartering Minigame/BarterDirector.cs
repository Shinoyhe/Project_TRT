using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarterDirector : MonoBehaviour
{
    // Parameters =================================================================================

    [SerializeField, Tooltip("The card user used by the player.")]
    private CardUser PlayerCardUser;
    [SerializeField, Tooltip("The card user used by the opposing NPC.")]
    private CardUser OppCardUser;
    [SerializeField, Tooltip("Whether or not we should print debug messages.")]
    private bool DebugMode = false;

    // Misc Internal Variables ====================================================================

    private BarterStateMachine _machine = null;
    private bool _lastDebugMode = false;

    // Initializers ===============================================================================

    private void Start()
    {
        _machine = new(this, PlayerCardUser, OppCardUser);
        _machine.StartMachine();
    }

    // Initializers ===============================================================================

    private void Update()
    {
        _machine.UpdateState();

        if (DebugMode != _lastDebugMode) {
            _lastDebugMode = DebugMode;
            _machine.SetDebug(DebugMode);
        }
    }
}
