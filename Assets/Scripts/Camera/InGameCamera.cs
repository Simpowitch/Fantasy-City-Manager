using UnityEngine;
public class InGameCamera : MonoBehaviour
{
    enum Directions { Up, Left, Down, Right }

    [Header("Camera reference")]
    [SerializeField] Camera activeCamera = null;

    [Header("Limits")]
    [SerializeField] Transform minPosTranform = null;
    [SerializeField] Transform maxPosTransform = null;
    [SerializeField] Vector3 maxPos = new Vector3();
    [SerializeField] Vector3 minPos = new Vector3();
    [SerializeField] float zoomMin = 1;
    [SerializeField] float zoomMax = 30;

    [Header("Movement")]
    [Range(0.1f, 100)] [SerializeField] float cameraSpeed = 1f;

    [Header("Zoom")]
    [Range(0.1f, 100)] [SerializeField] float zoomSpeed = 1;

    [Header("Lerp")]
    [SerializeField] float zoomRigidness = 5;
    [SerializeField] float moveRigidness = 5;

    [Header("Edge Movement")]
    [SerializeField] bool edgeMovement = true;
    [SerializeField] float cursorDetectionRange = 30;

    CameraTransform newCameraTransform;

    //Drag
    Vector3 mouseDownPos = new Vector3();
    Vector3 mouseUpPos = new Vector3();

    //Tracking
    public bool IsTracking { get; private set; } = false;
    Transform trackingTransform;

    private void Start()
    {
        newCameraTransform = new CameraTransform(activeCamera);

        if (minPosTranform != null && maxPosTransform != null)
        {
            minPos = minPosTranform.localPosition;
            maxPos = maxPosTransform.localPosition;
        }
    }

    private void Update()
    {
        TrackPosition();
        InputHandler();
        UpdatePosition();
    }

    public void SetCamera(Camera camera)
    {
        activeCamera = camera;
    }

    void InputHandler()
    {
        //Zoom
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            newCameraTransform.cameraSize -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        }
        if (!IsTracking)
        {
            //Middle Mouse Movement
            if (Input.GetMouseButtonDown(2))
            {
                mouseDownPos = activeCamera.ScreenToWorldPoint(Input.mousePosition);
            }
            if (Input.GetMouseButton(2))
            {
                mouseUpPos = activeCamera.ScreenToWorldPoint(Input.mousePosition);
                newCameraTransform.cameraPosition = transform.localPosition + mouseDownPos - mouseUpPos;
            }
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                //Key movement
                MoveDirection(Directions.Right, Input.GetAxis("Horizontal") > 0);
                MoveDirection(Directions.Left, Input.GetAxis("Horizontal") < 0);
                MoveDirection(Directions.Down, Input.GetAxis("Vertical") < 0);
                MoveDirection(Directions.Up, Input.GetAxis("Vertical") > 0);
            }
            else if (edgeMovement)
            {
                //Edge detection
                MoveDirection(Directions.Right, Input.mousePosition.x > Screen.width - cursorDetectionRange);
                MoveDirection(Directions.Left, Input.mousePosition.x < 0 + cursorDetectionRange);
                MoveDirection(Directions.Down, Input.mousePosition.y < 0 + cursorDetectionRange);
                MoveDirection(Directions.Up, Input.mousePosition.y > Screen.height - cursorDetectionRange);
            }
            void MoveDirection(Directions direction, bool isButtonPressed)
            {
                if (isButtonPressed)
                {
                    float zoomScale = activeCamera.orthographicSize / zoomMax;
                    switch (direction)
                    {
                        case Directions.Up:
                            newCameraTransform.cameraPosition.y += cameraSpeed * zoomScale;
                            break;
                        case Directions.Left:
                            newCameraTransform.cameraPosition.x -= cameraSpeed * zoomScale;
                            break;
                        case Directions.Down:
                            newCameraTransform.cameraPosition.y -= cameraSpeed * zoomScale;
                            break;
                        case Directions.Right:
                            newCameraTransform.cameraPosition.x += cameraSpeed * zoomScale;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }

    void UpdatePosition()
    {
        //Apply zoom
        newCameraTransform.cameraSize = Mathf.Clamp(newCameraTransform.cameraSize, zoomMin, zoomMax);
        activeCamera.orthographicSize = Mathf.Lerp(activeCamera.orthographicSize, newCameraTransform.cameraSize, zoomRigidness * Time.deltaTime);

        //Apply movement
        float halfScreenWidth = activeCamera.ScreenToWorldPoint(new Vector3(activeCamera.scaledPixelWidth, 0, 0)).x - transform.localPosition.x;
        float halfScreenHeight = activeCamera.ScreenToWorldPoint(new Vector3(0, activeCamera.scaledPixelHeight, 0)).y - transform.localPosition.y;
        newCameraTransform.cameraPosition.x = Mathf.Clamp(newCameraTransform.cameraPosition.x, minPos.x + halfScreenWidth, maxPos.x - halfScreenWidth);
        newCameraTransform.cameraPosition.y = Mathf.Clamp(newCameraTransform.cameraPosition.y, minPos.y + halfScreenHeight, maxPos.y - halfScreenHeight);
        newCameraTransform.cameraPosition.z = -5;

        Vector3 lerpVector = Vector3.Lerp(transform.localPosition, newCameraTransform.cameraPosition, moveRigidness * Time.deltaTime);
        transform.position = lerpVector;
    }

    #region Tracking
    void TrackPosition()
    {
        if (IsTracking)
        {
            if (trackingTransform == null)
            {
                IsTracking = false;
                return;
            }
            else
            {
                newCameraTransform.cameraPosition = trackingTransform.position;
            }
        }
    }

    public void ToggleTracking()
    {
        IsTracking = !IsTracking;
    }

    public void SetTrackingTarget(Transform transform)
    {
        trackingTransform = transform;
        IsTracking = true;
    }
    #endregion


    protected struct CameraTransform
    {
        public float cameraSize;
        public Vector3 cameraPosition;
        public CameraTransform(Camera camera)
        {
            cameraSize = camera.orthographicSize;
            cameraPosition = camera.transform.position;
        }
    }
    public void SetBoundries(Vector3 min, Vector3 max)
    {
        minPos = min;
        maxPos = max;
    }
    public void SetBoundries(Transform min, Transform max)
    {
        minPos = min.position;
        maxPos = max.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y, 0), new Vector3(newCameraTransform.cameraPosition.x, newCameraTransform.cameraPosition.y, 0));
    }
}