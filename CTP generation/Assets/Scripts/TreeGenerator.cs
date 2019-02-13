using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSegment
{
    public Vector2 position;
    public bool branch;
    public Vector2 branchDirection;

    public TreeSegment(Vector2 pos, bool isBranch, Vector2 branchDir)
    {
        position = pos;
        branch = isBranch;
        branchDirection = branchDir;
    }
}

public class TreeGenerator : MonoBehaviour
{

    public GameObject ground;
    public List<int> treeSequence = new List<int>();
    public List<int> branchChosen = new List<int>();
    public List<Brancher> brancherList = new List<Brancher>();
    public List<List<GameObject>> platformSequence = new List<List<GameObject>>();

    private Vector2 currentPos;

    private List<Color> colorList = new List<Color>();
    private int colorIter = 0;

    public int treeLength = 10;

    public List<TreeSegment> treePoints = new List<TreeSegment>();

    List<GameObject> disablePlats = new List<GameObject> ();

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

	    for (int i = 0; i < treeLength; i++)
	    {
	        if (treeSequence.Count > 0)
	        {
	            if (Random.Range(0, 100) > 25)
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
            brancherList.Add(new Brancher());
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

	                branch = true;
                    branchChosen[selected]++;

	                //currentPos = platformSequence[(i - rCount) - 1][platformSequence[(i - rCount) - 1].Count - 1].transform.position;
	                //currentPos += new Vector2(0, 1);
	            }

	            if (branch)
	            {
                    //check avalible directions, else skip
	                if (brancherList[selected].GetAvailibleDirection() != Vector2.zero)
	                {
	                    LoadSegement(true, selected);                    	                  
                    }
	                else
	                {
	                    platformSequence.Add(new List<GameObject>());
	                    rCount++;
	                    treeLength++;

	                }
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

	    foreach (var disPlat in disablePlats)
	    {
	        disPlat.SetActive(false);
	    }

        GetComponent<TreeGeneticGenAlg>().InitTreeGen(treePoints, 10, 50, 80);
    }
	
	// Update is called once per frame
	void Update ()
	{
		
	}


    void LoadSegement(bool branch, int branchNum)
    {
        List<GameObject> plats = new List<GameObject>();
        Vector2 addVec = brancherList[branchNum].GetAvailibleDirection();

     //   int levelLength = Random.Range(1, 4);

        for (int i = 0; i < 50; i++)
        {

            GameObject gobj = Instantiate(ground, currentPos + new Vector2(0, Random.Range(0, 0)), transform.rotation);

            if (branch)
            {
                // gobj.GetComponent<SpriteRenderer>().color = Color.yellow;

                if (i == 0)
                {
                    treePoints.Add(new TreeSegment(currentPos, true, addVec));
                }

                currentPos += addVec;
                disablePlats.Add(gobj); //comment to for full tree
            }
            else
            {

                if (i == 0)
                {
                    treePoints.Add(new TreeSegment(currentPos, false, Vector2.zero));
                }

                currentPos += new Vector2(1, 0);
                disablePlats.Add(gobj); //comment to for full tree
            }

            plats.Add(gobj);

            gobj.GetComponent<SpriteRenderer>().color = colorList[colorIter];

        }


        colorIter++;
        if (colorIter >= colorList.Count)
            colorIter = 0;

        platformSequence.Add(plats);
    }
}


public class Brancher
{
    private bool rightUp = false;
    private bool rightDown = false;

    public Vector2 GetAvailibleDirection()
    {
        if (!rightUp && !rightDown)
        {
            if (Random.Range(0, 10) > 4)
            {
                rightUp = false;
                return new Vector2(1, 1);
            }
            else
            {
                rightDown = false;
                return new Vector2(1, -1);
            }
        }

        if (rightDown)
        {
            rightUp = false;
            return new Vector2(1, -1);
        }

        if (rightUp)
        {
            rightDown = false;
            return new Vector2(1, 1);
        }

        return Vector2.zero;
    }
}