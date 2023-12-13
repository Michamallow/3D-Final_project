using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float flySpeed = 10f; // Speed for flying
    public float mouseSensitivity = 2f;

    private void Update()
    {
        // Get the input values for movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Get the camera's forward and right vectors
        Vector3 cameraForward = transform.Find("Main Camera").GetComponent<Camera>().transform.forward;
        Vector3 cameraRight = transform.Find("Main Camera").GetComponent<Camera>().transform.right;

        // Ignore the vertical component of the camera's forward vector
        cameraForward.y = 0f;

        // Calculate the movement direction
        Vector3 movement = (cameraForward.normalized * verticalInput + cameraRight.normalized * horizontalInput).normalized;

        // Move the player horizontally
        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);

        // Fly when space bar is pressed
        if (Input.GetKey(KeyCode.Space))
        {
            Fly();
        }

        // Move down when left shift is pressed
        if (Input.GetKey(KeyCode.LeftShift))
        {
            MoveDown();
        }
    }

    void Fly()
    {
        // Move the player upwards when flying
        transform.Translate(Vector3.up * flySpeed * Time.deltaTime, Space.World);
    }

    void MoveDown()
    {
        // Move the player downwards when left shift is pressed
        transform.Translate(Vector3.down * flySpeed * Time.deltaTime, Space.World);
    }
}