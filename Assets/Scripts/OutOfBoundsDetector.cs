using UnityEngine;

public class OutOfBoundsDetector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform[] carTransforms;
    [SerializeField] private Transform respawnPoint;

    [Header("Track Safe-Zone Dimensions (World Units)")]
    [SerializeField] private Vector3 centerOffset = Vector3.zero;
    [SerializeField] private Vector3 safeZoneSize = new Vector3(100f, 30f, 100f);

    private Matrix4x4 UnscaledMatrix => Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

    private void Update()
    {
        if (carTransforms == null) return;

        for (int i = 0; i < carTransforms.Length; i++)
        {
            Transform car = carTransforms[i];

            if (car == null || !car.gameObject.activeInHierarchy) continue;

            Vector3 localPos = UnscaledMatrix.inverse.MultiplyPoint3x4(car.position) - centerOffset;

            if (!IsInsideSafeZone(localPos))
            {
                OnCarOutOfBounds(car);
            }
        }
    }

    private bool IsInsideSafeZone(Vector3 localPos)
    {
        Vector3 halfSize = safeZoneSize * 0.5f;

        bool withinX = Mathf.Abs(localPos.x) <= halfSize.x;
        bool withinY = Mathf.Abs(localPos.y) <= halfSize.y;
        bool withinZ = Mathf.Abs(localPos.z) <= halfSize.z;

        return withinX && withinY && withinZ;
    }

    private void OnCarOutOfBounds(Transform car)
    {
        Debug.Log($"{car.name} went out of bounds!");

        if (respawnPoint != null)
        {
            car.position = respawnPoint.position;
            car.rotation = respawnPoint.rotation;

            if (car.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = UnscaledMatrix;
        Gizmos.DrawWireCube(centerOffset, safeZoneSize);
    }
}