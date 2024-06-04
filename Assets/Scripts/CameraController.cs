using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Control Settings")]
    [Tooltip("There are values ​​for both mobile and pc.")]
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

    [SerializeField]
    private bool isPanning = false; // To avoid giving direction to the character while moving the camera


    private void Update()
    {
#if UNITY_EDITOR
        ControlCamera();
#endif
    }



    private void FixedUpdate()
    {
#if !UNITY_EDITOR 
        ControlCamera();

#endif
    }

    void ControlCamera() // Updated to FixedUpdate
    {
        Vector3 pos = transform.position;

        // Tek parmakla kamera hareketi
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
        // İki parmakla büyütme/küçültme hareketi
        else if (Input.touchCount == 2)
        {
            isPanning = true; // Kamera hareketi için işaretle
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

                // Yumuşatılmış kamera hareketi
                pos.y = Mathf.Lerp(pos.y, targetHeight, smoothSpeed * Time.fixedDeltaTime);
            }
            else if (touchZero.phase == TouchPhase.Ended || touchOne.phase == TouchPhase.Ended)
            {
                Invoke(nameof(EndOfPaning), 0.15f); // Hemen false olursa selectin scripti çalışabiliyor
            }
        }
        // Fare tekerleği ile yakınlaştırma/uzaklaştırma
        else if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            pos.y -= scroll * scrollSpeed * Time.fixedDeltaTime;
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
        }
        // Kameranın pozisyonunu sınırla
        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.z = Mathf.Clamp(pos.z, -panLimit.z, panLimit.z);

        transform.position = pos;
    }

    public void EndOfPaning()
    {
        isPanning = false;
    }

    public bool IsPanning()
    {
        return isPanning;
    }
}
