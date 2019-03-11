using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private bool stopMoveRight;

    private Transform standingPlat;

    private bool jumpHitIgnore = true;

    private Vector2 startPos;

    private bool waitJump;

    private List<Transform> failedPlatformMover;

    private bool tryThing;

    private bool dirLeft = false;

    private Transform lastJumpedOffPlat;

    //  List<Transform> failedPlats = new List<Transform>();
    List<Transform> doNotRetryPlats = new List<Transform>();

    List<PlatformCheckClass> failedPlatfromList = new List<PlatformCheckClass>();

    public bool doNotColor;

    private Transform lastTriedPlat;

    private bool spikeBelowStop = false;
    
    ///
    ///
    
    private List<int> actions = new List<int>();
    private float recordPosX = 0.45f;
    private bool recordRepeat = false;
    SetUpManager.MappingType mapping;

    private int prevIterToCheck = 0;
    private List<int[]> prevInputVector = new List<int[]>(); //right, a(jump)

    private float inputDelay = 0f;
    private float inputDelayTimer = 0;

    private int health = 3;
    private int maxHealth = 3;
    private bool invul = false;
    private float invulTimer = 0;

    // Use this for initialization
    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity / 2;
        GetComponent<Rigidbody2D>().angularVelocity = GetComponent<Rigidbody2D>().angularVelocity / 2;

        jumpTargetPos = transform.position;
        startPos = transform.position + new Vector3(-1, -10);

        //InvokeRepeating("AddActions", 0.0f, 0.1f);
    }


    public void SetMapping(SetUpManager.MappingType set)
    {
        mapping = set;
    }

    //if (jumpHitIgnore || !stopMoveRight)
    //{
    //    if (!waitJump && !spikeBelowStop)
    //    {
    //        if (!dirLeft)
    //        {
    //            transform.Translate(Vector3.right* Time.deltaTime* speed);
    //        }

    void AddActions()
    {
        switch (mapping)
        {
            case SetUpManager.MappingType.InputChangeRate:
                int[] inputVector = { 0, 0 };
                if (jumpHitIgnore || !stopMoveRight)
                    if (!waitJump && !spikeBelowStop)
                        if (!dirLeft || dirLeft) // left and right are the same input value, as only one or none can be true, not both.  
                            inputVector[0] = 1; // right
                if(jumpStarted)
                    inputVector[1] = 1; //jump

                prevInputVector.Add(inputVector); // ads to list

                int[] changeVector = {0, 0};
                for (int j = 0; j < inputVector.Length; j++)
                {
                    bool different = false;
                    for (int i = 1; i < prevIterToCheck; i++)
                    {
                        if (inputVector[j] != prevInputVector[prevIterToCheck - i][j] ) // compares the input from x tiles back
                            different = true;
                    }
                    if(different)
                       changeVector[j] = 1;
                }

                actions.Add(changeVector.Sum());

                if (prevInputVector.Count <= 3)
                    prevIterToCheck++; // updates to be 3 behind after the tester has moved enough

             
                break;
            case SetUpManager.MappingType.ReactionDelay:
                //how to map?
               
                break;
            case SetUpManager.MappingType.Health:
                actions.Add(health);
                break;
            default:
                if (!recordRepeat)
                {
                    //add current action
                    if (jumpStarted)
                    {
                        if (jumpTime == 0.1f)
                        {
                            actions.Add(1);
                        }

                        if (jumpTime == 0.3f)
                        {
                            actions.Add(2);
                        }
                    }
                    else
                    {
                        actions.Add(0);

                    }

                    recordRepeat = true;
                }
                else
                {
                    //add last action
                    actions.Add(actions[actions.Count - 1]);
                    recordRepeat = false;
                }
                break;

        }
    }


    // Update is called once per frame
    void Update()
    {
        if (invul)
        {
            invulTimer += Time.deltaTime;
            if (invulTimer >= 0.5f)
            {
                invul = false;
                GetComponent<SpriteRenderer>().color = Color.green;
            }
        }


        if (!dirLeft)
        {
            if (transform.position.x >= recordPosX)
            {
                AddActions();
                recordPosX = recordPosX + 1;
            }
        }
        else
        {
            if (transform.position.x <= recordPosX - 1)
            {
                actions.Remove(actions.Count);
                recordPosX = recordPosX - 1;
            }
        }
    }

    void FixedUpdate()
    {
        // actions.Add(Random.Range(0,3));
        grounded = Physics2D.OverlapArea(groundPoint1.position, groundPoint2.position);

        if (grounded)
        {
            spikeBelowStop = false;

            if (!hitGround)
            {
                GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity / 3;
                GetComponent<Rigidbody2D>().angularVelocity = GetComponent<Rigidbody2D>().angularVelocity / 3;

                spikeBelowStop = false;
                dirLeft = false;

                jumpTargetHit = true;
                jumpHitIgnore = true;
                stopMoveRight = false;

                waitJump = false;
            }

            RaycastHit2D hitR = Physics2D.Raycast(transform.position + new Vector3(0.5f, 0, 0), Vector2.right, 0.05f);
            RaycastHit2D hitR2 = Physics2D.Raycast(transform.position + new Vector3(0.5f, 1, 0), Vector2.right, 1f);
            RaycastHit2D hitR3 = Physics2D.Raycast(transform.position + new Vector3(0.5f, 2, 0), Vector2.right, 1f);
            if (hitR.collider != null && hitR2.collider == null && hitR3.collider == null)
            {
                StartJump(0.1f);
            }

            if (hitR2.collider != null || hitR3.collider != null)
            {
                StartJump(0.3f);
            }

            RaycastHit2D hitD = Physics2D.Raycast(transform.position + new Vector3(0.7f, 0, 0), -Vector2.up, 3f); ///ahead of player down check
            if (hitD.collider == null)
            {
                RaycastHit2D hitD2 = Physics2D.Raycast(transform.position + new Vector3(1.7f, 0, 0), -Vector2.up, 3f);///ahead x2 of player down check
                                                                                                                      //
                if (hitD2.collider == null)
                {
                    StartJump(0.3f);
                }
                else
                {
                    if (hitD2.transform.gameObject.GetComponent<Spike>() != null)
                    {
                        StartJump(0.3f);
                    }
                    else
                    {
                        RaycastHit2D hitRA = Physics2D.Raycast(transform.position + new Vector3(0, 0.7f, 0), Vector2.right, 0.05f);
                        RaycastHit2D hitRAA = Physics2D.Raycast(transform.position + new Vector3(0, 1.7f, 0), Vector2.right, 0.05f);
                        if (hitRA.collider != null || hitRAA.collider != null)
                        {
                            StartJump(0.3f);
                        }
                        else
                        {
                            StartJump(0.1f);
                        }
                    }
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
                            StartJump(0.3f);
                        }
                        else
                        {
                            StartJump(0.1f);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < doNotRetryPlats.Count; i++)
                    {
                        if (doNotRetryPlats[i] != null)
                        {
                            if (doNotRetryPlats[i].gameObject.GetComponent<Spike>() != null)
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
                }
            }

    
        }
        else
        {
            grounded = false;
            hitGround = false;

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
                    RaycastHit2D hitCeiling = Physics2D.Raycast(transform.position + new Vector3(0.45f, 0.5f, 0),
                        Vector2.up, 0.1f);
                    if (hitCeiling.collider != null)
                    {
                        transform.position = transform.position - new Vector3(0.3f, 0);
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
                if (transform.position.x >= jumpTargetPos.x)
                {
                    //  MoverCounter++;
                    stopMoveRight = true;
                    transform.position = new Vector3(jumpTargetPos.x, transform.position.y);
                    GetComponent<Rigidbody2D>().velocity = Vector2.zero;

                   // spikeBelowStop = false;
                }
            }
            else
            {
                if (transform.position.x + 0.5f <= jumpTargetPos.x)
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
            if (jumpTargetPos.y - jumpTargetPos.y < 0f)
            {
                dirLeft = false;
               // stopMoveRight = false;
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
            if (!waitJump && !spikeBelowStop)
            {
                if (!dirLeft)
                {
                    if (!grounded)
                    {
                        RaycastHit2D hitD0 = Physics2D.Raycast(groundPoint1.transform.position , -Vector2.up, 3f);
                        RaycastHit2D hitD1 = Physics2D.Raycast(groundPoint2.transform.position, -Vector2.up, 3f);
                        RaycastHit2D hitD2 = Physics2D.Raycast(transform.position + new Vector3(1f, -0.5f, 0),
                            -Vector2.up, 15f);
                        RaycastHit2D hitD3 = Physics2D.Raycast(transform.position + new Vector3(2f, -0.5f, 0),
                            -Vector2.up, 3f);


                        if ((hitD0.collider != null && hitD1.collider != null) && !jumpStarted)
                        {
                            if (hitD0.collider.gameObject.GetComponent<Spike>() == null &&
                                hitD1.collider.gameObject.GetComponent<Spike>() == null)
                            {
                                if (hitD2.collider == null)
                                {
                                    if (hitD1.collider.gameObject.GetComponent<Spike>() != null)
                                    {
                                        transform.Translate(Vector3.right * Time.deltaTime * speed);
                                    }
                                }
                                else
                                {
                                    if (hitD2.collider.gameObject.GetComponent<Spike>() != null)
                                    {
                                        if (hitD1.collider.gameObject.GetComponent<Spike>() != null)
                                        {
                                            transform.Translate(Vector3.right * Time.deltaTime * speed);
                                        }
                                    }
                                    else
                                    {
                                        transform.Translate(Vector3.right * Time.deltaTime * speed);
                                    }
                                }
                            }
                            else
                            {
                                transform.Translate(Vector3.right * Time.deltaTime * speed);
                            }
                        }
                        else
                        {
                            transform.Translate(Vector3.right * Time.deltaTime * speed);
                        }
                    }
                    else
                    {
                        transform.Translate(Vector3.right * Time.deltaTime * speed);
                    }

                }
                else
                {
                    transform.Translate(-Vector3.right * Time.deltaTime * speed);
                }

                RaycastHit2D hitD =
                    Physics2D.Raycast(transform.position - new Vector3(1f, 0.5f, 0), -Vector2.up, 5f);

                if (hitD.collider != null && !grounded && !jumpStarted && !hitGround)
                {
                    if (hitD.transform.gameObject.GetComponent<Spike>() != null)
                    {
                        spikeBelowStop = true;

                        //RaycastHit2D hitD2 =
                        //    Physics2D.Raycast(transform.position - new Vector3(2f, 0.5f, 0), -Vector2.up, 5f);

                        //if (hitD2.collider != null)
                        //{
                        //    if (hitD2.transform.gameObject.GetComponent<Spike>() == null)                           
                        //    {
                        //       spikeBelowStop = false;
                        //    }
                        //}

                    }
                }


                if (spikeBelowStop)
                {
                    hitD =
                        Physics2D.Raycast(transform.position - new Vector3(0f, 0.5f, 0), -Vector2.up, 10f);

                    if (!grounded && !jumpStarted)
                    {
                        if (hitD.collider == null)
                        {
                            spikeBelowStop = false;
                        }
                        else
                        {
                            if (hitD.transform.gameObject.GetComponent<Spike>() != null)
                            {
                                spikeBelowStop = false;
                            }
                            else
                            {
                                transform.position = new Vector3(hitD.transform.position.x, transform.position.y);
                            }
                        }
                    }
                }
            }
            else
            {
            //    RaycastHit2D hitD = Physics2D.Raycast(transform.position + new Vector3(-0, -0.5f, 0), -Vector2.up, 4f);
            //    if (hitD.collider != null)
            //    {
            //        if (hitD.transform.gameObject.GetComponent<Spike>() != null)
            //        {
            //            transform.Translate(Vector3.right * Time.deltaTime * speed);
            //            waitJump = false;
            //        }
            //    }
            }
        }

        if (GetComponent<Rigidbody2D>().velocity.x > maxVelocity.x)
            GetComponent<Rigidbody2D>().velocity = new Vector2(maxVelocity.x, GetComponent<Rigidbody2D>().velocity.y);


        if (GetComponent<Rigidbody2D>().velocity.y > maxVelocity.y)
            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, maxVelocity.y);

    }


    void StartJump(float time)
    {

        bool go = true;
        if (mapping == SetUpManager.MappingType.ReactionDelay)
        {
            go = false;
            if (inputDelayTimer >= inputDelay)
            {
                go = true;
                inputDelayTimer = 0;
            }
            else
            {
                inputDelayTimer += Time.deltaTime;
            }
        }

        if (!jumpStarted && go)
        {
            go = false;

            spikeBelowStop = false;
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
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 30);

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

                        if (Vector2.Distance(transform.position, cols[i].transform.position) > 25)
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
                            tDist -= yDist * 2;
                        }

                        if (tDist > 20)
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
                jumpTargetPos = nearestCol.position + new Vector3(0, 0.5f, 0);
                //    Debug.Log(nearestDist.ToString());

                RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(-0.2f, -0.5f, 0), -Vector2.up, 3f);
                if (hit.collider != null)
                {
                    lastJumpedOffPlat = hit.transform;
                }
            }
            else
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(-0.2f, -0.5f, 0), -Vector2.up, 3f);
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.GetComponent<Spike>()  == null)
                    {
                        doNotRetryPlats.Add(hit.transform);

                        if (!doNotColor)
                            hit.transform.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                    }

                }

                dirLeft = true;
                // failedPlats.Clear();           


                if (standingPlat != null)
                {
                    //  doNotRetryPlats.Add(standingPlat);
                    //  standingPlat.gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
                }



                if (lastJumpedOffPlat != null)
                {
                    //     doNotRetryPlats.Add(lastJumpedOffPlat);
                    //     lastJumpedOffPlat.transform.GetComponent<SpriteRenderer>().color = Color.red;
                }


                for (int i = 0; i < cols.Length; i++)
                {
                    if (cols[i].gameObject != this.gameObject)
                    {
                        bool inFailed = false;

                        foreach (var c in doNotRetryPlats)
                        {
                            if (c == cols[i].transform)
                                inFailed = true;
                        }

                        if (inFailed == false)
                        {
                            //angle check
                            Vector3 playerDir = cols[i].transform.position - transform.position;

                            float angle = Vector3.Dot(playerDir, -cols[i].transform.right);


                            if (angle > 2f)
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


                //  nearestCol.gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
                jumpTargetTransform = nearestCol.transform;

                jumpTargetPos = nearestCol.position;
                //    Debug.Log(nearestDist.ToString());

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
        if (jumpStarted)
        {
            return false;
        }

        RaycastHit2D hitD = Physics2D.Raycast(transform.position + new Vector3(0.25f, -0.5f, 0), -Vector2.up, 0.3f);
        if (hitD.collider != null)
        {
            if (hitD.transform.gameObject.GetComponent<Spike>() != null)
                return true;
        }

        hitD = Physics2D.Raycast(transform.position + new Vector3(-0.0f, -0.5f, 0), -Vector2.up, 0.2f);
        if (hitD.collider != null)
        {
            if (hitD.transform.gameObject.GetComponent<Spike>() != null)
                return true;
        }

        hitD = Physics2D.Raycast(transform.position + new Vector3(-0.25f, -0.5f, 0), -Vector2.up, 0.2f);
        if (hitD.collider != null)
        {
            if (hitD.transform.gameObject.GetComponent<Spike>() != null)
                return true;
        }
        return false;
    }


    public void ResetPlayer()
    {
        GetComponent<SpriteRenderer>().color = Color.green;
        health = maxHealth;
        invul = false;
        invulTimer = 0;

        spikeBelowStop = false;
        dirLeft = false;

        failedPlatfromList.Clear();
        doNotRetryPlats.Clear();

        lastJumpedOffPlat = null;
        jumpTargetTransform = null;

        jumpTargetHit = true;
        jumpHitIgnore = true;
        stopMoveRight = false;

        jumpTargetPos = startPos;

        waitJump = false;

        recordPosX = 0.45f;
        recordRepeat = false;

        prevInputVector.Clear();
        prevIterToCheck = 0;

        actions.Clear();
    }


    public void BackToLastTile()
    {
        if (lastJumpedOffPlat != null && jumpTargetTransform != null)
        {
            spikeBelowStop = false;
            dirLeft = false;
            jumpTargetHit = true;
            jumpHitIgnore = true;
            stopMoveRight = false;
            waitJump = false;

            foreach (var fp in failedPlatfromList)
            {
                if (fp.referenceTransform == lastJumpedOffPlat.transform)
                {
                    fp.AddFailedPlat(jumpTargetTransform.transform);
                }
            }

            transform.position = lastJumpedOffPlat.position + new Vector3(0, 1.1f, 0);
        }
        else
        {

        }

    }

    public SetUpManager.MappingType GetMapping()
    {
        return mapping;
    }


    public bool CheckInvul()
    {
        return invul;
    }


    public int GetHealth()
    {
        return health;
    }

    public void HealthDown()
    {
        health--;
        invul = true;
        GetComponent<SpriteRenderer>().color = Color.red;
        invulTimer = 0;
    }

    public void HealthUp()
    {
        if (health < maxHealth)
        {
            health++;
        }
    }

    public List<int> GetAllActions()
    {
        return actions;
    }


    public bool getJumpingUp()
    {
        if (jumpStarted)
        {
            return true;
        }

        return false;
    }
}