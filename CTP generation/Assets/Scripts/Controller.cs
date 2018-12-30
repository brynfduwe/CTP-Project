using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public KeyCode Jump;
    public KeyCode Left;
    public KeyCode Right;

    public float jumpMin;
    public float jumpMax;
    public float jumpPower;
    public float speed;

    private bool jumpStarted;
    private float maxJumpPos;
    private float startedJumpPos;
    private bool grounded;
    private bool hitGround;

    public Transform groundPoint1;
    public Transform groundPoint2;

    public Transform topPoint1;
    public Transform topPoint2;

    public Vector2 maxVelocity;

    private float inputTimer;


    // Use this for initialization
    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity / 2;
        GetComponent<Rigidbody2D>().angularVelocity = GetComponent<Rigidbody2D>().angularVelocity / 2;
    }


    // Update is called once per frame
    void FixedUpdate()
    {

        grounded = Physics2D.OverlapArea(groundPoint1.position, groundPoint2.position);

        if (grounded)
        {
            if (!hitGround)
            {
                GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity / 2;
                GetComponent<Rigidbody2D>().angularVelocity = GetComponent<Rigidbody2D>().angularVelocity / 2;

                hitGround = true;
            }

            if (Input.GetKeyDown(Jump))
            {
                if (!jumpStarted)
                {
                    startedJumpPos = transform.position.y;
                  //  maxJumpPos = transform.position.y + jumpMax;
                    GetComponent<Rigidbody2D>().AddForce(Vector2.up * (jumpPower * 1), ForceMode2D.Impulse);
                    jumpStarted = true;
                    hitGround = false;
                }
            }
        }
        else
        {
            grounded = false;
        }

        if (jumpStarted)
        {
            if (Physics2D.OverlapArea(topPoint1.position, topPoint2.position))
            {
                jumpStarted = false;
            }

            //player wants to jump heiger
            if (Input.GetKey(Jump))
            {
                GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);

                //continue jump till max height
                if (transform.position.y >= startedJumpPos + jumpMax)
                {
                    jumpStarted = false;
                }
            }
            else
            {
                //if they havent reached the minimum height, they continue to jump
                if (transform.position.y >= startedJumpPos + jumpMin)
                {
                    jumpStarted = false;
                }
                else
                {
                    GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
                }
            }

            inputTimer += Time.deltaTime;
        }

            //player wants to end jump
        if (Input.GetKeyUp(Jump))
        {
            Debug.Log("Jump Input Time: " + inputTimer.ToString());
            inputTimer = 0;
        }



        if (Input.GetKey(Right) && Input.GetKey(Left))
        {
            //do nothing
        }
        else if (Input.GetKeyUp(Left) || Input.GetKeyUp(Right))
        {
            if (grounded)
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                GetComponent<Rigidbody2D>().angularVelocity = 0;
            }
        }
        else
        {
            //moving
            if (Input.GetKey(Left) && Input.GetKey(Right) == false)
            {
                transform.Translate(-Vector3.right * Time.deltaTime * speed);
            }

            if (Input.GetKey(Right) && Input.GetKey(Left) == false)
            {
                transform.Translate(Vector3.right * Time.deltaTime * speed);
            }
        }

        if (GetComponent<Rigidbody2D>().velocity.x > maxVelocity.x)
            GetComponent<Rigidbody2D>().velocity = new Vector2(maxVelocity.x, GetComponent<Rigidbody2D>().velocity.y);


        if (GetComponent<Rigidbody2D>().velocity.y > maxVelocity.y)
            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, maxVelocity.y);

    }
}
