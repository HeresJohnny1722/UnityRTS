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

    public Vector3 originalZoom;
    public Quaternion originalRotation;

    public Vector3 dragStartPosition;
    public Vector3 dragCurrentPosition;
    public Vector3 rotateStartPosition;
    public Vector3 rotateCurrentPosition;

    public Vector2 boundaryX = new Vector2(-100f, 100f); // Adjust these values to set the boundary in the X-axis
    public Vector2 boundaryZ = new Vector2(-100f, 100f); // Adjust these values to set the boundary in the Z-axis

    // Start is called before the first frame update
    void Start()
    {
        newZoom = originalZoom = cameraTransform.localPosition;
        newZoom.y = maxZoom;
        originalRotation = newRotation = transform.rotation;

        newRotation = originalRotation;
        newZoom = originalZoom;

    }

    // Update is called once per frame
    void Update()
    {
        HandleMouseInput();
        HandleMovementInput();
        HandleResetInput();
        
    }

    void HandleMouseInput()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            if (newZoom.y > minZoom)
            {
                newZoom += Input.mouseScrollDelta.y * zoomAmount;
            }
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            if (newZoom.y < maxZoom)
            {
                newZoom += Input.mouseScrollDelta.y * zoomAmount;
            }
        }

        Vector3 mousePosition = Input.mousePosition;
        Vector3 moveDirection = Vector3.zero;

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

        newPosition.x = Mathf.Clamp(newPosition.x, boundaryX.x, boundaryX.y);
        newPosition.z = Mathf.Clamp(newPosition.z, boundaryZ.x, boundaryZ.y);

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

        newPosition.x = Mathf.Clamp(newPosition.x, boundaryX.x, boundaryX.y);
        newPosition.z = Mathf.Clamp(newPosition.z, boundaryZ.x, boundaryZ.y);

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }

    void HandleResetInput()
    {
        if (Input.GetKey(KeyCode.R))
        {
            // Lerping back to original rotation and zoom
            newRotation = originalRotation;
            newZoom = originalZoom;
        }
    }
}
