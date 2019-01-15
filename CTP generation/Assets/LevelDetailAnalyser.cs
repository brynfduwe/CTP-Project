using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelDetailAnalyser : MonoBehaviour
{

    public GameObject AI;

    private int rests = 0; // platform of 3 or more
    private List<int> restLengths = new List<int>(); // number of platforms in that rest
    private int rhythmGroups = 0;
    private List<int> rhythmGroupLengths = new List<int>();
    private List<List<GameObject>> rhythmGroupTransforms = new List<List<GameObject>>();

    private float fitness = 0;

    ///-
    private float restsNumOf;
    private float restLengthAvg;

    public List<GameObject> AITesters = new List<GameObject>();
    public List<bool> AIsuccesses = new List<bool>();



    // Use this for initialization
    void Start ()
    {

        Time.timeScale = 3;

    }
	
	// Update is called once per frame
	void Update ()
	{
	    bool fail = false;

        if (AITesters.Count > 0)
	    {
	        for (int i = 0; i < AITesters.Count; i++)
	        {
	            if (AITesters[i].transform.position.x >
	                rhythmGroupTransforms[i][rhythmGroupTransforms[i].Count - 1].transform.position.x)
	            {
	                AIsuccesses[i] = true;
	                AITesters[i].gameObject.SetActive(false);
                }
	        }

	        foreach (var sucess in AITesters)
	        {
	            if (sucess.activeSelf == true)
	            {
	                fail = true;
	            }
	        }

	        if (fail == false)
	        {
	            foreach (var ait in AITesters)
	            {
	                ait.gameObject.SetActive(false);
	            }

	            AITesters.Clear();


	            foreach (var rgt in rhythmGroupTransforms)
	            {
	                foreach (var t in rgt)
	                {
	                    if (t.gameObject.GetComponent<SpriteRenderer>().color != Color.cyan)
	                    {
	                        t.gameObject.GetComponent<SpriteRenderer>().color = Color.red;

	                    }
	                }
	            }
	        }
	    }
	}

    public void CheckLevelDifficulty(GameObject[] levelPlats)
    {
        foreach (var ait in AITesters)
        {
            ait.gameObject.SetActive(false);
        }

        AITesters.Clear();

        rests = 0;
        rhythmGroups = 0;

        restLengths.Clear();
        rhythmGroupLengths.Clear();

        rhythmGroupTransforms.Clear();

        bool resting = false;

        int restCount = 0;
        int rbmCount = 0;

        bool newList = false;

        for (int i = 0; i < levelPlats.Length; i++)
        {
            if (i > 0)
            {
                resting = false;
                if (Vector2.Distance(new Vector2(levelPlats[i - 1].transform.position.x, 0),
                        new Vector2(levelPlats[i].transform.position.x, 0)) < 1.25f)
                {
                    if (Vector2.Distance(new Vector2(0, levelPlats[i - 1].transform.position.y),
                            new Vector2(0, levelPlats[i].transform.position.y)) < 0.2f)
                    {
                        resting = true;
                    }
                }

                if (resting)
                {
                    restCount++;

                    //check to log rest
                    if (rbmCount > 0)
                    {
                        rhythmGroups++;
                        rhythmGroupLengths.Add(rbmCount + 1);
                     
                        rbmCount = 0;

                             newList = false;
                    }
                }
                else
                {
                    rbmCount++;
                    if (!newList)
                    {
                        rhythmGroupTransforms.Add(new List<GameObject>());
                        if (levelPlats[i - 1].gameObject != null)
                        {
                            rhythmGroupTransforms[rhythmGroupTransforms.Count - 1].Add(levelPlats[i - 1]);
                        }

                        newList = true;
                    }

                    rhythmGroupTransforms[rhythmGroupTransforms.Count - 1].Add(levelPlats[i]);
                    

                    //check to log rest
                    if (restCount > 0)
                    {
                        rests++;
                        restLengths.Add(restCount + 1);

                        restCount = 0;
                    }
                }
            }
        }

        string s = "";
        foreach (var rl in restLengths)
        {
            s += rl.ToString();
            s += ", ";
        }

        string r = "";
        foreach (var rl in rhythmGroupLengths)
        {
            r += rl.ToString();
            r += ", ";
        }

        //   Debug.Log("Diff Rests: " + rests.ToString() + " - " + s + " /// Diff Rhg: " + rhythmGroups + " - " + r);

        int rlScore = 0;
        int rglScore = 0;

        foreach (var rl in restLengths)
        {
            rlScore += rl;
        }

        restsNumOf = restLengths.Count;

        restLengthAvg = rlScore / restLengths.Count;

        foreach (var rgl in rhythmGroupLengths)
        {
            rglScore += rgl;
        }

        //   Debug.Log("FITNESS DIFFUCICULTY: " + fitness.ToString());
        //DifficultyUI.text = restsNumOf.ToString();



        AIsuccesses.Clear();
        
        foreach (var rgt in rhythmGroupTransforms)
        {
            foreach (var t in rgt)
            {
                t.gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
            }

            GameObject ait = Instantiate(AI, rgt[0].transform.position + new Vector3(0, 1), transform.rotation);
            AITesters.Add(ait);
            AIsuccesses.Add(false);
            ait.GetComponent<AITesterController>().doNotColor = true;
        }
    }
}
