using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public enum BillboardMode { None, FaceTarget, MatchRotation }

    [Header("Parameters")]
    public Transform Target;
    public BillboardMode Mode = BillboardMode.None;
    [SerializeField] private float lerpSpeed = 45f;



    void Start()
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
                UpdateFacingTarget();
                break;
            case BillboardMode.MatchRotation:
                UpdateMatchingRotation();
                break;
        }
    }

    private void UpdateFacingTarget()
    {
        Vector3 direction = Vector3.Scale(Target.position - transform.position, Vector3.right + Vector3.forward);
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lerpSpeed * Time.deltaTime);
    }

    private void UpdateMatchingRotation()
    {
        Debug.Log(Target.rotation.eulerAngles.y);
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, Target.rotation.eulerAngles.y, 0));
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lerpSpeed * Time.deltaTime);
    }
}
