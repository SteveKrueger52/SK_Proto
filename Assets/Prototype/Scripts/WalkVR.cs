using UnityEngine;

// Code by Steve Krueger
// VR Prototyping for ARTG 5640 - Prototyping for Experience Design
// 
// Original code protected under Creative Commons Attribution 4.0 International (CC BY 4.0)
// Basically keep this comment intact and you're good

[RequireComponent(typeof(Rigidbody))]
public class WalkVR : MonoBehaviour
{
    // Walking variables
    public float walkSpeed = 2f;
    public float deadzone = .1f;
    
    // Head tilt variables
    public float rotateAngle = 60f;
    public float deadAngle = 10f;
    public float maxAngle = 45f;

    // Controller tilt variables
    public float fastAnchor;
    public float fastRotateAngle = 100f;
    public float fastDeadAngle = 10f;
    public float fastMaxAngle = 90f;
    
    // Debug tools
    // public Text text;
    // private LineRenderer line;
    
    public GameObject controller;
    public Camera head;
    private Rigidbody rb;
    private bool dragRotate;
    
    // Start is called before the first frame update
    void Start()
    {
        /*
        // Debug stuff - shows a line for the X/Z axis of the controller
        
        line = GetComponent<LineRenderer>();
        line.startColor = line.endColor = Color.white;
        line.startWidth = line.endWidth = 0.01f;
        */
        
        head = Camera.main;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Get Controller Input
        Vector2 touchCoords = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
        bool touchPressed = OVRInput.Get(OVRInput.Button.PrimaryTouchpad);
        bool trigger = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger);
        
        // Toggle fast Rotate with Touchpad + Trigger
        if (!dragRotate && trigger && touchPressed)
        {
            dragRotate = true;
            fastAnchor = controller.transform.eulerAngles.z;
            fastAnchor = fastAnchor > 180 ? fastAnchor - 360 : fastAnchor;
        }
        if (dragRotate && !(trigger && touchPressed))
            dragRotate = false;

        // Rotate Scene
        if (!dragRotate)
        {
            // Get Head Tilt for Scene Rotation
            float tilt = head.transform.eulerAngles.z;
            tilt = tilt > 180 ? tilt - 360 : tilt;

            // Linearly map from minAngle <-> max Angle to 0<->1, or the negative equivalent
            // Y = (X-A)/(B-A) * (D-C) + C
            float d_tilt = Mathf.Clamp(Mathf.Abs(tilt) > deadAngle ?
                (Mathf.Abs(tilt) - deadAngle) / (maxAngle - deadAngle) * (Mathf.Abs(tilt) / tilt) : 0,-1,1);
        
            // text.text = "Mode: Head\nTilt: " + tilt + " -> " + d_tilt;
            
            // Rotate Scene via Head Tilt
            if (Mathf.Abs(tilt) > deadAngle)
                transform.Rotate(Vector3.up, -1f * d_tilt * Time.deltaTime * rotateAngle);
        }
        else
        {
            // Get Controller Tilt for Scene Rotation, scale and clamp
            float tilt = controller.transform.eulerAngles.z;
            tilt = (tilt > 180 ? tilt - 360 : tilt) - fastAnchor;
            
            // Linearly map from minAngle <-> max Angle to 0<->1, or the negative equivalent
            // Y = (X-A)/(B-A) * (D-C) + C
            float d_tilt = Mathf.Clamp(Mathf.Abs(tilt) > fastDeadAngle ? 
                (Mathf.Abs(tilt) - fastDeadAngle) / (fastMaxAngle - fastDeadAngle) * (Mathf.Abs(tilt) / tilt) : 0,-1,1);
            
            // text.text = "Mode: Controller\nTilt: " + tilt + " -> " + d_tilt;
            
            // Rotate Scene via Controller Tilt
            if (Mathf.Abs(tilt) > fastDeadAngle)
                transform.Rotate(Vector3.up, -1f * d_tilt * Time.deltaTime * fastRotateAngle);
        }
        
        // Get Steering - oriented to controller direction
        Vector3 forward = Vector3.Normalize(new Vector3(controller.transform.forward.x, 0, controller.transform.forward.z));
        Vector3 right = Quaternion.AngleAxis(90, Vector3.up) * forward;
        Vector3 steering = Vector3.ClampMagnitude(touchCoords.x * right + touchCoords.y * forward, 1);

        // Move via touchpad
        if (!(trigger && touchPressed) && steering.magnitude > deadzone)
            rb.MovePosition(transform.position + steering * Time.deltaTime * walkSpeed);
        
        // Debug line
//        line.SetPositions(new[]
//        {
//            controller.transform.position + right,
//            controller.transform.position,
//            controller.transform.position + forward
//        });
    }
}
