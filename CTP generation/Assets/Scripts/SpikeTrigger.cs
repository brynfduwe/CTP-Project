﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.GetComponent<SimpleAIController>() != null)
        {
            if (!col.gameObject.GetComponent<SimpleAIController>().getJumpingUp())
            {
            
                    col.transform.position -= new Vector3(0, 100, 10);
                
            }
        }
    }
}
