using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTracker : MonoBehaviour
{
    private float failY;
    private float successX;
    public Transform player;

    private float lastX;
    private float startX;
    float stucktimer;

    private float attemptTimer;
    private float failTimer;
    private float fitness;

	// Use this for initialization
	void Start ()
	{
	    successX = GetComponent<LevelGenerator>().levelLength - 1;
	    failY = transform.position.y - 2;
	    lastX = player.transform.position.x - 100;
	    startX = lastX;
	}
	
	// Update is called once per frame
    void FixedUpdate()
    {
        attemptTimer += Time.deltaTime;

        failTimer += Time.deltaTime;
        if (failTimer > 120)
        {
            ResetCheck();
            failTimer = 0;
        }

        stucktimer += Time.deltaTime;
        if (stucktimer > 10)
        {
            StuckCheck();
            stucktimer = 0;
        }
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


    public float GetFitness()
    {
        return fitness;
    }


    public bool SuccessCheck()
    {
        if (player.transform.position.x > successX)
        {
            fitness = 
                //player.GetComponent<AITesterController>().GetAIMoveCount();
        //    Debug.Log("Completed Level Diffuculty: " + fitness.ToString());

            attemptTimer = 0;
            return true;
        }

        return false;
    }

    public bool FailCheck()
    {
        if (player.transform.position.y < failY)
        {
            attemptTimer = 0;
            stucktimer = 0;
            return true;
        }

        return false;
    }
}
