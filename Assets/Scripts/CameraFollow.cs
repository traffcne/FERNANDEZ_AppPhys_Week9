using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] public Transform target;

    [SerializeField]private float distance = 7;
    [SerializeField]private float height = 2.5f;
    [SerializeField]private float lookHeight = 1;

    [SerializeField] private float positionSmoothTime = .15f;

    [SerializeField] private float rotationSharpness;

    private Vector3 lastFlatForward;
    private Vector3 currentCameraVelocity;

    private void Start()
    {
        if (target == null) return;
        transform.position = DesiredPosition();
        transform.rotation = DesiredRotation();
    }

    private void LateUpdate()
    {
        if (target == null) return;
        
        transform.position = Vector3.SmoothDamp(transform.position, DesiredPosition(), ref currentCameraVelocity, positionSmoothTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, DesiredRotation(), SmoothFactor(rotationSharpness));
    }

    private Quaternion DesiredRotation()
    {
        Vector3 lookPoint = target.position + Vector3.up * lookHeight;
        
        return Quaternion.LookRotation(lookPoint - transform.position, Vector3.up);
    }


    private Vector3 DesiredPosition()
    {
        return target.position - FlatForward() * distance + height * Vector3.up;
    }

    private Vector3 FlatForward()
    {
        Vector3 flat = Vector3.ProjectOnPlane(target.forward, Vector3.up);
        
        if(flat.sqrMagnitude > .001f) lastFlatForward = flat.normalized;
        
        return lastFlatForward;
    }

    private float SmoothFactor(float rotationSmoothness)
    {
        return 1f - Mathf.Exp(-rotationSmoothness * Time.deltaTime);
    }
}
