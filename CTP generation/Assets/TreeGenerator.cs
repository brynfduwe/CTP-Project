using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{

    public GameObject ground;
    public List<int> treeSequence = new List<int>();
    public List<List<GameObject>> platformSequence = new List<List<GameObject>>();

    private Vector2 currentPos;



    // Use this for initialization
    void Start ()
	{
	    int rCount = 0;
	    int rMax = 0;
	    bool rStart = false;

	    for (int i = 0; i < 100; i++)
	    {
	        if (treeSequence.Count > 0)
	        {
	            if (Random.Range(0, 10) > 1)
	            {
	                rCount++;

	                if (!rStart)
	                {
	                    rMax = treeSequence.Count;
	                    rStart = true;
	                }

	                if (rCount < rMax)
	                {
	                    treeSequence.Add(0);
	                }
	                else
	                {
	                    rCount = 0;
	                    treeSequence.Add(1);
                    }
	            }
	            else
	            {
	                rStart = false;
                    rCount = 0;
	                treeSequence.Add(1);
                }
	        }
	        else
	        {
	            treeSequence.Add(1);
	        }
	    }

	    rCount = 0;
	    string tree = "";
	    for (int i = 0; i < treeSequence.Count; i++)
        {
	        if (treeSequence[i] == 1)
	        {
	            if (rCount > 0)
	            {
	                for (int j = i - 1; j > 0; j--)
	                {
	                    if (rCount > 0)
	                    {
	                        if (platformSequence[j].Count > 0)
	                        {
	                            currentPos = platformSequence[j][0].transform.position;
	                            rCount--;
	                        }
	                    }

	                }

	                //currentPos = platformSequence[(i - rCount) - 1][platformSequence[(i - rCount) - 1].Count - 1].transform.position;
                    currentPos += new Vector2(0, 1);
	            }

	            LoadSegement();
	            rCount = 0;
	        }
	        else
	        {
	            platformSequence.Add(new List<GameObject>());
	            rCount++;
	        }


	        tree += treeSequence[i].ToString() + ", ";
	    }

	    Debug.Log(tree.ToString());
    }
	
	// Update is called once per frame
	void Update ()
	{
		
	}


    void LoadSegement()
    {
        List<GameObject> plats = new List<GameObject>();

        for (int i = 0; i < 50; i++)
        {
            GameObject gobj = Instantiate(ground, currentPos + new Vector2(0, Random.Range(0, 10)),
                transform.rotation);

            plats.Add(gobj);


            currentPos += new Vector2(1, 0);
        }

        platformSequence.Add(plats);
    }
}
