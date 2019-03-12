using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LevelGenerator : MonoBehaviour
{
    public GameObject ground;
    public GameObject endFlag;
    public GameObject spikePlat;
    public GameObject heartPlat;

    public Text transitionMatrixVis;

    // public string level;

    private List<GameObject> platsformObjects = new List<GameObject>();

    enum States
    {
        NoGround,
        Ground1,
    }

    public List<int[]> probabilityTransList = new List<int[]>();

    public int levelHeight = 10;

    public int stepHistory;
    private States currentState;
    private int xPos = 0;
    public int levelLength;

    public Transform player;
    private Vector2 startPlayerPos;

    private float restFreqLevel = 0;
    private float restAmountLevel = 0;

    public bool PlayerTesting;

    private int startX = 0;

    Vector2 enforceBranchDirection = Vector2.zero;

    // List<int> history = new List<int>();

    public List<Vector2> transitionIndex = new List<Vector2>();
    Vector2 currentIndex = new Vector2(0,0);

    // Use this for initialization
    void Start()
    {
        startX = (int)transform.position.x;
        if (PlayerTesting)
        {
            probabilityTransList = GameObject.Find("SceneManager").GetComponent<SceneManager>().GetChromo();
            SetNewChain(GameObject.Find("SceneManager").GetComponent<SceneManager>().GetChromo());
            //MyStart(GameObject.Find("SceneManager").GetComponent<SceneManager>().height, GameObject.Find("SceneManager").GetComponent<SceneManager>().length);
        }
    }


    public void MyStart(int height, int length, int transitions, SetUpManager.MappingType mapping, int historyStep, int stateAmount)
    {
        for (int i = 0; i < historyStep - 1; i++)
        {
        //    history.Add(0);
        }

        stepHistory = historyStep;

        player.GetComponent<SimpleAIController>().SetMapping(mapping);

        levelLength = length;
        levelHeight = height;

        startPlayerPos = player.transform.position;

        List<int> x = new List<int>();

        Debug.Log(transitions.ToString());

        for (int j = 0; j < transitions; j++)
        {
            x.Add(100 / transitions);
        }


        int iterA = 0;
        int iterB = 0;
        for (int j = 0; j < transitions; j++)
        {
            Vector2 index = new Vector2(iterA, iterB);
            transitionIndex.Add(index);

            if (iterB >= (stateAmount - 1))
            {
                iterA++;
                iterB = 0;
            }
            else
            {
                iterB++;
            }
        }


        probabilityTransList = new List<int[]>();
        for (int i = 0; i < transitions; i++)
        {
            // probabilityTransList.Add(new int[] { 1, 2, 3 });

            probabilityTransList.Add(x.ToArray());
        }

        RandomChain();

        if (PlayerTesting)
        {
            SetNewChain(GameObject.Find("SceneManager").GetComponent<SceneManager>().GetChromo());
        }

       // GenerateLevel();
        NewLevelCandidate();
    }


    void GenerateLevel()
    {
        player.gameObject.SetActive(true);
       
        player.transform.position = startPlayerPos + new Vector2(0, 2);

        if (player.GetComponent<SimpleAIController>() != null)
        {
            player.GetComponent<SimpleAIController>().ResetPlayer();
        }

        int xPos = (int)transform.position.x;
        int yPos = (int) transform.position.y;

        foreach (var plat in platsformObjects)
        {
            Destroy(plat.gameObject);
        }

        platsformObjects.Clear();

        for (int i = 0; i < levelLength; i++)
        {
            SwitchStates();

            if ((int) currentState < (levelHeight) * 2 && (int) currentState > levelHeight)
            {
                currentState = States.NoGround;
            }

            if (currentState != States.NoGround)
            {
                GameObject toSpawn = ground;
                int spawnY = yPos + (int) currentState - 1;

                if ((int) currentState >= (levelHeight) * 2 && (int)currentState  < (levelHeight) * 3)
                {
                    toSpawn = spikePlat;
                    spawnY = spawnY - (levelHeight * 2);
                }

                if ((int)currentState >= (levelHeight) * 3)
                {
                    toSpawn = heartPlat;
                    spawnY = spawnY - (levelHeight * 3);
                }

                GameObject plat = Instantiate(toSpawn, new Vector3(xPos, spawnY, 0),
                toSpawn.transform.rotation, transform);
                platsformObjects.Add(plat);
            }

            if (enforceBranchDirection.y == 1)
            {
                yPos++;
            }

            if (enforceBranchDirection.y == -1)
            {
                yPos--;
            }

                xPos++;
        }

        //end space
        if (currentState == States.NoGround)
        {
            currentState = States.Ground1;
        }

        int ypos = yPos + (int)currentState - 1; ;
        if ((int)currentState >= (levelHeight) * 2)
        {
            ypos = ypos - (levelHeight * 2);
        }
        if ((int)currentState >= (levelHeight))
        {
            ypos = ypos - (levelHeight);
        }

        GameObject end = Instantiate(endFlag, new Vector3(xPos, ypos, 0), ground.transform.rotation, transform);
        GameObject endPlus = Instantiate(ground, end.transform.position + new Vector3(1,0, 0), ground.transform.rotation, transform);
        GameObject endPlusPlus = Instantiate(ground, end.transform.position + new Vector3(2, 0, 0), ground.transform.rotation, transform);
        platsformObjects.Add(end);
        platsformObjects.Add(endPlus);
        platsformObjects.Add(endPlusPlus);


        //move player to start
        GameObject start = Instantiate(ground, new Vector3(platsformObjects[0].transform.position.x - 1, platsformObjects[0].transform.position.y), ground.transform.rotation, transform);
        player.transform.position = new Vector3(platsformObjects[0].transform.position.x - 1.5f, platsformObjects[0].transform.position.y + 1.5f);
        platsformObjects.Add(start);

        player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        GetComponent<EventTracker>().SetSuccess(end.transform);
    }


    void SwitchStates()
    {
        int r = Random.Range(0, 100);
        int iter = 0;
        int selectedTransition = 0;

        int trueState = 0;
        for (int i = 0; i < transitionIndex.Count; i++)
        {
            if (transitionIndex[i] == currentIndex)
            {
                trueState = i;
                Debug.Log(currentIndex + ": " + trueState);
            }
        }

        for (int i = 0; i < probabilityTransList[trueState].Length; i++)
        {
            if (iter < r)
            {
                iter += probabilityTransList[trueState][i];
                selectedTransition = i;
            }
        }

        currentIndex.x = currentIndex.y;
        currentIndex.y = transitionIndex[selectedTransition].y;

        currentState = (States)currentIndex.y;

     //   Debug.Log(currentIndex);
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
        //if (Input.GetKeyDown(KeyCode.Alpha0))
        //{
        //    NewLevelCandidate();
        //}

        //if (player.GetComponent<SimpleAIController>().SpikeCheck())
        //{
        //    player.transform.position -= new Vector3(0, 50, 0);
        //}
    }


    public List<int[]> GetGeneratorChromosome()
    {
        return probabilityTransList;
    }

    public void NewLevelCandidate()
    {
        GenerateLevel();

        if (!PlayerTesting)
        {
          //  StartCoroutine(WaitAndDiffCheck());
        }
        else
        {
            GetComponent<LevelDetailAnalyser>().CheckLevelDifficulty(platsformObjects.ToArray());
        }
    }


    IEnumerator WaitAndDiffCheck()
    {
        while (true)
        {
            yield return new WaitForSeconds(5 * Time.timeScale);

            if (GameObject.FindGameObjectWithTag("GAManager").GetComponent<GeneticAlgManager>().generation >= 1)
            {
            //    CheckDiffucultySuccess();
            }
        }
    }

    public bool CheckDiffucultySuccess()
    {
        this.GetComponent<DifficultyTracker>().CheckLevelDifficulty(platsformObjects.ToArray());

        bool fail = false;

        float restsNumGoal = (levelLength / 5) * restFreqLevel;
        float boundrySplit = ((levelLength / 5));

        float candidateRest = ((this.GetComponent<DifficultyTracker>().GetRestAvgLength() *
                                this.GetComponent<DifficultyTracker>().GetRestNumOf())); // / levelLength) * 100;???

        if (candidateRest < restsNumGoal - boundrySplit || candidateRest > restsNumGoal + boundrySplit)
        {
            player.transform.position = -new Vector2(0, 20);
            return false;
        }

        return true;
    }

    public void SetNewChain(List<int[]> chain)
    {
        for (int i = 0; i < chain.Count; i++)
        {
            probabilityTransList[i] = chain[i];
        }
    }


    public void SetRests(float _restFreq)
    {
        ////IDK
        restFreqLevel = _restFreq;
    }


    public void SetBranchDirection(Vector2 dir)
    {
        enforceBranchDirection = dir;
    }


    public void LockPlayer()
    {
        player.gameObject.SetActive(false);
    }


    public bool CheckPlayerLocked()
    {
        if (player.gameObject.activeSelf == false)
        {
            return true;
        }

        return false;
    }
}