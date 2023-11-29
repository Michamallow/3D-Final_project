using UnityEngine;

public class EarthBehaviour : MonoBehaviour
{
    Vector3 originMousePos = Vector3.zero;
    Vector3 deltaMousePos = Vector3.zero;
    Vector2 rotationVelocity = Vector2.zero;
    [SerializeField]
    float velocity_factor = 1; // Adjust this value for rotation smoothness
    [SerializeField]
    float speed_factor = 100;

    // Update is called once per frame
    void Update()
    {
        if (CastRay(Input.mousePosition) && Input.GetMouseButtonDown(0))
        {
            originMousePos = deltaMousePos = Input.mousePosition;
            rotationVelocity = Vector2.zero; // Reset velocity on mouse down
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - originMousePos;
            rotationVelocity = delta * Time.deltaTime * speed_factor; // Adjust speed
            RotateSphere(rotationVelocity);
        }
        else
        {
            // Apply a smoothing effect to slow down the rotation gradually
            rotationVelocity = Vector2.Lerp(rotationVelocity, Vector2.zero, velocity_factor * Time.deltaTime);
            RotateSphere(rotationVelocity);
        }
    }

    private bool CastRay(Vector3 pos)
    {
        Ray ray = Camera.main.ScreenPointToRay(pos);
        if (Physics.Raycast(ray))
            return true;
        return false;
    }

    private void RotateSphere(Vector2 delta)
    {
        // Multiply the rotation by Time.deltaTime to make it frame-rate independent
        transform.rotation *= Quaternion.Euler(delta.y * Time.deltaTime, delta.x * Time.deltaTime, 0);
    }
}