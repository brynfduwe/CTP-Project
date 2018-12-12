using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public KeyCode Jump;
    public KeyCode Left;
    public KeyCode Right;

    public float jumpMax;
    public float jumpPower;
    public float speed;

    private bool jumpStarted;
    private float maxJumpPos;
    private bool grounded;
    private bool hitGround;

    public Transform groundPoint1;
    public Transform groundPoint2;

    public Vector2 maxVelocity;


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

            if (Input.GetKeyDown(Jump) || Input.GetKey(Jump))
            {
                if (!jumpStarted)
                {
                    maxJumpPos = transform.position.y + jumpMax;
                    GetComponent<Rigidbody2D>().AddForce(Vector2.up * (jumpPower * 5), ForceMode2D.Impulse);
                    jumpStarted = true;
                    hitGround = false;
                }
            }
        }
        else
        {
            grounded = false;
        }

        if (Input.GetKey(Jump) && jumpStarted == true)
        {
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }

        if (jumpStarted)
        {
            if (Input.GetKeyUp(Jump) || transform.position.y >= maxJumpPos)
            {
                jumpStarted = false;
            }
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
                GetComponent<Rigidbody2D>().AddForce(Vector2.left * speed, ForceMode2D.Force);
            }

            if (Input.GetKey(Right) && Input.GetKey(Left) == false)
            {
                GetComponent<Rigidbody2D>().AddForce(Vector2.right * speed, ForceMode2D.Force);
            }
        }

        if (GetComponent<Rigidbody2D>().velocity.x > maxVelocity.x)
            GetComponent<Rigidbody2D>().velocity = new Vector2(maxVelocity.x, GetComponent<Rigidbody2D>().velocity.y);


        if (GetComponent<Rigidbody2D>().velocity.y > maxVelocity.y)
            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, maxVelocity.y);

    }
}
