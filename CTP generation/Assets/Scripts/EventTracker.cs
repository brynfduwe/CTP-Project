using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTracker : MonoBehaviour
{
    private float failY;
    private float successX;
    public Transform player;

    private float lastX;
    float stucktimer;

	// Use this for initialization
	void Start ()
	{
	    successX = GetComponent<LevelGenerator>().levelLength - 1;
	    failY = transform.position.y - 2;
	    lastX = player.transform.position.x - 100;
    }
	
	// Update is called once per frame
    void FixedUpdate()
    {
        stucktimer += Time.deltaTime;
        if (stucktimer > 5)
        {
            StuckCheck();
            stucktimer = 0;
        }
    }


    void StuckCheck()
    {
        if (player.transform.position.x > lastX - 0.1f && player.transform.position.x < lastX + 0.1f)
        {
            player.transform.position = new Vector3(transform.position.x, failY - 1);
            lastX = player.transform.position.x - 100;
        }

            lastX = player.transform.position.x;
        
    }


    public bool SuccessCheck()
    {
        if (player.transform.position.x > successX)
        {
            return true;
        }

        return false;
    }

    public bool FailCheck()
    {
        if (player.transform.position.y < failY)
        {
            stucktimer = 0;
            return true;
        }

        return false;
    }
}
