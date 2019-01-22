using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{

    public GameObject ground;
    public List<int> treeSequence = new List<int>();
    public List<int> branchChosen = new List<int>();
    public List<List<GameObject>> platformSequence = new List<List<GameObject>>();

    private Vector2 currentPos;

    private List<Color> colorList = new List<Color>();
    private int colorIter = 0;

    // Use this for initialization
    void Start ()
	{

        colorList.Add(Color.blue);
	    colorList.Add(Color.red);
	    colorList.Add(Color.green);
	    colorList.Add(Color.magenta);
	    colorList.Add(Color.gray);
        colorList.Add(Color.cyan);

        int rCount = 0;
	    int rMax = 0;
	    bool rStart = false;

	    for (int i = 0; i < 500; i++)
	    {
	        if (treeSequence.Count > 0)
	        {
	            if (Random.Range(0, 10) > 5)
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

            branchChosen.Add(0);
	    }

	    bool branch = false;
	    rCount = 0;
	    string tree = "";
	    for (int i = 0; i < treeSequence.Count; i++)
        {
	        if (treeSequence[i] == 1)
	        {
	            int selected = 0;

                if (rCount > 0)
	            {
	                branch = true;	                
                    //rCount--;
	                for (int j = i - 1; j > 0; j--)
	                {
	                    if (rCount > 0)
	                    {
	                        if (platformSequence[j].Count > 0)
	                        {
	                            currentPos = platformSequence[j][0].transform.position;
	                            rCount--;
	                            selected = j;
	                        }
	                    }

	                }

	                branchChosen[selected]++;

	                //currentPos = platformSequence[(i - rCount) - 1][platformSequence[(i - rCount) - 1].Count - 1].transform.position;
	                //currentPos += new Vector2(0, 1);
	            }

	            if (branch)
	            {
	                LoadSegement(true, branchChosen[selected]);
	            }
	            else
	            {
	                LoadSegement(false, 0);
                }

	            branch = false;
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


    void LoadSegement(bool branch, int branchNum)
    {
        List<GameObject> plats = new List<GameObject>();

        Vector2 addVec = new Vector2(0,0);

        if (branchNum == 1)
        {
            addVec = new Vector2(1, 1);
        }

        if (branchNum >= 2)
        {
            addVec = new Vector2(1, -1);
        }

        //if (branchNum == 3)
        //{
        //    addVec = new Vector2(-1, 1);
        //}

        //if (branchNum >= 4)
        //{
        //    addVec = new Vector2(-1, -1);
        //}

        for (int i = 0; i < 50; i++)
        {
            GameObject gobj = Instantiate(ground, currentPos + new Vector2(0, Random.Range(0, 0)),
                transform.rotation);

            if (branch)
            {
                gobj.GetComponent<SpriteRenderer>().color = Color.yellow;
                currentPos += addVec;
            }
            else
            {
                gobj.GetComponent<SpriteRenderer>().color = colorList[colorIter];
                currentPos += new Vector2(1, 0);
            }

            plats.Add(gobj);

            
        }

        colorIter++;
        if (colorIter >= colorList.Count)
            colorIter = 0;

        platformSequence.Add(plats);
    }
}


public class Brancher
{
    private bool rightUp;
    private bool rightDown;

    public Brancher(Transform reference)
    {
      
    }

    public Vector2 GetAvailibleDirection()
    {
       return Vector2.zero;
    }
}