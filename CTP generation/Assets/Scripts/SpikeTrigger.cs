using System.Collections;
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
                if (col.gameObject.GetComponent<SimpleAIController>().GetMapping() != SetUpManager.MappingType.Health)
                {
                    col.transform.position -= new Vector3(0, 100, 10);
                }
                else
                {
                    if (col.gameObject.GetComponent<SimpleAIController>().GetHealth() > 1)
                    {
                        if (col.gameObject.GetComponent<SimpleAIController>().CheckInvul() == false)
                            col.gameObject.GetComponent<SimpleAIController>().HealthDown();
                    }
                    else
                    {
                        col.transform.position -= new Vector3(0, 100, 10);
                    }
                }
            }
        }
    }
}
