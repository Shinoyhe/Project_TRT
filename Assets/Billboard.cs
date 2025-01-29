using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] Transform pivotCamera;

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = Vector3.Scale(pivotCamera.position - transform.position, Vector3.right + Vector3.forward);
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 45f * Time.deltaTime);
    }
}
