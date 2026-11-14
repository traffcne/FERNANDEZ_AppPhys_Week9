using UnityEngine;

public class SimpleWheelRotate : MonoBehaviour
{
    public Rigidbody carRigidbody;

    [Header("Wheel Transforms")]
    public Transform[] frontWheels; 
    public Transform[] rearWheels;  

    [Header("Settings")]
    public float wheelRadius = 0.35f;
    public float maxSteerAngle = 30f;
    public float steerSpeed = 10f; 

    private float currentRollAngle = 0f;
    private float currentSteerAngle = 0f;

    private void Update()
    {
        // 1. Calculates forward speed and rolling rotation.
        float forwardSpeed = Vector3.Dot(carRigidbody.linearVelocity, transform.forward);
        float rotationDelta = (forwardSpeed / wheelRadius) * Mathf.Rad2Deg * Time.deltaTime;
        
        // Accumulates the roll angle and keeps it within 0-360 degrees
        currentRollAngle = (currentRollAngle + rotationDelta) % 360f;

        // 2. Get steering input and smoothly interpolate the steering angle.
        float steerInput = Input.GetAxis("Horizontal"); 
        float targetSteerAngle = steerInput * maxSteerAngle;
        currentSteerAngle = Mathf.Lerp(currentSteerAngle, targetSteerAngle, Time.deltaTime * steerSpeed);

        // 3. Apply both steering (Y) and rolling (X) to the front wheels.
        foreach (Transform wheel in frontWheels)
        {
            wheel.localRotation = Quaternion.Euler(currentRollAngle, currentSteerAngle, 0f);
        }

        // 4. Applies only rolling (X) to the rear wheels.
        foreach (Transform wheel in rearWheels)
        {
            wheel.localRotation = Quaternion.Euler(currentRollAngle, 0f, 0f);
        }
    }
}