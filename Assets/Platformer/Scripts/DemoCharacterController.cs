using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using TMPro;
using Object = UnityEngine.Object;

public class DemoCharacterController : MonoBehaviour
{
    public float acceleration = 150f;
    public float maxSpeed = 10f;
    public float jumpForce = 15f;
    public float jumpBoost = 5f;

    public TextMeshProUGUI timerText;
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI scoreText;

    private int timeLeft;
    private int timerSinceReset;
    private int coinsCollected;
    private int score;
    private Vector3 position;
    
    private bool isfacingLeft;
    private Vector3 facingLeft;
    
    public bool isGrounded;
    // Start is called before the first frame update
    void Start()
    {
        timeLeft = 100;
        timerSinceReset = 0;
        coinsCollected = 0;
        score = 0;
        
        var localScale = transform.localScale;
        facingLeft = new Vector3(-localScale.x, localScale.y, localScale.z);
        isfacingLeft = false;
        
        position = GetComponent<Transform>().position;
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = "Score \n" + score.ToString();
        coinsText.text = "Coins \n" + coinsCollected.ToString();
        timerText.text = "Time \n" + Math.Floor(timeLeft - (Time.realtimeSinceStartup - timerSinceReset)).ToString();

        if (Math.Floor(timeLeft - (Time.realtimeSinceStartup - timerSinceReset)) <= 0)
        {
            Debug.Log("You ran out of time and lost! Reloaded level.");
            ReloadLevel();
        }
        
        float horizontalAxis = Input.GetAxis("Horizontal");
        Rigidbody rbody = GetComponent<Rigidbody>();
        rbody.velocity += horizontalAxis * Vector3.right * Time.deltaTime * acceleration;

        Collider col = GetComponent<Collider>();
        float halfHeight = col.bounds.extents.y + 0.03f;

        isGrounded = Physics.Raycast(transform.position, Vector3.down, halfHeight);

        rbody.velocity = new Vector3(Mathf.Clamp(rbody.velocity.x, -maxSpeed, maxSpeed), rbody.velocity.y, rbody.velocity.z);

        if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && !isfacingLeft)
        {
            isfacingLeft = true;
            flipSprite();
        } else if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && isfacingLeft)
        {
            isfacingLeft = false;
            flipSprite();
        }
        
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            rbody.AddForce(Vector3.up * jumpForce, ForceMode.Force);

            var ray = new Ray(transform.position, Vector3.up);
            RaycastHit raycastHit;

            if (Physics.Raycast(transform.position, Vector3.up, out raycastHit, 2f))
            {
                Debug.Log("Hit Block");
                if (raycastHit.collider.name == "QuestionBlock(Clone)")
                {
                    Object.Destroy(raycastHit.collider.gameObject);
                    coinsCollected++;
                    score += 100;
                } else if (raycastHit.collider.name == "BrickBlock(Clone)")
                {
                    Object.Destroy(raycastHit.collider.gameObject);
                    score += 100;
                }
            }
        }

        float speed = rbody.velocity.magnitude;
        Animator animator = GetComponent<Animator>();
        animator.SetFloat("Speed", speed);
        animator.SetBool("Jumping", !isGrounded);
        
        Color lineColor = (isGrounded) ? Color.green : Color.red;
        Debug.DrawLine(transform.position, transform.position + Vector3.down * halfHeight, lineColor, 0f, false);


        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadLevel();
        }
    }

    private void flipSprite()
    {
        if (isfacingLeft)
        {
            transform.localScale = facingLeft;
            Vector3 rotation = new Vector3(0, 180f, 0);
            transform.Rotate(rotation);
        } else if (!isfacingLeft)
        {
            var localScale = transform.localScale;
            localScale = new Vector3(-localScale.x, localScale.y, localScale.z);
            transform.localScale = localScale;
            
            Vector3 rotation = new Vector3(0, 180f, 0);
            transform.Rotate(rotation);
        }
    }

    private void ReloadLevel()
    {
        timeLeft = 100;
        timerSinceReset = Convert.ToInt32(Math.Floor(Time.realtimeSinceStartup));
        GetComponent<Transform>().position = position;
        
        score = 0;
        coinsCollected = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name is "GoalBlock(Clone)")
        {
            Debug.Log("You Won!");
            ReloadLevel();
        } else if (other.gameObject.name is "WaterBlock(Clone)")
        {
            Debug.Log("You Drowned in the Water :(  Restarted Level");
            ReloadLevel();
        }
    }
}
