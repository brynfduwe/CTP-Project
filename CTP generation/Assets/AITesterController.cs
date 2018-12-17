using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITesterController : MonoBehaviour {

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

    public Vector2 maxVelocity;

    private float jumpInputTimer;


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

            RaycastHit2D hitD = Physics2D.Raycast(transform.position + new Vector3(1, 0, 0), -Vector2.up);
            if (hitD.collider == null)
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

            RaycastHit2D hitR = Physics2D.Raycast(transform.position + new Vector3(1, 0, 0), Vector2.right, 0.05f);
            if (hitR.collider != null)
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

            RaycastHit2D hitR2 = Physics2D.Raycast(transform.position + new Vector3(1, 1, 0), Vector2.right, 0.1f);
            if (hitR2.collider != null)
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
            jumpInputTimer += Time.deltaTime;

            if (jumpInputTimer < 0.3f)
            {

                GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);

                //continue jump till max height
                if (transform.position.y >= startedJumpPos + jumpMax)
                {
                    jumpStarted = false;
                    jumpInputTimer = 0;
                }
            }
            else
            {
                //if they havent reached the minimum height, they continue to jump
                if (transform.position.y >= startedJumpPos + jumpMin)
                {
                    jumpStarted = false;
                    jumpInputTimer = 0;
                }
                else
                {
                    GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
                }
            }
        }

        //player wants to end jump
        if (Input.GetKeyUp(Jump))
        {

        }


        transform.Translate(Vector3.right * Time.deltaTime * speed);

        if (GetComponent<Rigidbody2D>().velocity.x > maxVelocity.x)
            GetComponent<Rigidbody2D>().velocity = new Vector2(maxVelocity.x, GetComponent<Rigidbody2D>().velocity.y);


        if (GetComponent<Rigidbody2D>().velocity.y > maxVelocity.y)
            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, maxVelocity.y);

    }
}
