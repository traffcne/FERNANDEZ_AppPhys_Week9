using UnityEngine;
using TMPro;

public class RaceTimer : MonoBehaviour
{
    [Header("Trigger & Car References")]
    [SerializeField] private GameObject startTriggerObject;
    [SerializeField] private Rigidbody[] carRigidbodies;

    [Header("Live UI")]
    [SerializeField] private TMP_Text timerText;

    [Header("Results UI (Optional)")]
    [SerializeField] private TMP_Text finalTimeText;
    [SerializeField] private TMP_Text topSpeedText;

    [Header("Speed Settings")]
    [SerializeField] private bool useForwardSpeedOnly = true;

    private const float MS_TO_KMH = 3.6f;

    private float elapsedTime = 0f;
    private float topSpeedKMH = 0f;
    private bool isRunning = false;
    private bool hasStarted = false;

    public float FinalTime => elapsedTime;
    public float TopSpeedKMH => topSpeedKMH;

    private void Start()
    {
        UpdateTimerDisplay(timerText, 0f);
    }

    private void Update()
    {
        if (startTriggerObject == null || timerText == null) return;

        if (!hasStarted && startTriggerObject.activeInHierarchy)
        {
            StartTimer();
        }

        if (hasStarted && !startTriggerObject.activeInHierarchy)
        {
            ResetTimer();
        }

        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay(timerText, elapsedTime);
            RecordTopSpeed();
        }
    }

    private void RecordTopSpeed()
    {
        Rigidbody activeCar = GetActiveCar();
        if (activeCar == null) return;

        float speedInMS = useForwardSpeedOnly
            ? Mathf.Abs(Vector3.Dot(activeCar.linearVelocity, activeCar.transform.forward))
            : activeCar.linearVelocity.magnitude;

        float currentSpeedKMH = speedInMS * MS_TO_KMH;

        if (currentSpeedKMH > topSpeedKMH)
        {
            topSpeedKMH = currentSpeedKMH;
        }
    }

    public void StartTimer()
    {
        elapsedTime = 0f;
        topSpeedKMH = 0f;
        isRunning = true;
        hasStarted = true;
    }

    public void StopTimer()
    {
        if (!isRunning) return;

        isRunning = false;

        string formattedFinalTime = FormatTime(elapsedTime);
        int roundedTopSpeed = Mathf.RoundToInt(topSpeedKMH);

        if (finalTimeText != null)
            finalTimeText.text = "Final Time: " + formattedFinalTime;

        if (topSpeedText != null)
            topSpeedText.text = "Top Speed: " + roundedTopSpeed + " km/h";

        Debug.Log($"Race Finished! Final Time: {formattedFinalTime} | Top Speed: {roundedTopSpeed} km/h");
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
        topSpeedKMH = 0f;
        isRunning = false;
        hasStarted = false;
        UpdateTimerDisplay(timerText, 0f);
    }

    private Rigidbody GetActiveCar()
    {
        if (carRigidbodies == null) return null;

        for (int i = 0; i < carRigidbodies.Length; i++)
        {
            if (carRigidbodies[i] != null && carRigidbodies[i].gameObject.activeInHierarchy)
            {
                return carRigidbodies[i];
            }
        }
        return null;
    }

    private void UpdateTimerDisplay(TMP_Text targetText, float timeToDisplay)
    {
        if (targetText == null) return;
        targetText.text = FormatTime(timeToDisplay);
    }

    private string FormatTime(float timeToFormat)
    {
        int minutes = Mathf.FloorToInt(timeToFormat / 60f);
        int seconds = Mathf.FloorToInt(timeToFormat % 60f);
        int milliseconds = Mathf.FloorToInt((timeToFormat * 1000f) % 1000f);

        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }
}