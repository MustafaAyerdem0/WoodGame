using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f; // Kamera kaydırma hızı
    public float panBorderThickness = 10f; // Kenar boşluğu kalınlığı
    public Vector3 panLimit; // Kaydırma sınırları

    public float scrollSpeed = 20f; // Yakınlaştırma/Uzaklaştırma hızı
    public float minY = 20f; // Minimum yakınlaştırma
    public float maxY = 120f; // Maksimum uzaklaştırma

    private Vector3 touchStart;
    private float initialPinchDistance;
    private float initialCameraHeight;

    void Update()
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
                Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.transform.position.y));
                pos.x += direction.x * panSpeed * Time.deltaTime;
                pos.z += direction.z * panSpeed * Time.deltaTime;
            }
        }
        // İki parmakla büyütme/küçültme hareketi
        else if (Input.touchCount == 2)
        {
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

                pos.y = initialCameraHeight + pinchDifference * scrollSpeed * Time.deltaTime;
                pos.y = Mathf.Clamp(pos.y, minY, maxY);
            }
        }
        // Fare tekerleği ile yakınlaştırma/uzaklaştırma
        else if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            pos.y -= scroll * scrollSpeed * Time.deltaTime;
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
            print(scroll);
        }

        // Kenar hareketleri (mouse veya klavye)
        if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            pos.z += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness)
        {
            pos.z -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("a") || Input.mousePosition.x <= panBorderThickness)
        {
            pos.x -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            pos.x += panSpeed * Time.deltaTime;
        }

        // Kameranın pozisyonunu sınırla
        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.z = Mathf.Clamp(pos.z, -panLimit.z, panLimit.z);

        transform.position = pos;
    }
}


