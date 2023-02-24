using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoCharacterController : MonoBehaviour
{
    public float acceleration = 150f;
    public float maxSpeed = 10f;
    public float jumpForce = 15f;
    public float jumpBoost = 5f;

    public bool isGrounded;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalAxis = Input.GetAxis("Horizontal");
        Rigidbody rbody = GetComponent<Rigidbody>();
        rbody.velocity += horizontalAxis * Vector3.right * Time.deltaTime * acceleration;

        Collider col = GetComponent<Collider>();
        float halfHeight = col.bounds.extents.y + 0.03f;

        isGrounded = Physics.Raycast(transform.position, Vector3.down, halfHeight);
        
        // if (rbody.velocity.magnitude > maxSpeed)
        // {
        //     rbody.velocity = Vector3.ClampMagnitude(rbody.velocity, maxSpeed);
        // }

        rbody.velocity = new Vector3(Mathf.Clamp(rbody.velocity.x, -maxSpeed, maxSpeed), rbody.velocity.y, rbody.velocity.z);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            rbody.AddForce(Vector3.up * jumpForce, ForceMode.Force);
        }
        
        Color lineColor = (isGrounded) ? Color.green : Color.red;
        Debug.DrawLine(transform.position, transform.position + Vector3.down * halfHeight, lineColor, 0f, false);
        
    }
}
