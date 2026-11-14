using UnityEngine;

public class RaceRestartManager : MonoBehaviour
{
    [Header("Car & Timer References")]
    [SerializeField] private Transform[] carTransforms;     
    [SerializeField] private RaceTimer raceTimer;            
    [SerializeField] private GameObject startTriggerObject;   

    [Header("UI Containers")]
    [SerializeField] private GameObject startMenuContainer;   
    [SerializeField] private GameObject hudContainer;         
    [SerializeField] private GameObject endScreenContainer;   

    private Vector3[] initialPositions;
    private Quaternion[] initialRotations;

    private void Awake()
    {
        if (carTransforms == null) return;

        
        initialPositions = new Vector3[carTransforms.Length];
        initialRotations = new Quaternion[carTransforms.Length];

        for (int i = 0; i < carTransforms.Length; i++)
        {
            if (carTransforms[i] != null)
            {
                initialPositions[i] = carTransforms[i].position;
                initialRotations[i] = carTransforms[i].rotation;
            }
        }
    }

   
    public void ReturnToStartMenu()
    {
        // 1. Deactivate the timer trigger object first so the race timer doesn't auto-restart.
        if (startTriggerObject != null)
            startTriggerObject.SetActive(false);

        // 2. Stops and resets the race timer.
        if (raceTimer != null)
        {
            raceTimer.StopTimer();
            raceTimer.ResetTimer();
        }

        // 3. Resets all cars to their starting positions, clears their velocity, and deactivates them.
        if (carTransforms != null)
        {
            for (int i = 0; i < carTransforms.Length; i++)
            {
                Transform car = carTransforms[i];
                if (car == null) continue;

                car.position = initialPositions[i];
                car.rotation = initialRotations[i];

                if (car.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                car.gameObject.SetActive(false);
            }
        }

        // 4. Reset UI states and brings the player back to the car selection menu.
        if (endScreenContainer != null)
            endScreenContainer.SetActive(false);

        if (hudContainer != null)
            hudContainer.SetActive(false);

        if (startMenuContainer != null)
            startMenuContainer.SetActive(true);
    }
}