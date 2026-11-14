using UnityEngine;
using TMPro;

public class Speedometer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody[] carRigidbodies; 
    [SerializeField] private TMP_Text speedText;

    [Header("Settings")]
    [SerializeField] private bool useForwardSpeedOnly = true;
    [SerializeField] private string unitLabel = " km/h";

    private const float MS_TO_KMH = 3.6f;

    private void Update()
    {
        if (speedText == null || carRigidbodies == null) return;

        Rigidbody activeCar = GetActiveCar();

        if (activeCar == null)
        {
            speedText.text = "0" + unitLabel;
            return;
        }

        // Calculate speed in meters per second
        float speedInMS = useForwardSpeedOnly
            ? Mathf.Abs(Vector3.Dot(activeCar.linearVelocity, activeCar.transform.forward))
            : activeCar.linearVelocity.magnitude;

        // Convert to km/h and round to a whole number
        int speedInKMH = Mathf.RoundToInt(speedInMS * MS_TO_KMH);

        speedText.text = speedInKMH + unitLabel;
    }

    private Rigidbody GetActiveCar()
    {
        for (int i = 0; i < carRigidbodies.Length; i++)
        {
            if (carRigidbodies[i] != null && carRigidbodies[i].gameObject.activeInHierarchy)
            {
                return carRigidbodies[i];
            }
        }
        return null;
    }
}