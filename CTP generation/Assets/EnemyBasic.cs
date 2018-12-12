using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBasic : MonoBehaviour
{

    public float speed;

    private float turnAroundTimer;
    private bool turning;

    public Transform groundPoint1;
    public Transform groundPoint2;

    private bool dirR = false;
    private Vector2 dir = Vector2.left;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	    GetComponent<Rigidbody2D>().AddForce(dir * speed, ForceMode2D.Force);

	    if (Physics2D.OverlapArea(groundPoint1.position, groundPoint2.position) && turning == false)
	    {
	        turning = true;
            transform.Rotate(Vector3.up, 180);
            dirR = !dirR;

	        if (dirR)
	        {
	            dir = Vector2.right;
	        }
	        else
	        {
	            dir = Vector2.left;
	        }
	    }

	    if (turning)
	    {
	        turnAroundTimer += Time.deltaTime;

	        if (turnAroundTimer > 1)
	        {
	            turning = false;
	            turnAroundTimer = 0;
	        }
	    }
	}
}
