using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    [SerializeField]
    float panSpeed = 20f;
    [SerializeField]
    float panBorderThickness = 10f;
    [SerializeField]
    float panLimit = 150.0f;
    [SerializeField]
    float zoomSpeed = 40f;

    [SerializeField]
    bool enablePanByBorder = false;

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

    //Camera setup
    public void InitializeCameraSettings(Vector3 m_startingPosition, float m_panLimit, float m_minZoom, float m_maxZoom)
    {
        transform.position = m_startingPosition;
        cam.orthographicSize = m_minZoom;
        panLimit = m_panLimit;
        minZoom = m_minZoom;
        maxZoom = m_maxZoom;
    }

    public void LoadSettings(float zoom, Vector3 position)
    {
        cam.orthographicSize = zoom;
        transform.position = position;
    }

    // Update is called once per frame
    void Update()
    {
        //Camera positioning
        Vector3 currentPos = transform.position;
        if (Input.GetKey("w") || (Input.mousePosition.y >= Screen.height - panBorderThickness && enablePanByBorder == true))
        {
            currentPos.y += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("s") || (Input.mousePosition.y <= panBorderThickness && enablePanByBorder == true))
        {
            currentPos.y -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("d") || (Input.mousePosition.x >= Screen.width - panBorderThickness && enablePanByBorder == true))
        {
            currentPos.x += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("a") || (Input.mousePosition.x <= panBorderThickness && enablePanByBorder == true))
        {
            currentPos.x -= panSpeed * Time.deltaTime;
        }

        //Camera clamping and assignment
        currentPos.x = Mathf.Clamp(currentPos.x, -panLimit, panLimit);
        currentPos.y = Mathf.Clamp(currentPos.y, -panLimit, panLimit);
        transform.position = currentPos;

        //Saving data to save manager
        SaveData.current.cameraPosition = transform.position;

        //Camera scrolling
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        cam.orthographicSize -= scroll * zoomSpeed * 100 * Time.deltaTime;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        if(SaveData.current.cameraOrthographicSize != 0)
        {
            cam.orthographicSize = SaveData.current.cameraOrthographicSize;
        }
        SaveData.current.cameraOrthographicSize = cam.orthographicSize;
    }
}
