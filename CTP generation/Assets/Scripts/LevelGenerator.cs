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
    }

    public List<int[]> probabilityTransList = new List<int[]>();

    public int transitionNum = 10; 

    private States currentState;
    private int xPos = 0;
    public int levelLength;

    public Transform player;
    private Vector2 startPlayerPos;

    private float restFrequency = 0;
    private float restAmount = 0;

    // Use this for initialization
    void Awake()
    {
        startPlayerPos = player.transform.position;

        List<int> x = new List<int>();
 
            for (int j = 0; j < transitionNum; j++)
            {
                x.Add(100 / transitionNum);
            }

        probabilityTransList = new List<int[]>();
        for (int i = 0; i < transitionNum; i++)
        {
          // probabilityTransList.Add(new int[] { 1, 2, 3 });

           probabilityTransList.Add(x.ToArray());
        }

        RandomChain();
        GenerateLevel();
    }


    void GenerateLevel()
    {
        player.transform.position = startPlayerPos + new Vector2(0, 2);

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
                GameObject plat = Instantiate(ground, new Vector3(xPos, transform.position.y + (int)currentState, 0),
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
        GameObject endPlus = Instantiate(ground, end.transform.position + new Vector3(1,0, 0), ground.transform.rotation, transform);
        platsformObjects.Add(end);
        platsformObjects.Add(endPlus);


        //move player to start
        GameObject start = Instantiate(ground, new Vector3(platsformObjects[0].transform.position.x - 1, platsformObjects[0].transform.position.y), ground.transform.rotation, transform);
        player.transform.position = new Vector3(platsformObjects[0].transform.position.x - 1, platsformObjects[0].transform.position.y + 2f);
        platsformObjects.Add(start);


        //transition matrix visualisation
        transitionMatrixVis.text = "";

        foreach (var ptl in probabilityTransList)
        {
            foreach (var i in ptl)
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

        for (int i = 0; i < probabilityTransList[(int) currentState].Length; i++)
        {
            if (iter < r)
            {
                iter += probabilityTransList[(int) currentState][i];
                selectedTransition = i;
            }
        }

        currentState = (States)selectedTransition;               
    }


    public void RandomChain()
    {
        foreach (var pa in probabilityTransList)
        {
            int leftVal = 100;
            int decremter = probabilityTransList.Count - 1;
            List<bool> usedCheck = new List<bool>();

            foreach (var x in pa)
            {
                usedCheck.Add(false);
            }

            while (decremter >= 0)
            {
                int rT = Random.Range(0, pa.Length);
                while (usedCheck[rT] == true)
                {
                    rT = Random.Range(0, pa.Length);
                }

                int r = Random.Range(0, leftVal + 1);
                leftVal -= r;
                //  usedList.Add(rT);
                pa[rT] = r;
                usedCheck[rT] = true;
                decremter--;
            }

            // probablitiyLists[i] = new List<int>(usedList);
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


    public List<int[]> GetGeneratorChromosome()
    {
        return probabilityTransList;
    }

    public void NewLevelCandidate()
    {
        GenerateLevel();

        this.GetComponent<DifficultyTracker>().CheckLevelDifficulty(platsformObjects.ToArray());
        if (GetComponent<DifficultyTracker>().GetFitness() < 3 || GetComponent<DifficultyTracker>().GetFitness() > 20)
        {
            player.transform.position = -new Vector2(0, 20);
        }
    }

    public void SetNewChain(List<int[]> chain)
    {
        for (int i = 0; i < chain.Count; i++)
        {
            probabilityTransList[i] = chain[i];
        }
    }


    public void SetRests(float _restFreq, float _restAmount)
    {
        ////IDK
        restFrequency = restAmount * (levelLength / 10);
        this.restAmount =_restAmount * (levelLength / 10);
    }
}