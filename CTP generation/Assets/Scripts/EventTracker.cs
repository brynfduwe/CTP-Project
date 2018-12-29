using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTracker : MonoBehaviour
{
    private float failY;
    private float successX;
    public Transform player;

	// Use this for initialization
	void Start ()
	{
	    successX = GetComponent<LevelGenerator>().levelLength - 1;
	    failY = transform.position.y - 2;
	}
	
	// Update is called once per frame
	void Update () {
		
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
            return true;
        }

        return false;
    }
}
