using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target; // The target to follow
    [SerializeField] private float smoothSpeed = 8f; // Speed of the camera smoothing
    [SerializeField] private Vector3 offset; // Offset from the target position

    private void LateUpdate()
    {
        if (target == null) 
            return;
        
        Vector3 desiredPosition = target.position + offset;
        
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        
        transform.position = smoothedPosition;
    }
}
