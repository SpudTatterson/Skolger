using UnityEngine;
using NaughtyAttributes;
using System.Collections;

public class CameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Minimum and maximum distance the camera can go from the pivotPoint")]
    [SerializeField] Vector2 minMaxDistance = new Vector2(10, 40);
    [Tooltip("Speed in units per second")]
    [SerializeField, DisableIf("useFollowTarget")] float speed = 15f;
    [Tooltip("Speed to add when sprinting")]
    [SerializeField, DisableIf("useFollowTarget")] float sprintSpeedAddition = 5f;
    [SerializeField] bool useFollowTarget = true;
    [Tooltip("Time it takes for the camera to go back to the follow target(in seconds)")]
    [SerializeField] float CamResetSpeed = 1f;
    bool sprinting;

    [Header("Drag Move Settings")]
    [SerializeField] KeyCode dragToMoveCameraKeyCode = KeyCode.Mouse2;
    [SerializeField] float dragSmoothing = 2;
    [SerializeField] LayerMask draggableLayers;
    Vector3 dragOrigin;
    Vector3 dragDiff;
    bool isDragging;

    [Header("Drag Rotation Settings")]
    [SerializeField] KeyCode dragToRotateCameraKeyCode = KeyCode.Mouse1;
    [SerializeField] float dragRotationSpeed = 25f;
    Vector3 lastMousePosition;
    bool isRotating = false;

    [Header("Rotation Settings")]
    [Tooltip("Initial speed of rotation in degrees per second")]
    [SerializeField] float initialRotationSpeed = 45f;
    [Tooltip("Maximum speed of rotation in degrees per second")]
    [SerializeField] float maxRotationSpeed = 90f;
    [Tooltip("Speed of acceleration in degrees per second")]
    [SerializeField] float rotationAcceleration = 22.5f;
    float currentRotationSpeed = 0f;

    [Header("References")]
    [Tooltip("The point in the world the camera rotates around")]
    [SerializeField] Transform cameraPivot;
    [Tooltip("The cameras location and rotation")]
    [SerializeField] Transform cameraHolder;
    [SerializeField] Camera cam;
    [SerializeField, EnableIf("useFollowTarget")] Transform followTarget;

    [Header("Inputs")]
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    float mouseScroll;
    float horiz;
    float vert;


    void Update()
    {
        GetInput();

        ScrollToZoom();
        DragToMove();
        Movement();
        DragToRotate();
        Rotation();
    }

    void LateUpdate()
    {
        if (isDragging)
        {
            // Calculate target position for smoother movement
            Vector3 targetPosition = dragOrigin - dragDiff;
            // Use Lerp or MoveTowards for smooth transition
            cameraPivot.position = Vector3.Lerp(cameraPivot.position, targetPosition, Time.deltaTime * dragSmoothing);
        }
    }

    void Movement()
    {
        if (!useFollowTarget)
        {
            float speed = this.speed;
            if (sprinting)
            {
                speed += sprintSpeedAddition;
            }

            Vector3 movementDirection = new Vector3(vert, 0, -horiz);
            cameraPivot.Translate(movementDirection.normalized * speed * Time.deltaTime);
        }
        else
            cameraPivot.position = followTarget.position;

    }
    void DragToMove()
    {
        if (Input.GetKey(dragToMoveCameraKeyCode))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray.origin, ray.direction, out hit, float.MaxValue, draggableLayers);
            if (!isDragging)
            {
                dragOrigin = hit.point;
                isDragging = true;
                useFollowTarget = false;
            }
            dragDiff = hit.point - cameraPivot.position;
        }
        else if (Input.GetKeyUp(dragToMoveCameraKeyCode))
        {
            isDragging = false;
        }

    }

    void DragToRotate()
    {
        if (Input.GetKeyDown(dragToRotateCameraKeyCode))
        {
            lastMousePosition = Input.mousePosition;
            isRotating = true;
        }

        if (Input.GetKey(KeyCode.Mouse1) && isRotating)
        {
            // Calculate the difference in position
            Vector3 dragDiff = Input.mousePosition - lastMousePosition;

            // Apply rotation
            cameraPivot.Rotate(Vector3.up, dragDiff.x * dragRotationSpeed * Time.deltaTime, Space.World);

            // Update lastMousePosition for the next frame
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            isRotating = false;
        }
    }

    void Rotation()
    {
        // Check for input to start rotation
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E))
        {
            // Determine direction based on key pressed
            int direction = Input.GetKey(KeyCode.Q) ? 1 : -1;

            // If currentRotationSpeed is not already greater than the initial rotationSpeed, set it to rotationSpeed
            if (currentRotationSpeed < initialRotationSpeed)
            {
                currentRotationSpeed = initialRotationSpeed;
            }

            // Accelerate rotation speed
            currentRotationSpeed += rotationAcceleration * Time.deltaTime;
            // Clamp the currentRotationSpeed to ensure it doesn't exceed maxRotationSpeed
            currentRotationSpeed = Mathf.Clamp(currentRotationSpeed, 0, maxRotationSpeed);

            // Apply rotation
            // Use currentRotationSpeed multiplied by direction and Time.deltaTime to ensure frame-rate independent rotation
            cameraPivot.Rotate(Vector3.up, direction * currentRotationSpeed * Time.deltaTime);
        }
        else
        {
            // Reset rotation speed when no input is detected
            currentRotationSpeed = 0;
        }
    }

    void GetInput()
    {
        mouseScroll = Input.mouseScrollDelta.y;
        horiz = Input.GetAxisRaw("Horizontal");
        vert = Input.GetAxisRaw("Vertical");
        sprinting = Input.GetKey(sprintKey);
    }

    void ScrollToZoom()
    {
        float distance = CheckDistance();

        if (mouseScroll > 0 && distance > minMaxDistance.x)
        {
            cameraHolder.position = cameraHolder.position + cam.transform.forward;
        }
        else if (mouseScroll < 0 && distance < minMaxDistance.y)
        {
            cameraHolder.position = cameraHolder.position - cam.transform.forward;
        }
    }
    float CheckDistance()
    {
        return Vector3.Distance(cameraHolder.position, cameraPivot.position);
    }
    private IEnumerator ResetCameraCoroutine()
    {
        float elapsedTime = 0;
        Vector3 startingPosition = cameraPivot.position;
        while (elapsedTime < CamResetSpeed)
        {
            cameraPivot.position = Vector3.Lerp(startingPosition, followTarget.position, (elapsedTime / CamResetSpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cameraPivot.position = followTarget.position; // Ensure it's exactly at the follow target's position
        isDragging = false;
        useFollowTarget = true;
    }
    public void ResetCamera()
    {
        StartCoroutine(ResetCameraCoroutine());
    }
    public bool IsUsingFollowTarget()
    {
        return useFollowTarget;
    }
}
