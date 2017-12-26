using UnityEngine;

public class CamMovement : MonoBehaviour {

    public float panSpeed = 30f;
    public float dragSpeed = 0.2f;
    public float camBorders = 100f;

    public float minX = 100f;
    public float maxX = 100f;
    public float minZ = 50f;
    public float maxZ = 50f;

    private bool movingCam;
    public Vector3 prevMousePos;

    public float zoomSpeed = 0.5f;
    public float maxZoom = 5f;
    public float minZoom = 30f;

    private void Start()
    {
        movingCam = false;
        Debug.Log("Test");
    }

    // Update is called once per frame
    void Update () {

        if (GameLogic.Instance == null)
            Debug.Log("Fuck");

        if (GameLogic.Instance.holding)
        {
            BorderMove();
        }

        if (Input.touchCount == 2)
        {
            Zoom();
            movingCam = false;
            return;
        }

        if (Input.GetMouseButtonUp(0) && movingCam)
        {
            movingCam = false;
        }

        if (Input.GetMouseButton(0) && !GameLogic.Instance.holding)
        {
            HoldMove();
        }

    }

    private void Zoom()
    {
        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);

        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

        float deltaMagDiff = prevTouchDeltaMag - touchDeltaMag;

        float camSize = GetComponent<Camera>().orthographicSize + deltaMagDiff * zoomSpeed * Time.deltaTime;
        camSize = Mathf.Clamp(camSize, minZoom, maxZoom);
        GetComponent<Camera>().orthographicSize = camSize;
    }

    private void BorderMove()
    {
        Vector3 pos = transform.position;

        if (Input.mousePosition.y > Screen.height - camBorders) // Move up
            pos.z += panSpeed * Time.deltaTime;

        if (Input.mousePosition.y < camBorders) // Move down
            pos.z -= panSpeed * Time.deltaTime;

        if (Input.mousePosition.x < camBorders) // Move left
            pos.x -= panSpeed * Time.deltaTime;

        if (Input.mousePosition.x > Screen.width - camBorders) // Move right
            pos.x += panSpeed * Time.deltaTime;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);

        transform.position = pos;
    }

    private void HoldMove()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 pos = transform.position;

        float camSize = GetComponent<Camera>().orthographicSize;

        if (!movingCam)
        {
            prevMousePos = mousePos;
            movingCam = true;
            Debug.Log("What");
        }
        else
        {
            pos.z -= (mousePos.y - prevMousePos.y) * dragSpeed * Time.deltaTime * camSize;
            pos.x -= (mousePos.x - prevMousePos.x) * dragSpeed * Time.deltaTime * camSize;

            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.z = Mathf.Clamp(pos.z, minZ, maxZ);

            transform.position = pos;
            prevMousePos = mousePos;
        }
    }
}
