using System;
using UnityEngine;

public class CarThrust : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer = ~0;

    [SerializeField] private float engineForce;
    [SerializeField] private float turnTorque;
    [SerializeField] private float damping;
    [SerializeField] private float springStrength;
    [SerializeField] private float damperStrength;
    [SerializeField] private float frictionCoefficient;
    [SerializeField] private Transform[] wheelAnchor = Array.Empty<Transform>();

    [SerializeField] private float restLength;
    [SerializeField] private float wheelRadius;


    private const int WHEEL_COUNT = 4;
    private Rigidbody rb;
    private float throttle;
    private float steering;
    private Vector3 thrustForce;
    private Vector3 lastVelocity;
    private Vector3 frictionForce;
    private float measuredAcceleration;

    private float TurnRate => Vector3.Dot(rb.angularVelocity, transform.up);
    private float MaxRayDistance => restLength + wheelRadius;

    private bool grounded;
    private int groundedWheels;
    private float normalForce;
    private readonly bool[] wheelGrounded = new bool[WHEEL_COUNT];
    private readonly float[] hitDistance = new float[WHEEL_COUNT];
    private readonly float[] compression = new float[WHEEL_COUNT];
    private readonly float[] springForce = new float[WHEEL_COUNT];



    public Vector3 Velocity
    {
        get => rb.linearVelocity;
        set => rb.linearVelocity = value;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        for (int i = 0; i < hitDistance.Length; i++)
            hitDistance[i] = MaxRayDistance;

        if (wheelAnchor.Length != WHEEL_COUNT)
        {
            Debug.LogError("Invalid Wheel Count");
            enabled = false;
        }

        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void Update()
    {
        GetInputs();
    }

    private void FixedUpdate()
    {
        MeasureAcceleration();
        SimulateSuspension();
        ApplyFriction();
        ApplyThrust();
        ApplyTorque();
        ApplyTurnDamping();
    }

    private void SimulateSuspension()
    {
        groundedWheels = 0;
        normalForce = 0f;

        for (int i = 0; i < WHEEL_COUNT; i++)
        {
            SimulateWheel(i);
            if (wheelGrounded[i]) groundedWheels++;
            normalForce += springForce[i];
            
        }

        grounded = groundedWheels > 0;
        if (grounded) normalForce = rb.mass * Physics.gravity.magnitude;
    }

    private void SimulateWheel(int i)
{
    var wheel = wheelAnchor[i];
    var up = transform.up;
    springForce[i] = 0;

    var hitGround = Physics.Raycast(wheel.position, -up, out var hit, MaxRayDistance, groundLayer,
        QueryTriggerInteraction.Ignore);

    if (!hitGround)
    {
        wheelGrounded[i] = false;
        hitDistance[i] = MaxRayDistance;
        compression[i] = 0;
        return;
    }

    wheelGrounded[i] = true;
    hitDistance[i] = hit.distance;
    compression[i] = MaxRayDistance - hit.distance;
    
    float cornerSpeed = Vector3.Dot(rb.GetPointVelocity(wheel.position), up);
    float force = springStrength * compression[i] - cornerSpeed * damperStrength;
    springForce[i] = Mathf.Max(0, force);
    
    rb.AddForceAtPosition(up * force, wheel.position);
}


    private void ApplyTurnDamping()
    {
        if (!grounded) return;
        rb.AddTorque(-transform.up * (damping * TurnRate));
    }

    private void ApplyTorque()
    {
        if (!grounded) return;
        rb.AddTorque(transform.up * steering * turnTorque);
    }

    private void ApplyFriction()
    {
        Vector3 slide = Vector3.ProjectOnPlane(Velocity, Vector3.up);

        float speed = slide.magnitude;

        if (!grounded || speed < 0.000001f)
        {
            frictionForce = Vector3.zero;
            return;
        }


        float friction = frictionCoefficient * normalForce;

        float stoppingForce = rb.mass * speed / Time.fixedDeltaTime;
        friction = Mathf.Min(friction, stoppingForce);
        frictionForce = -slide / speed * friction;
        rb.AddForce(frictionForce);


    }

    private void ApplyThrust()
    {
        thrustForce = transform.forward * (throttle * engineForce);
        rb.AddForce(thrustForce);
    }

    private void GetInputs()
    {
        throttle = Input.GetAxisRaw("Vertical");
        steering = Input.GetAxisRaw("Horizontal");
    }

    private void MeasureAcceleration()
    {
        Vector3 velocity = Velocity;
        measuredAcceleration = (velocity - lastVelocity).magnitude / Time.fixedDeltaTime;
        lastVelocity = velocity;
    }


}

