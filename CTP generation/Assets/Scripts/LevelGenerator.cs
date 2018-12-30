using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelGenerator : MonoBehaviour
{
    public GameObject ground;
    public GameObject endFlag;

    public Text transitionMatrixVis;

    // public string level;

    private List<GameObject> platsformObjects = new List<GameObject>();

    enum States
    {
        NoGround,
        Ground1,
        Ground2,
        Ground3,
        Ground4,
        Ground5,
        Ground6,
        Ground7,
        Ground8,
        Ground9
    }


    private List<List<int>> probablitiyLists = new List<List<int>>();
    public int probTransitionNum = 10;

    private States currentState;
    private int xPos = 0;
    public int levelLength;

    public Transform player;
    private Vector2 startPlayerPos;

    // Use this for initialization
    void Awake()
    {
        startPlayerPos = player.transform.position;

        currentState = (States)Random.Range(1, 7);

        //chain set up
        for (int i = 0; i < probTransitionNum; i++)
        {
            List<int> probs = new List<int>();

            for (int j = 0; j < probTransitionNum; j++)
            {
                probs.Add(100 / probTransitionNum);
            }
            probablitiyLists.Add(probs);
        }

        RandomChain();
        GenerateLevel();
    }


    void GenerateLevel()
    {
        player.transform.position = startPlayerPos;

        if (player.GetComponent<AITesterController>() != null)
        {
            player.GetComponent<AITesterController>().resetPlayer();
        }

        int xPos = 0;

        foreach (var plat in platsformObjects)
        {
            Destroy(plat.gameObject);
        }

        platsformObjects.Clear();

        for (int i = 0; i < levelLength; i++)
        {
            SwitchStates();

            if (currentState != States.NoGround)
            {
                GameObject plat = Instantiate(ground, new Vector3(xPos, transform.position.y + (int) currentState, 0),
                    ground.transform.rotation, transform);
                platsformObjects.Add(plat);
            }

            xPos++;
        }

        //end space
        if (currentState == States.NoGround)
        {
            currentState = States.Ground1;
        }
        GameObject end = Instantiate(endFlag, new Vector3(xPos, transform.position.y + (int)currentState, 0), ground.transform.rotation, transform);
        platsformObjects.Add(end);

        //move player to start
        GameObject start = Instantiate(ground, new Vector3(platsformObjects[0].transform.position.x - 1, platsformObjects[0].transform.position.y), ground.transform.rotation, transform);
        player.transform.position = new Vector3(platsformObjects[0].transform.position.x - 1, platsformObjects[0].transform.position.y + 1);
        platsformObjects.Add(start);


        transitionMatrixVis.text = "";
        foreach (var probl in probablitiyLists)
        {
            foreach (var i in probl)
            {
                transitionMatrixVis.text += (i.ToString() + ", ");
            }
            transitionMatrixVis.text += "\n";
        }
    }


    void SwitchStates()
    {
        int r = Random.Range(0, 100);
        int iter = 0;
        int selectedTransition = 0;

        for (int i = 0; i < probablitiyLists.Count; i++)
        {
            if (iter < r)
            {
                iter += probablitiyLists[(int) currentState][i];
                selectedTransition = i;
            }
        }

        currentState = (States) selectedTransition;
    }

    public void RandomChain()
    {
        foreach (var pl in probablitiyLists)
        {
            int leftVal = 100;

            for (int i = 0; i < pl.Count - 1; i++)
            {
                int r = Random.Range(0, leftVal);
                leftVal -= r;
                pl[i] = r;
            }
            pl[pl.Count - 1] = leftVal;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            NewLevelCandidate();
        }
    }


    public List<List<int>> GetGeneratorChromosome()
    {
        return probablitiyLists;
    }

    public void NewLevelCandidate()
    {
        GenerateLevel();
    }

    public void SetNewChain(List<List<int>> chain)
    {
        currentState = (States)Random.Range(1, 7);

        probablitiyLists.Clear();
        probablitiyLists = new List<List<int>>(chain);

     //   Debug.Log("lgnth: " + probablitiyLists.Count.ToString());

        for (int i = 0; i < probablitiyLists.Count; i++)
        {
            foreach (var c in probablitiyLists[i])
            {
              //  Debug.Log(i.ToString() + ": " + c.ToString());
            }
        }
    }
}
