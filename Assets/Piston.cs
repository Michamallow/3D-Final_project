using UnityEngine;

public class Piston : MonoBehaviour
{
    public Rigidbody2D rb;

    private SliderJoint2D joint;
    private float accumulatedTime = 0.0f;

    public float amplitude = 0.2f;
    public float offset = 0.0f;
    public float frequence = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        joint = GetComponent<SliderJoint2D>();

        JointMotor2D motor2d = joint.motor;
        motor2d.motorSpeed = 1;
        joint.motor = motor2d;
    }


    // Update is called once per frame
    void Update()
    {
        accumulatedTime += Time.deltaTime;

        JointMotor2D motor2d = joint.motor;
        float speed = amplitude * Mathf.Sin(offset + (frequence * accumulatedTime));
        Debug.Log(speed);
        motor2d.motorSpeed = speed;
        joint.motor = motor2d;
    }
}
