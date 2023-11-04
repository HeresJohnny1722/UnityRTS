using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform cameraTransform;
    public float screenEdgeWidth = 10f;

    public float minZoom = 25f;
    public float maxZoom = 60f;

    public float normalSpeed;
    public float fastSpeed;
    public float normalKeyboardSpeed;
    public float fastKeyboardSpeed;
    private float movementMouseSpeed;
    private float movementKeyboardSpeed;
    public float movementTime;
    public float rotationAmount;
    public Vector3 zoomAmount;

    public Vector3 newPosition;
    public Quaternion newRotation;
    public Vector3 newZoom;

    public Vector3 dragStartPosition;
    public Vector3 dragCurrentPosition;
    public Vector3 rotateStartPosition;
    public Vector3 rotateCurrentPosition;

    // Start is called before the first frame update
    void Start()
    {
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMouseInput();
        HandleMovementInput();
    }

    void HandleMouseInput()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            // Zoom in only if the current zoom is not at the minimum limit
            if (newZoom.y > minZoom)
            {
                newZoom += Input.mouseScrollDelta.y * zoomAmount;
            }
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            // Zoom out only if the current zoom is not at the maximum limit
            if (newZoom.y < maxZoom)
            {
                newZoom += Input.mouseScrollDelta.y * zoomAmount;
            }
        }

        // Get the mouse position
        Vector3 mousePosition = Input.mousePosition;

        // Initialize a movement vector
        Vector3 moveDirection = Vector3.zero;

        // Check if the mouse is near the screen edges
        if (mousePosition.x < screenEdgeWidth)
        {
            moveDirection += -transform.right;
        }
        else if (mousePosition.x > Screen.width - screenEdgeWidth)
        {
            moveDirection += transform.right;
        }

        if (mousePosition.y < screenEdgeWidth)
        {
            moveDirection += -transform.forward;
        }
        else if (mousePosition.y > Screen.height - screenEdgeWidth)
        {
            moveDirection += transform.forward;
        }

        // Apply camera movement based on the mouse position
        newPosition += moveDirection.normalized * movementMouseSpeed * Time.deltaTime;

        // Ensure the camera doesn't move too far away

        // ... Your existing camera bounds logic ...

        // Update the camera position and rotation
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }

    void HandleMovementInput()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movementMouseSpeed = fastSpeed;
            movementKeyboardSpeed = fastKeyboardSpeed;

        }
        else
        {
            movementMouseSpeed = normalSpeed;
            movementKeyboardSpeed = normalKeyboardSpeed;
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            newPosition += transform.forward * movementKeyboardSpeed;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            newPosition += transform.forward * -movementKeyboardSpeed;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            newPosition += transform.right * movementKeyboardSpeed;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            newPosition += transform.right * -movementKeyboardSpeed;

        }

        if (Input.GetKey(KeyCode.Q))
        {
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }
        if (Input.GetKey(KeyCode.E))
        {
            newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }

        /*if (Input.GetKey(KeyCode.R))
        {
            newZoom += zoomAmount;
        }
        if (Input.GetKey(KeyCode.F))
        {
            newZoom -= zoomAmount;
        }*/

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }
}