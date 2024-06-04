using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Control Settings")]
    [Tooltip("There are values for mobile")]
    [SerializeField] public float panSpeed = 20f;
    [SerializeField] public float panBorderThickness = 10f;
    [SerializeField] public Vector3 panLimit;
    [SerializeField] public float scrollSpeed = 20f;
    [SerializeField] public float minY = 20f;
    [SerializeField] public float maxY = 120f;
    [SerializeField] public float smoothSpeed = 5f;

    private Vector3 touchStart;
    private float initialPinchDistance;
    private float initialCameraHeight;
    private bool isPanning = false; // To avoid giving direction to the character while moving the camera


    private void Update()
    {
#if UNITY_EDITOR
        ControlCamera();
#endif
    }

    private void FixedUpdate()  // Due to the fps problem in Android, camera movement works more smoothly in fixedupdate
    {
#if !UNITY_EDITOR 
        ControlCamera();

#endif
    }

    void ControlCamera()
    {
        Vector3 pos = transform.position;

        // One-finger camera movement
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStart = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.transform.position.y));
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                isPanning = true;
                Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.transform.position.y));
                pos.x += direction.x * panSpeed * Time.fixedDeltaTime;
                pos.z += direction.z * panSpeed * Time.fixedDeltaTime;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                Invoke(nameof(EndOfPaning), 0.15f);
            }
        }
        // Two-finger zoom gesture
        else if (Input.touchCount == 2)
        {
            isPanning = true;
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            if (touchZero.phase == TouchPhase.Began || touchOne.phase == TouchPhase.Began)
            {
                initialPinchDistance = Vector2.Distance(touchZero.position, touchOne.position);
                initialCameraHeight = pos.y;
            }
            else if (touchZero.phase == TouchPhase.Moved || touchOne.phase == TouchPhase.Moved)
            {
                float currentPinchDistance = Vector2.Distance(touchZero.position, touchOne.position);
                float pinchDifference = initialPinchDistance - currentPinchDistance;
                float targetHeight = initialCameraHeight + pinchDifference * scrollSpeed * Time.fixedDeltaTime;
                targetHeight = Mathf.Clamp(targetHeight, minY, maxY);
                pos.y = Mathf.Lerp(pos.y, targetHeight, smoothSpeed * Time.fixedDeltaTime);
            }
            else if (touchZero.phase == TouchPhase.Ended || touchOne.phase == TouchPhase.Ended)
            {
                Invoke(nameof(EndOfPaning), 0.15f); // Adding a small delay so the move function doesn't work immediately
            }
        }

        // Limit camera position
        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.z = Mathf.Clamp(pos.z, -panLimit.z, panLimit.z);

        transform.position = pos;
    }

    public void EndOfPaning()
    {
        isPanning = false;
    }

    public bool IsPanning() //Check if not dragging
    {
        return isPanning;
    }
}
