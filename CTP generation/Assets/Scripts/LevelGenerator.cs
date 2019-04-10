using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    enum States
    {
        NoGround,
        Ground1,
    }

    public bool PlayerTesting;

    public GameObject ground;
    public GameObject groundB;
    public GameObject endFlag;
    public GameObject spikePlat;

    public List<int[]> probabilityTransList = new List<int[]>(); //transition matrix used for level generation

    public Transform player;

    private States currentState;

    private List<GameObject> platsformObjects = new List<GameObject>(); //generated level objects
    private List<GameObject> additPlatsformObjects = new List<GameObject>(); //additional, non essitential generated level objects



    private int startX = 0;
    private int xPos = 0;
    private int levelLength;
    private int levelHeight = 10;
    private int stepHistory;

    private float restFreqLevel = 0;
    private float restAmountLevel = 0;

    private Vector2 enforceBranchDirection = Vector2.zero;
    private Vector2 startPlayerPos;

    private List<int> history = new List<int>(); //list of states used to create each level instance. 
    private List<string> transitionIndexString = new List<string>(); //transition state reference.
    private int[] currentIndex;
    private int[] manualTransitionPath;


    void Start()
    {
        startX = (int)transform.position.x;
    }

    /// <summary>
    /// Sets up generation parameters
    /// </summary>
    /// <param name="height"></param>
    /// <param name="length"></param>
    /// <param name="transitions"></param>
    /// <param name="mapping"></param>
    /// <param name="historyStep"></param>
    /// <param name="stateAmount"></param>
    public void MyStart(int height, int length, int transitions, SetUpManager.MappingType mapping, int historyStep, int stateAmount)
    {
        stepHistory = historyStep + 1;
        currentIndex = new int[stepHistory];

        player.GetComponent<SimpleAIController>().SetMapping(mapping); //sets mapping to level testing agen for correct output

        levelLength = length;
        levelHeight = height;
        startPlayerPos = player.transform.position;

        //Generates empty transition matrix
        List<int> x = new List<int>();
        for (int j = 0; j < transitions; j++)
        {
            x.Add(100 / transitions);
        }

        //Generates transition index, used to refrence transition state.
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
            probabilityTransList.Add(x.ToArray());
        }

        //begins generation 1, using randomly generated transition matrix.
        NewLevelCandidate(); 
    }

    /// <summary>
    /// Generates level using current transition matrix.
    /// </summary>
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
            Destroy(plat.gameObject); //Destroy existing level
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
                //generates level from a manually set state list - used in specific level generation
                currentState = (States) manualTransitionPath[i];
            }

            //Determines type of platform too spawn based, by comparing heights to states.
            if ((((int) currentState < (levelHeight) * 2 && (int) currentState > levelHeight)))
            {
                currentState = States.NoGround;
            }

            if ((int) currentState >= (levelHeight) * 3)
            {
                currentState = States.NoGround;
            }

            if (currentState != States.NoGround)
            {
                GameObject toSpawn = ground;
                int spawnY = yPos + (int) currentState - 1;

                if ((int) currentState > levelHeight)
                {
                    spawnY = yPos + (int) currentState - (levelHeight);
                    spawnY = spawnY - 1;
                }

                if ((int) currentState > (levelHeight) * 2)
                {
                    toSpawn = spikePlat;
                    spawnY = yPos + (int) currentState - (levelHeight * 2);
                    spawnY = spawnY - 1;
                }

                GameObject plat = Instantiate(toSpawn, new Vector3(xPos, spawnY, 0),
                    toSpawn.transform.rotation, transform);
                platsformObjects.Add(plat);
            }

            //used for tree branch generation upwards
            if (enforceBranchDirection.y == 1)
            {
                yPos++;
            }

            //used for tree branch generation downwards
            if (enforceBranchDirection.y == -1)
            {
                yPos--;
            }

            xPos++;
        }

        //end space height
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

        //final platforms for level
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

        GetComponent<EventTracker>().SetSuccess(end.transform); //sets end of level target for AI

        foreach (var plt in additPlatsformObjects)
        {
            Destroy(plt);
        }
        additPlatsformObjects.Clear();
        foreach (var plt in platsformObjects)
        {
            for (int i = (int)plt.transform.position.y - 1; i >= transform.position.y; i--)
            {
                GameObject plat = Instantiate(groundB, new Vector3(plt.transform.position.x, i), ground.transform.rotation, transform);
                additPlatsformObjects.Add(plat);
            }
        }
    }

    /// <summary>
    /// State change, based on transition probablilties.
    /// </summary>
    void SwitchStates()
    {
        int r = Random.Range(0, 100);
        int iter = 0;
        int selectedTransition = 0;

        int trueState = 0;

        string currentIdxStr = "";
        foreach (var id in currentIndex)
        {
            currentIdxStr += id; //index as string
        }

        for (int i = 0; i < transitionIndexString.Count; i++)
        {
            if (transitionIndexString[i] == currentIdxStr)
            {
                trueState = i;
            }
        }

        //adds value from each transtion indx until the randomly selected value is surpassed to find transition index.
        for (int i = 0; i < probabilityTransList[trueState].Length; i++)
        {
            if (iter < r)
            {
                iter += probabilityTransList[trueState][i];
                selectedTransition = i;
            }
        }

        //WIERD ISSUE
        int stateString = (transitionIndexString[selectedTransition][currentIndex.Length - 1] - 48);

        List<int> newIndex = new List<int>();
        for (int i = 0; i < currentIndex.Length - 1; i++)
        {
            newIndex.Add(currentIndex[i + 1]);
        }
        newIndex.Add(stateString);

        currentIndex = newIndex.ToArray();
        currentState = (States)currentIndex[currentIndex.Length - 1];
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