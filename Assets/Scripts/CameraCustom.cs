using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCustom : MonoBehaviour
{
    public float mouseSensitivity = 2f;
    public float maxLookUp = 80f;
    public float maxLookDown = 80f;

    void Start()
    {
        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Rotate the player based on mouse movement
        RotateWithMouse();
    }

    void RotateWithMouse()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate the player horizontally based on mouse X movement
        transform.Rotate(Vector3.up * mouseX);

        // Rotate the player vertically based on mouse Y movement, with constraints
        float newRotationX = transform.eulerAngles.x - mouseY;

        if (newRotationX > 180)
        {
            newRotationX -= 360;
        }

        newRotationX = Mathf.Clamp(newRotationX, -maxLookUp, maxLookDown);

        transform.rotation = Quaternion.Euler(newRotationX, transform.eulerAngles.y, 0f);
    }
}