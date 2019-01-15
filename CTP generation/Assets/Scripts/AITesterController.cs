using System.Collections;
using System.Collections.Generic;
using System.Linq;
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


    // Use this for initialization
    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity / 2;
        GetComponent<Rigidbody2D>().angularVelocity = GetComponent<Rigidbody2D>().angularVelocity / 2;

        jumpTargetPos = transform.position;
        startPos = transform.position + new Vector3(-1, -10);
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (doNotColor)
        {

            RaycastHit2D hitU = Physics2D.Raycast(transform.position - new Vector3(0f, 0.6f, 0), -Vector2.up, 0.3f);
            if (hitU.collider != null)
            {
                hitU.transform.gameObject.GetComponent<SpriteRenderer>().color = Color.cyan;
            }
        }

        grounded = Physics2D.OverlapArea(groundPoint1.position, groundPoint2.position);

        if (grounded)
        {
            if (!hitGround)
            {
                GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity / 2;
                GetComponent<Rigidbody2D>().angularVelocity = GetComponent<Rigidbody2D>().angularVelocity / 2;

                hitGround = true;
            }

            if (!dirLeft)
            {
                RaycastHit2D hitD = Physics2D.Raycast(transform.position + new Vector3(0.7f, 0, 0), -Vector2.up, 3f);
                if (hitD.collider == null)
                {
                    StartJump(0.3f);
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

                RaycastHit2D hitR = Physics2D.Raycast(transform.position + new Vector3(1, 0, 0), Vector2.right, 0.05f);
                if (hitR.collider != null)
                {
                    StartJump(0.15f);
                }

                RaycastHit2D hitR2 = Physics2D.Raycast(transform.position + new Vector3(1, 1, 0), Vector2.right, 1f);
                if (hitR2.collider != null)
                {
                    StartJump(0.3f);
                }
            }
            else
            {
                RaycastHit2D hitD = Physics2D.Raycast(transform.position + new Vector3(-0.5f, 0, 0), -Vector2.up, 3f);
                if (hitD.collider == null)
                {
                    StartJump(0.3f);
                }

                RaycastHit2D hitR =
                    Physics2D.Raycast(transform.position + new Vector3(-1, 0, 0), -Vector2.right, 0.05f);
                if (hitR.collider != null)
                {
                    StartJump(0.15f);
                }

                RaycastHit2D hitR2 = Physics2D.Raycast(transform.position + new Vector3(-1, 1, 0), -Vector2.right, 1f);
                if (hitR2.collider != null)
                {
                    StartJump(0.3f);
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
             //   stopMoveRight = true;
              //  jumpTargetHit = true;
             //   jumpHitIgnore = true;

           ///     transform.position = transform.position - new Vector3(0.2f, 0.2f);

           //     transform.GetComponent<Rigidbody2D>().velocity = Vector3.zero;

          //      Debug.Log("TOP HIT");
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

          //  RaycastHit2D hitCeiling = Physics2D.Raycast(transform.position + new Vector3(0, 0.5f, 0), Vector2.up, 0.25f);
          //  if (hitCeiling.collider != null)
       //     {
       //         jumpStarted = false;
               // jumpTargetHit = true;
               // jumpHitIgnore = true;
                //stopMoveRight = false;
       //     }

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
            if (Vector2.Distance(transform.position, jumpTargetPos) < 1)
            {
                dirLeft = false;
                stopMoveRight = false;
                jumpTargetHit = true;
                jumpHitIgnore = true;

                standingPlat = jumpTargetTransform;

                //RaycastHit2D hitD = Physics2D.Raycast(transform.position + new Vector3(0f, -0.5f, 0), -Vector2.up, 3f);
                //if (hitD.collider != null)
                //{
                //    //    hitD = Physics2D.Raycast(hitD.ptoin + new Vector3(0f, -0.5f, 0), -Vector2.up, 3f);

                //    doNotRetryPlats.Add(hitD.transform);
                //    hitD.transform.GetComponent<SpriteRenderer>().color = Color.red;
                //}

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
                else
                {
                    transform.Translate(-Vector3.right * Time.deltaTime * speed);
                }
            }
        }


        if (GetComponent<Rigidbody2D>().velocity.x > maxVelocity.x)
            GetComponent<Rigidbody2D>().velocity = new Vector2(maxVelocity.x, GetComponent<Rigidbody2D>().velocity.y);


        if (GetComponent<Rigidbody2D>().velocity.y > maxVelocity.y)
            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, maxVelocity.y);


        RaycastHit2D hitFU = Physics2D.Raycast(transform.position + new Vector3(1f, 0f, 0), Vector2.up);
        RaycastHit2D hitFR = Physics2D.Raycast(transform.position + new Vector3(1f, 0f, 0), Vector2.right);
        if (hitFU.collider != null)
        {
            if (hitFR.collider != null)
            {
              //  StartJump(0.3f);
            }
        }
    }


    public void StuckTryThing()
    {
    //    dirLeft = true;

      //--//  failedPlats.Add(jumpTargetTransform);

        //TODO - DONT DO BELOW, ADD TO NEW LIST THAT DOESNT ALLOW IT TO BE TARGETED FOR X TRIES??? MAYBE
       // doNotRetryPlats.Add(jumpTargetTransform);
       // jumpTargetTransform.gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;

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
                //    hitD = Physics2D.Raycast(hitD.ptoin + new Vector3(0f, -0.5f, 0), -Vector2.up, 3f);

                failedPlatfromList.Add(new PlatformCheckClass(hitD.transform));

                if(!doNotColor)
                    hitD.transform.GetComponent<SpriteRenderer>().color = Color.yellow;
                
            }



            int possibleRoutes = 0;
            //target platform picker.
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 10);

            Transform nearestCol = cols[0].transform;
            float nearestDist = 100;

            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i].gameObject != this.gameObject)
                {
                    bool inFailed = false;

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

                            //                 break;
                        }
                    }

                    foreach (var c in doNotRetryPlats)
                    {
                        if (c == cols[i].transform)
                            inFailed = true;
                    }

                    if (Vector2.Distance(transform.position, cols[i].transform.position) > 8)
                    {
                        inFailed = true;
                    }

                    if (Vector2.Distance(new Vector2(transform.position.x, 0),
                            new Vector2(cols[i].transform.position.x, 0)) > 5)
                    {
                        //   if(transform.position.y < cols[i].transform.position.y)
                    //    inFailed = true;
                    }

                    if (Vector2.Distance(new Vector2(0, transform.position.y),
                            new Vector2(0, cols[i].transform.position.y)) > 5 &&
                        transform.position.y < cols[i].transform.position.y)
                    {
                     //   inFailed = true;
                    }


                    float xDist = Vector2.Distance(new Vector2(transform.position.x, 0), new Vector2(cols[i].transform.position.x, 0));
                    float yDist = Vector2.Distance(new Vector2(0, transform.position.y), new Vector2(0, cols[i].transform.position.y));
                    float tDist = xDist;

                    if (cols[i].transform.position.y > transform.position.y - 1)
                    {
                        tDist += yDist;
                    }
                    else
                    {
                        tDist -= yDist * 1.5f;
                    }

                    if (tDist > 6)
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
                            float dist = Vector2.Distance(transform.position, cols[i].transform.position);
                            if (dist < nearestDist)
                            {
                                nearestDist = Vector2.Distance(transform.position, cols[i].transform.position);
                                nearestCol = cols[i].transform;
                            }

                            possibleRoutes++;
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
            else
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(-0.2f, -0.5f, 0), -Vector2.up, 3f);
                if (hit.collider != null)
                {
                    doNotRetryPlats.Add(hit.transform);

                    if (!doNotColor)
                        hit.transform.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                    
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
                                float dist = Vector2.Distance(transform.position, cols[i].transform.position);
                                if (dist < nearestDist)
                                {
                                    nearestDist = Vector2.Distance(transform.position, cols[i].transform.position);
                                    nearestCol = cols[i].transform;
                                }

                                possibleRoutes++;
                            }
                        }
                    }
                }


              //  nearestCol.gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
                jumpTargetTransform = nearestCol.transform;
                jumpTargetPos = nearestCol.position;
                //    Debug.Log(nearestDist.ToString());

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

            if (nearestDist < 1)
            {
                //short jump
                jumpTime = 0.1f;
            }
            else if(nearestDist < 2.5f)
            {
                jumpTime = 0.2f;
            }
            else
            {
                //highjump
                jumpTime = 0.35f;
            }

            MoverCounter+=nearestDist;

            // jumpTime = time;
        }


        RaycastHit2D hitU = Physics2D.Raycast(transform.position + new Vector3(0.5f, 0.0f, 0), Vector2.up);
        if (hitU.collider != null)
        {
            waitJump = true;
        }

        if (!doNotColor)
            GetComponent<SpriteRenderer>().color = Color.green;
    }


    public void resetPlayer()
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
    }


    public float GetAIMoveCount()
    {
        return MoverCounter;

        // Debug.Log("Completed Level Diffuculty: " + MoverCounter.ToString());
    }
}

public class PlatformCheckClass
{
    public Transform referenceTransform;
    List<Transform> failedJumpToPlats = new List<Transform>();

    public PlatformCheckClass(Transform reference)
    {
        referenceTransform = reference;
    }

    public void AddFailedPlat(Transform fail)
    {
        failedJumpToPlats.Add(fail);
    }


    public List<Transform> GetFailedPlats()
    {
        return failedJumpToPlats;
    }
}
