using UnityEngine;

public class SimpleWheelRotate : MonoBehaviour
{
    public Rigidbody carRigidbody;
    public Transform[] wheelAnchor;
    public float wheelRadius = 0.35f; // Adjust to match your wheel size

    private void Update()
    {
        // Get the speed of the car along its forward axis
        float forwardSpeed = Vector3.Dot(carRigidbody.linearVelocity, transform.forward);
        
        // Calculate angular distance: distance / radius (in radians, then converted to degrees)
        float rotationAngle = (forwardSpeed / wheelRadius) * Mathf.Rad2Deg * Time.deltaTime;

        foreach (Transform wheel in wheelAnchor)
        {
            wheel.Rotate(rotationAngle, 0f, 0f, Space.Self);
        }
    }
}