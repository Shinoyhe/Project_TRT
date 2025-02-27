using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarterStarter : MonoBehaviour
{
    // Parameters =================================================================================
    [SerializeField] private GameObject barterContainerPrefab;

    [BoxGroup("Barter Settings"), ReadOnly] public BarterResponseMatrix BarterResponseMatrix;
    [BoxGroup("Barter Settings"), ReadOnly] public BarterNeutralBehavior BarterNeutralBehaviour;
    [BoxGroup("Barter Settings"), ReadOnly] public InventoryCardData PrizeCard;
    [BoxGroup("Barter Settings"), ReadOnly] public float DecayPerSecond = 5;
    [BoxGroup("Barter Settings"), ReadOnly] public float WillingnessPerMatch = 5;
    [BoxGroup("Barter Settings"), ReadOnly] public float WillingnessPerFail = -5;
    [BoxGroup("Barter Settings"), ReadOnly] public float StartingWillingness = 50;

    // Misc Internal Variables ====================================================================
    private GameObject _barterInstance;

    // Public Functions ===========================================================================

    /// <summary>
    /// Starts the Barter
    /// </summary>
    /// <returns>The created barter instance GameObject, containing</returns>
    public GameObject StartBarter()
    {
        _barterInstance = Instantiate(barterContainerPrefab, Vector3.zero, Quaternion.identity,
                                      GameManager.MasterCanvas.transform);

        BarterDirector _barterDirector = _barterInstance.GetComponentInChildren<BarterDirector>();
        _barterDirector.OnWin += WinBarter;
        _barterDirector.OnLose += LoseBarter;

        // Set up the BarterDirector settings
        _barterDirector.InitializeWillingness(StartingWillingness);

        if (BarterResponseMatrix != null) { 
            _barterDirector.BarterResponses = BarterResponseMatrix;
        } else
        {
            Debug.LogError("BarterStarter: Could not find BarterResponseMatrix");
        }

        if (BarterNeutralBehaviour != null)
        {
            _barterDirector.NeutralBehavior = BarterNeutralBehaviour;
        }
        else
        {
            Debug.LogError("BarterStarter: Could not find BarterNeutralBehaviour");
        }

        _barterDirector.DecayPerSecond = DecayPerSecond;
        _barterDirector.WillingnessPerMatch = WillingnessPerMatch;
        _barterDirector.WillingnessPerFail = WillingnessPerFail;

        GameManager.PlayerInput.IsActive = false;
        //MusicManager.play

        return _barterInstance;
    }

    // Private Methods ============================================================================

    private void Start()
    {

    }

    private void Update()
    {
        
    }

    /// <summary>
    /// Call on Barter Win, give player card.
    /// </summary>
    void WinBarter()
    {
        if (PrizeCard != null)
        {
            GameManager.Inventory.AddCard(PrizeCard);
        }
        CleanupBarter();
    }

    /// <summary>
    /// Call on Barter lose, just cleans up.
    /// </summary>
    void LoseBarter()
    {
        CleanupBarter();
    }

    /// <summary>
    /// Handles cleanup of barter minigame.
    /// </summary>
    void CleanupBarter()
    {
        Destroy(_barterInstance);
        GameManager.PlayerInput.IsActive = true;
    }
}
