using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    [SerializeField]
    float panSpeed = 20f;
    [SerializeField]
    float panBorderThickness = 10f;
    [SerializeField]
    Vector2 panLimit;
    [SerializeField]
    float zoomSpeed = 40f;

    [SerializeField]
    float minZoom = 30.0f;
    [SerializeField]
    float maxZoom = 200.0f;

    [SerializeField]
    Camera cam;


    // Start is called before the first frame update
    void Start()
    {
        if (cam == null)
        {
            cam = GetComponent<Camera>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Camera positioning
        Vector3 currentPos = transform.position;
        if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            currentPos.y += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness)
        {
            currentPos.y -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            currentPos.x += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("a") || Input.mousePosition.x <= panBorderThickness)
        {
            currentPos.x -= panSpeed * Time.deltaTime;
        }

        //Camera clamping and assignment
        currentPos.x = Mathf.Clamp(currentPos.x, -panLimit.x, panLimit.x);
        currentPos.y = Mathf.Clamp(currentPos.y, -panLimit.y, panLimit.y);
        transform.position = currentPos;

        //Camera scrolling
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        cam.orthographicSize -= scroll * zoomSpeed * 100 * Time.deltaTime;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);

    }
}
