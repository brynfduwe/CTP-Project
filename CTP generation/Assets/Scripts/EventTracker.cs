﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTracker : MonoBehaviour
{
    private float failY;
    private Vector2 successXY;
    public Transform player;

    private float lastX;
    private float startX;
    float stucktimer;

    private float attemptTimer;
    private float failTimer;

	// Use this for initialization
	void Start ()
	{
	    failY = transform.position.y - 2;
	    lastX = player.transform.position.x - 100;
	    startX = lastX;
	}
	
	// Update is called once per frame
    void FixedUpdate()
    {

        stucktimer += Time.deltaTime;
        if (stucktimer > 10)
        {
            StuckCheck();
            stucktimer = 0;
        }
    }


    public void SetSuccess(Transform success)
    {
        successXY = success.position;
    }


    void StuckCheck()
    {
        if (player.transform.position.x > lastX - 0.1f && player.transform.position.x < lastX + 0.1f)
        {
            // player.GetComponent<AITesterController>().StuckTryThing();
            player.transform.position = new Vector3(transform.position.x, failY - 1);

            lastX = startX;
        }

         lastX = player.transform.position.x;
    }



    void ResetCheck()
    {
         if (player.transform.position.x > lastX - 2.5f && player.transform.position.x < lastX + 0)
        {
            player.transform.position = new Vector3(transform.position.x, failY - 1);

            lastX = startX;
        }

        lastX = player.transform.position.x;
    }

    public bool SuccessCheck()
    {
        if (player.transform.position.x >= successXY.x - 1 && player.transform.position.y > failY)  
        {
            return true;
        }

        return false;
    }


    public bool FailCheck()
    {
        if (player.transform.position.y < failY)
        {
            //  player.GetComponent<SimpleAIController>().BackToLastTile();
            return true;
        }

        return false;
    }
}
