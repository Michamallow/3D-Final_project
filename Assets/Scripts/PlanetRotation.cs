using System.Net.NetworkInformation;
using UnityEngine;

public class EarthBehaviour : MonoBehaviour
{
    Vector3 originMousePos = Vector3.zero;
    Vector2 rotationVelocity = Vector2.zero;
    [SerializeField]
    float velocity_factor = 1; // Adjust this value for rotation smoothness
    [SerializeField]
    float speed_factor = 100;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            originMousePos = Input.mousePosition;
            rotationVelocity = Vector2.zero; // Reset velocity on mouse down
        }
        else if (Input.GetMouseButton(0))
        {
            print("MOUSE MOVE");
            Vector3 delta = Input.mousePosition - originMousePos;
            print(delta);
            rotationVelocity = speed_factor * Time.deltaTime * delta; // Adjust speed
            RotateSphere(rotationVelocity);
            originMousePos = Input.mousePosition;
        }
        else
        {
            // Apply a smoothing effect to slow down the rotation gradually
            rotationVelocity = Vector2.Lerp(rotationVelocity, Vector2.zero, velocity_factor * Time.deltaTime);
            RotateSphere(rotationVelocity);
        }

    }

    private void RotateSphere(Vector2 delta)
    {
        Vector3 dragDirection = delta.normalized;

        Vector3 rotationAxis = Vector3.Cross(dragDirection, Camera.main.transform.forward);

        // Multiply the rotation by Time.deltaTime to make it frame-rate independent
        transform.Rotate(rotationAxis, delta.magnitude * Time.deltaTime, Space.World);
    }
}