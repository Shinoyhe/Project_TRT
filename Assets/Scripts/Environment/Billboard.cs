using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    #region ======== [ ENUMS ] ========
    public enum BillboardMode { None, FaceTarget, MatchRotation }
    #endregion

    #region ======== [ PARAMETERS ] ========

    [Header("Parameters")]
    [InfoBox("The camera will be the automatic target if not assigned. [Recommended]")]
    [Tooltip("Controls where the billboard faces. Leaving it empty will default to a player camera.")] public Transform Target;
    [Tooltip("Determines how the billboard follows the target.\n\n" +
        "- None: Billboard is inactive.\n" +
        "- Face Target [Recommended]: Billboard will face towards the target.\n" +
        "- Match Rotation: Billboard will match the rotation of the target.")]
    public BillboardMode Mode = BillboardMode.FaceTarget;
    [Tooltip("Flips which way is side the target")] public bool FlipOrientation = false;
    [SerializeField] private float lerpSpeed = 3f;

    #endregion

    #region ======== [ PRIVATE PROPERTY ] ========
    private Quaternion _targetRotation;
    // yeah that it's it lol
    #endregion

    #region ======== [ PUBLIC METHOD ] ========

    /// <summary>
    /// Toggles "FlipOrientation", which flips which side is facing the target
    /// </summary>
    public void Flip()
    {
        FlipOrientation = !FlipOrientation;
    }

    #endregion

    #region ======== [ PRIVATE METHOD ] ========
    void Start()
    {
        if (Target == null)
        {
            Target = Player.PivotCamera;
        }
    }


    void Reset()
    {
        if (Target == null)
        {
            Target = Player.PivotCamera;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Target == null) return;

        switch (Mode)
        {
            case BillboardMode.FaceTarget:
                _targetRotation = UpdateFacingTarget();
                break;
            case BillboardMode.MatchRotation:
                _targetRotation = UpdateMatchingRotation();
                break;
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, lerpSpeed * Time.deltaTime);
    }


    private Quaternion UpdateFacingTarget()
    {
        Vector3 direction = Vector3.Scale(Target.position - transform.position, Vector3.right + Vector3.forward);
        if (FlipOrientation)
        {
            direction *= -1;
        }
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        return targetRotation;
    }


    private Quaternion UpdateMatchingRotation()
    {
        float y = Target.rotation.eulerAngles.y;
        y += FlipOrientation ? 180 : 0;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, y, 0));

        return targetRotation;
    }

    #endregion
}
