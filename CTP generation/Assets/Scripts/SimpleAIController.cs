using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAIController : MonoBehaviour {

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

    private float jumpInputTimer;
    private float jumpTime;

    private bool jumpTargetHit = true;
    private Vector2 jumpTargetPos;
    private Transform jumpTargetTransform;
    private Transform standingPlat;
    private bool stopMoveRight;

    private bool jumpHitIgnore = true;

    private Vector2 startPos;

    private bool waitJump;

    private List<Transform> failedPlatformMover;

    private float MoverCounter;

    private bool tryThing;

    private bool dirLeft = false;

    private Transform lastJumpedOffPlat;

    //  List<Transform> failedPlats = new List<Transform>();
    List<Transform> doNotRetryPlats = new List<Transform>();

    List<PlatformCheckClass> failedPlatfromList = new List<PlatformCheckClass>();

    public bool doNotColor;

    private Transform lastTriedPlat;


    private List<int> actions = new List<int>();

    // Use this for initialization
    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity / 2;
        GetComponent<Rigidbody2D>().angularVelocity = GetComponent<Rigidbody2D>().angularVelocity / 2;

        jumpTargetPos = transform.position;
        startPos = transform.position + new Vector3(-1, -10);

        InvokeRepeating("AddActions", 0.0f, 0.1f);
    }


    void AddActions()
    {
        if (jumpStarted)
        {
            if (jumpTime == 0.3f)
            {
                actions.Add(1);
            }

            if (jumpTime == 0.1f)
            {
                actions.Add(2);
            }
        }
        else
        {
            actions.Add(0);
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        // actions.Add(Random.Range(0,3));

        grounded = Physics2D.OverlapArea(groundPoint1.position, groundPoint2.position);

        if (grounded)
        {
            if (!hitGround)
            {
                GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity / 3;
                GetComponent<Rigidbody2D>().angularVelocity = GetComponent<Rigidbody2D>().angularVelocity / 3;

                hitGround = true;
            }

            RaycastHit2D hitD = Physics2D.Raycast(transform.position + new Vector3(0.7f, 0, 0), -Vector2.up, 3f);
            if (hitD.collider == null)
            {
                RaycastHit2D hitD2 = Physics2D.Raycast(transform.position + new Vector3(1.7f, 0, 0), -Vector2.up, 3f);
                if (hitD2.collider == null)
                {
                    StartJump(0.3f);
                }
                else
                {
                    StartJump(0.1f);
                }
            }
            else
            {
                if (hitD.transform.gameObject.GetComponent<Spike>() != null)
                {
                    RaycastHit2D hitD2 = Physics2D.Raycast(transform.position + new Vector3(1.7f, 0, 0), -Vector2.up, 3f);
                    if (hitD2.collider == null)
                    {
                        StartJump(0.3f);
                    }
                    else
                    {
                        if (hitD2.transform.gameObject.GetComponent<Spike>() != null)
                        {
                            StartJump(0.1f);
                        }
                        else
                        {
                            StartJump(0.3f);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < doNotRetryPlats.Count; i++)
                    {
                        if (doNotRetryPlats[i].gameObject != null)
                        {
                            if (hitD.transform == doNotRetryPlats[i].transform)
                            {
                                StartJump(0.3f);
                            }
                        }
                    }
                }
            }

            RaycastHit2D hitR = Physics2D.Raycast(transform.position + new Vector3(1, 0, 0), Vector2.right, 0.05f);
            if (hitR.collider != null)
            {
                StartJump(0.1f);
            }

            RaycastHit2D hitR2 = Physics2D.Raycast(transform.position + new Vector3(1, 1, 0), Vector2.right, 1f);
            if (hitR2.collider != null)
            {
                StartJump(0.3f);
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
            else
            {
                if (!dirLeft)
                {
                    RaycastHit2D hitCeiling = Physics2D.Raycast(transform.position + new Vector3(0.3f, 0.5f, 0), Vector2.up, 0.5f);
                    if (hitCeiling.collider != null)
                    {
                        transform.position = transform.position - new Vector3(0.2f, 0);
                    }
                }
                else
                {
                    RaycastHit2D hitCeiling = Physics2D.Raycast(transform.position + new Vector3(-0.3f, 0.5f, 0), Vector2.up, 0.5f);
                    if (hitCeiling.collider != null)
                    {
                        transform.position = transform.position + new Vector3(0.2f, 0);
                    }
                }
            }

            jumpInputTimer += Time.deltaTime;

            if (jumpInputTimer < jumpTime)
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

        if (!stopMoveRight && !jumpHitIgnore)
        {
            if (!dirLeft)
            {
                if (transform.position.x + 0.5f >= jumpTargetPos.x)
                {
                    //  MoverCounter++;
                    stopMoveRight = true;
                    transform.position = new Vector3(jumpTargetPos.x, transform.position.y);
                    GetComponent<Rigidbody2D>().velocity = Vector2.zero;


                }
            }
        }
        else
        {
            if (Vector2.Distance(transform.position, jumpTargetPos) < 1)
            {
                dirLeft = false;
                stopMoveRight = false;
                jumpTargetHit = true;
                jumpHitIgnore = true;

                standingPlat = jumpTargetTransform;
            }
        }

        if (waitJump)
        {
            if (jumpTargetPos.y - transform.position.y < 0f)
            {
                waitJump = false;
            }
        }

        if (jumpHitIgnore || !stopMoveRight)
        {
            if (!waitJump)
            {
                if (!dirLeft)
                {
                    transform.Translate(Vector3.right * Time.deltaTime * speed);
                }
            }
            else
            {
                RaycastHit2D hitD = Physics2D.Raycast(transform.position + new Vector3(-0, -0.5f, 0), -Vector2.up, 4f);
                if (hitD.collider != null)
                {
                    if (hitD.transform.gameObject.GetComponent<Spike>() != null)
                    {
                        transform.Translate(Vector3.right * Time.deltaTime * speed);
                        waitJump = false;
                    }
                }
            }
        }
        else
        {
            RaycastHit2D hitD = Physics2D.Raycast(transform.position + new Vector3(-0, -0.5f, 0), -Vector2.up, 4f);
            if (hitD.collider != null)
            {
                if (hitD.transform.gameObject.GetComponent<Spike>() != null)
                {
                    transform.Translate(Vector3.right * Time.deltaTime * speed);
                    waitJump = false;
                }
            }
        }

        if (GetComponent<Rigidbody2D>().velocity.x > maxVelocity.x)
            GetComponent<Rigidbody2D>().velocity = new Vector2(maxVelocity.x, GetComponent<Rigidbody2D>().velocity.y);


        if (GetComponent<Rigidbody2D>().velocity.y > maxVelocity.y)
            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, maxVelocity.y);

    }


    void StartJump(float time)
    {
        if (!jumpStarted)
        {
            bool fail = false;
            RaycastHit2D hitD = Physics2D.Raycast(transform.position + new Vector3(-0.2f, -0.5f, 0), -Vector2.up, 3f);
            foreach (var fp in failedPlatfromList)
            {
                if (fp.referenceTransform == hitD.transform)
                {
                    fail = true;
                }
            }

            if (hitD.collider != null && fail == false)
            {
                failedPlatfromList.Add(new PlatformCheckClass(hitD.transform));

                if (!doNotColor)
                    hitD.transform.GetComponent<SpriteRenderer>().color = Color.yellow;

            }

            int possibleRoutes = 0;
            //target platform picker.
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 20);

            Transform nearestCol = cols[0].transform;
            float nearestDist = 100;

            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i].gameObject != this.gameObject)
                {
                    bool inFailed = false;

                    if (cols[i].gameObject.GetComponent<Spike>() == null)
                    {



                        foreach (var fp in failedPlatfromList)
                        {
                            if (fp.referenceTransform == hitD.transform)
                            {
                                for (int c = 0; c < fp.GetFailedPlats().Count; c++)
                                {
                                    if (fp.GetFailedPlats()[c] == cols[i].transform)
                                    {
                                        inFailed = true;
                                    }
                                }
                            }
                        }

                        foreach (var c in doNotRetryPlats)
                        {
                            if (c == cols[i].transform)
                                inFailed = true;
                        }

                        if (Vector2.Distance(transform.position, cols[i].transform.position) > 15)
                        {
                            inFailed = true;
                        }


                        float xDist = Vector2.Distance(new Vector2(transform.position.x, 0),
                            new Vector2(cols[i].transform.position.x, 0));
                        float yDist = Vector2.Distance(new Vector2(0, transform.position.y),
                            new Vector2(0, cols[i].transform.position.y));
                        float tDist = xDist;

                        if (cols[i].transform.position.y > transform.position.y - 1)
                        {
                            tDist += yDist;
                        }
                        else
                        {
                            tDist -= yDist * 1.5f;
                        }

                        if (tDist > 5)
                        {
                            inFailed = true;
                        }


                        if (inFailed == false)
                        {
                            //angle check
                            Vector3 playerDir = cols[i].transform.position - transform.position;
                            float angle = Vector3.Dot(playerDir, cols[i].transform.right);
                            if (dirLeft)
                            {
                                angle = Vector3.Dot(playerDir, -cols[i].transform.right);
                            }

                            if (angle > 0.2f)
                            {
                                bool failNear = false;
                                if (lastTriedPlat != null)
                                {
                                    if (nearestCol == lastTriedPlat)
                                    {
                                        failNear = true;
                                    }
                                }

                                if (!failNear)
                                {

                                    float dist = Vector2.Distance(transform.position, cols[i].transform.position);
                                    if (dist < nearestDist)
                                    {


                                        nearestDist = Vector2.Distance(transform.position, cols[i].transform.position);
                                        nearestCol = cols[i].transform;
                                        lastTriedPlat = nearestCol;

                                    }

                                    possibleRoutes++;
                                }

                            }
                        }
                    }

                }
            }


            if (possibleRoutes > 0)
            {
                foreach (var fp in failedPlatfromList)
                {
                    if (fp.referenceTransform == nearestCol.transform)
                    {
                        fp.AddFailedPlat(nearestCol.transform);
                    }
                }

                if (!doNotColor)
                    nearestCol.gameObject.GetComponent<SpriteRenderer>().color = Color.green;



                jumpTargetTransform = nearestCol.transform;
                jumpTargetPos = nearestCol.position;
                //    Debug.Log(nearestDist.ToString());

                RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(-0.2f, -0.5f, 0), -Vector2.up, 3f);
                if (hit.collider != null)
                {
                    lastJumpedOffPlat = hit.transform;
                }
            }

            if (lastTriedPlat != null)
            {
                foreach (var fp in failedPlatfromList)
                {
                    if (fp.referenceTransform == hitD.transform)
                    {
                        fp.AddFailedPlat(lastTriedPlat);
                        lastTriedPlat = null;
                    }
                }
            }

            //jump set up
            startedJumpPos = transform.position.y;

            //  maxJumpPos = transform.position.y + jumpMax;
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * (jumpPower * 1), ForceMode2D.Impulse);
            jumpStarted = true;
            hitGround = false;

            if (nearestDist < 20)
            {
                jumpTargetHit = false;
                jumpHitIgnore = false;
                stopMoveRight = false;
            }
            else
            {
                jumpTargetHit = true;
                jumpHitIgnore = true;
                stopMoveRight = false;
            }

            if (nearestDist < 2)
            {
                //short jump
                jumpTime = 0.1f;
            }
            else
            {
                //highjump
                jumpTime = 0.3f;
            }

            MoverCounter += nearestDist;

             jumpTime = time;
        }


        RaycastHit2D hitU = Physics2D.Raycast(transform.position + new Vector3(0.5f, 0.0f, 0), Vector2.up);
        if (hitU.collider != null)
        {
            waitJump = true;
        }

        if (!doNotColor)
            GetComponent<SpriteRenderer>().color = Color.green;
    }


    public bool SpikeCheck()
    {
        RaycastHit2D hitD = Physics2D.Raycast(transform.position + new Vector3(0.2f, -0.5f, 0), -Vector2.up, 0.25f);
        if (hitD.collider != null)
        {
            if (hitD.transform.gameObject.GetComponent<Spike>() != null)
                return true;
        }

        hitD = Physics2D.Raycast(transform.position + new Vector3(-0.0f, -0.5f, 0), -Vector2.up, 0.4f);
        if (hitD.collider != null)
        {
            if (hitD.transform.gameObject.GetComponent<Spike>() != null)
                return true;
        }

        hitD = Physics2D.Raycast(transform.position + new Vector3(-0.2f, -0.5f, 0), -Vector2.up, 0.25f);
        if (hitD.collider != null)
        {
            if (hitD.transform.gameObject.GetComponent<Spike>() != null)
                return true;
        }
        return false;
    }


    public void ResetPlayer()
    {
        dirLeft = false;
        MoverCounter = 0;

        failedPlatfromList.Clear();
        doNotRetryPlats.Clear();

        lastJumpedOffPlat = null;
        jumpTargetTransform = null;

        jumpTargetHit = true;
        jumpHitIgnore = true;
        stopMoveRight = false;

        jumpTargetPos = startPos;

        waitJump = false;

        actions.Clear();
    }

    public List<int> GetAllActions()
    {
        return actions;
    }
}