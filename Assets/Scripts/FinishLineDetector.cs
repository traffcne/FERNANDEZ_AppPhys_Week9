using UnityEngine;
using UnityEngine.UI; 
using TMPro;          

public class FinishLineDetector : MonoBehaviour
{
    [Header("Car & Timer References")]
    [SerializeField] private Transform[] carTransforms; 
    [SerializeField] private RaceTimer raceTimer;         

    [Header("UI Containers")]
    [SerializeField] private GameObject hudContainer;       
    [SerializeField] private GameObject endScreenContainer; 

    [Header("End Screen Results UI")]
    [SerializeField] private TMP_Text finalTimeText;        
    [SerializeField] private TMP_Text topSpeedText;         
    [SerializeField] private Image carDisplayImage;         
    [SerializeField] private Sprite[] carSprites;           

    [Header("Arch Opening Dimensions (World Units)")]
    [SerializeField] private Vector3 centerOffset = Vector3.zero;
    [SerializeField] private float archWidth = 6f;
    [SerializeField] private float archHeight = 3f;

    private Vector3[] previousLocalPositions;
    private bool[] hasInitialized;

    private Matrix4x4 UnscaledMatrix => Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

    private void Awake()
    {
        if (carTransforms == null) return;

        previousLocalPositions = new Vector3[carTransforms.Length];
        hasInitialized = new bool[carTransforms.Length];
    }

    private void Update()
    {
        if (carTransforms == null) return;

        for (int i = 0; i < carTransforms.Length; i++)
        {
            Transform car = carTransforms[i];

            if (car == null || !car.gameObject.activeInHierarchy)
            {
                hasInitialized[i] = false;
                continue;
            }

            Vector3 currentLocalPos = UnscaledMatrix.inverse.MultiplyPoint3x4(car.position) - centerOffset;

            if (!hasInitialized[i])
            {
                previousLocalPositions[i] = currentLocalPos;
                hasInitialized[i] = true;
                continue;
            }

            bool crossedPlane = (previousLocalPositions[i].z < 0f && currentLocalPos.z >= 0f) ||
                                (previousLocalPositions[i].z > 0f && currentLocalPos.z <= 0f);

            if (crossedPlane && IsWithinArchBounds(currentLocalPos))
            {
                OnFinishLineCrossed(i);
            }

            previousLocalPositions[i] = currentLocalPos;
        }
    }

    private bool IsWithinArchBounds(Vector3 localPos)
    {
        bool withinX = Mathf.Abs(localPos.x) <= archWidth * 0.5f;
        bool withinY = Mathf.Abs(localPos.y) <= archHeight * 0.5f;

        return withinX && withinY;
    }

    private void OnFinishLineCrossed(int carIndex)
    {
        Transform finishedCar = carTransforms[carIndex];

        // 1. Captures and assigns both the Final Time and the Top Speed BEFORE resetting the timer.
        if (raceTimer != null)
        {
            if (finalTimeText != null)
                finalTimeText.text = FormatTime(raceTimer.FinalTime);

            if (topSpeedText != null)
                topSpeedText.text = Mathf.RoundToInt(raceTimer.TopSpeedKMH) + " km/h";

            // Stops and resets the timer
            raceTimer.StopTimer();
            raceTimer.ResetTimer();
        }

        // 2. Disables the active HUD containing the Timer & Speedometer.
        if (hudContainer != null)
            hudContainer.SetActive(false);

        // 3. Assigns the matching car Sprite using the exact same array index.
        if (carDisplayImage != null && carSprites != null && carIndex < carSprites.Length)
        {
            carDisplayImage.sprite = carSprites[carIndex];
        }

        // 4. Deactivates the finished car.
        if (finishedCar != null)
            finishedCar.gameObject.SetActive(false);

        // 5. Enables the End Screen parent (which activates all its enabled children).
        if (endScreenContainer != null)
            endScreenContainer.SetActive(true);

        Debug.Log($"{finishedCar.name} (Index {carIndex}) finished the race!");
    }

    private string FormatTime(float timeToFormat)
    {
        int minutes = Mathf.FloorToInt(timeToFormat / 60f);
        int seconds = Mathf.FloorToInt(timeToFormat % 60f);
        int milliseconds = Mathf.FloorToInt((timeToFormat * 1000f) % 1000f);

        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.matrix = UnscaledMatrix;

        Vector3 size = new Vector3(archWidth, archHeight, 0.05f);
        Gizmos.DrawWireCube(centerOffset, size);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(centerOffset, Vector3.forward * 2f);
    }
}