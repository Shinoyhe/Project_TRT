using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public enum BillboardMode { None, FaceTarget, MatchRotation }

    [Header("Parameters")]
    public Transform Target;
    public BillboardMode Mode = BillboardMode.None;
    public bool FlipOrientation = false;
    [SerializeField] private float lerpSpeed = 3f;

    private Quaternion _targetRotation;

    public void Flip()
    {
        FlipOrientation = !FlipOrientation;
    }


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
}
