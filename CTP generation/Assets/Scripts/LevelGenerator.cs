using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    public GameObject ground;
    public GameObject groundB;
    public GameObject endFlag;
    public GameObject spikePlat;
    public GameObject heartPlat;

    public Text transitionMatrixVis;

    // public string level;

    private List<GameObject> platsformObjects = new List<GameObject>();
    private List<GameObject> additPlatsformObjects = new List<GameObject>();

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

    List<int> history = new List<int>();

    public List<int[]> transitionIndex = new List<int[]>();
    public List<string> transitionIndexString = new List<string>();

    private int[] currentIndex;

    public int[] manualTransitionPath;

    // Use this for initialization
    void Start()
    {
        startX = (int)transform.position.x;
    }


    public void MyStart(int height, int length, int transitions, SetUpManager.MappingType mapping, int historyStep, int stateAmount)
    {
        for (int i = 0; i < historyStep - 1; i++)
        {
        //    history.Add(0);
        }

        stepHistory = historyStep + 1;

        currentIndex = new int[stepHistory];

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


        for (int j = 0; j < transitions; j++)
        {
            var idx = Convert.ToString(j, stateAmount);

            string addon = "";
            for (int i = idx.Length - stepHistory; i < 0; i++)
            {
                addon += '0';
            }

            transitionIndexString.Add(addon + idx);
        }


        probabilityTransList = new List<int[]>();
        for (int i = 0; i < transitions; i++)
        {
            // probabilityTransList.Add(new int[] { 1, 2, 3 });

            probabilityTransList.Add(x.ToArray());
        }

        RandomChain();

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
        history.Clear();
        for (int i = 0; i < levelLength; i++)
        {
            if (!PlayerTesting)
            {
                history.Add(currentIndex[currentIndex.Length - 1]);
                SwitchStates();
            }
            else
            {
                currentState = (States)manualTransitionPath[i];
            }       


            if ((((int) currentState < (levelHeight) * 2 && (int) currentState > levelHeight)))
            {
                currentState = States.NoGround;
            }

            if ((int)currentState >= (levelHeight) * 3)
            {
                currentState = States.NoGround;
            }


            if (currentState != States.NoGround)
            {
                GameObject toSpawn = ground;
                int spawnY = yPos + (int) currentState - 1;

                if ((int) currentState > levelHeight)
                {
                    spawnY = yPos + (int)currentState - (levelHeight);
                    spawnY = spawnY - 1;
                }

                if ((int) currentState > (levelHeight) * 2)
                {
                    toSpawn = spikePlat;
                    spawnY = yPos + (int)currentState - (levelHeight * 2);
                    spawnY = spawnY - 1;
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

        foreach (var plt in additPlatsformObjects)
        {
            Destroy(plt);
        }

        additPlatsformObjects.Clear();

        //
        foreach (var plt in platsformObjects)
        {
            for (int i = (int)plt.transform.position.y - 1; i >= transform.position.y; i--)
            {
                GameObject plat = Instantiate(groundB, new Vector3(plt.transform.position.x, i), ground.transform.rotation, transform);
                additPlatsformObjects.Add(plat);
            }
        }
    }


    void SwitchStates()
    {
        int r = Random.Range(0, 100);
        int iter = 0;
        int selectedTransition = 0;

        int trueState = 0;

        string currentIdxStr = "";
        foreach (var id in currentIndex)
        {
            currentIdxStr += id;
        }
       // Debug.Log(currentIdxStr);

        for (int i = 0; i < transitionIndexString.Count; i++)
        {
            if (transitionIndexString[i] == currentIdxStr)
            {
                trueState = i;
             //   Debug.Log("TS: " + trueState);
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

        //IDK WIERD ISSUE
        int stateString = (transitionIndexString[selectedTransition][currentIndex.Length - 1] - 48);

      //  currentIndex.x = currentIndex.y;
     //   currentIndex.y = transitionIndex[selectedTransition].y;

        List<int> newIndex = new List<int>();
        for (int i = 0; i < currentIndex.Length - 1; i++)
        {
            newIndex.Add(currentIndex[i + 1]);
        }
        newIndex.Add(stateString);

        currentIndex = newIndex.ToArray();
        currentState = (States)currentIndex[currentIndex.Length - 1];
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


    public List<int[]> GetGeneratorChromosome()
    {
        return probabilityTransList;
    }

    public void NewLevelCandidate()
    {
        GenerateLevel();
    }


    IEnumerator WaitAndDiffCheck()
    {
        while (true)
        {
            yield return new WaitForSeconds(5 * Time.timeScale);
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


    public void SetTransitionPath(int[] path, bool usePath)
    {
        manualTransitionPath = path;
        PlayerTesting = true;
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

    public int[] getHistory()
    {
        return history.ToArray();
    }
}