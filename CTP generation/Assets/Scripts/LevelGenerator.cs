using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject ground;
    public GameObject endFlag;

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

    public int[] G0Probs = new int[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 };
    public int[] G1Probs = new int[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 };
    public int[] G2Probs = new int[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 };
    public int[] G3Probs = new int[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 };
    public int[] G4Probs = new int[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 };
    public int[] G5Probs = new int[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 };
    public int[] G6Probs = new int[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 };
    public int[] G7Probs = new int[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 };
    public int[] G8Probs = new int[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 };
    public int[] G9Probs = new int[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 };

    private States currentState;
    private int xPos = 0;
    public int levelLength;

    public Transform player;
    private Vector2 startPlayerPos;

    // Use this for initialization
    void Start()
    {
        startPlayerPos = player.transform.position;

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
    }


    void SwitchStates()
    {
        int r = Random.Range(0, 100);
        int iter = 0;
        int selectedTransition = 0;

        switch (currentState)
        {
            case States.NoGround:

                for (int i = 0; i < G0Probs.Length; i++)
                {
                    if (iter < r)
                    {
                        iter += G0Probs[i];
                        selectedTransition = i;
                    }
                }

                currentState = (States) selectedTransition;

                break;

            case States.Ground1:

                for (int i = 0; i < G1Probs.Length; i++)
                {
                    if (iter < r)
                    {
                        iter += G1Probs[i];
                        selectedTransition = i;
                    }
                }

                currentState = (States) selectedTransition;

                break;

            case States.Ground2:

                for (int i = 0; i < G2Probs.Length; i++)
                {
                    if (iter < r)
                    {
                        iter += G2Probs[i];
                        selectedTransition = i;
                    }
                }

                currentState = (States) selectedTransition;

                break;

            case States.Ground3:

                for (int i = 0; i < G3Probs.Length; i++)
                {
                    if (iter < r)
                    {
                        iter += G3Probs[i];
                        selectedTransition = i;
                    }
                }

                currentState = (States) selectedTransition;

                break;

            case States.Ground4:

                for (int i = 0; i < G4Probs.Length; i++)
                {
                    if (iter < r)
                    {
                        iter += G4Probs[i];
                        selectedTransition = i;
                    }
                }

                currentState = (States) selectedTransition;

                break;

            case States.Ground5:

                for (int i = 0; i < G5Probs.Length; i++)
                {
                    if (iter < r)
                    {
                        iter += G5Probs[i];
                        selectedTransition = i;
                    }
                }

                currentState = (States)selectedTransition;

                break;

            case States.Ground6:

                for (int i = 0; i < G6Probs.Length; i++)
                {
                    if (iter < r)
                    {
                        iter += G6Probs[i];
                        selectedTransition = i;
                    }
                }

                currentState = (States)selectedTransition;

                break;

            case States.Ground7:

                for (int i = 0; i < G7Probs.Length; i++)
                {
                    if (iter < r)
                    {
                        iter += G7Probs[i];
                        selectedTransition = i;
                    }
                }

                currentState = (States)selectedTransition;

                break;


            case States.Ground8:

                for (int i = 0; i < G8Probs.Length; i++)
                {
                    if (iter < r)
                    {
                        iter += G8Probs[i];
                        selectedTransition = i;
                    }
                }

                currentState = (States)selectedTransition;

                break;


            case States.Ground9:

                for (int i = 0; i < G9Probs.Length; i++)
                {
                    if (iter < r)
                    {
                        iter += G9Probs[i];
                        selectedTransition = i;
                    }
                }

                currentState = (States)selectedTransition;

                break;
        }
    }


    public void RandomChain()
    {
        int leftVal = 100;
        for (int i = 0; i < G0Probs.Length - 1; i++)
        {
            int r = Random.Range(0, leftVal);
            leftVal -= r;
            G0Probs[i] = r;
        }
        G0Probs[G0Probs.Length - 1] = leftVal;

        leftVal = 100;
        for (int i = 0; i < G1Probs.Length - 1; i++)
        {
            int r = Random.Range(0, leftVal);
            leftVal -= r;
            G1Probs[i] = r;
        }
        G1Probs[G1Probs.Length - 1] = leftVal;

        leftVal = 100;
        for (int i = 0; i < G2Probs.Length - 1; i++)
        {
            int r = Random.Range(0, leftVal);
            leftVal -= r;
            G2Probs[i] = r;
        }

        G2Probs[G2Probs.Length - 1] = leftVal;

        leftVal = 100;
        for (int i = 0; i < G3Probs.Length - 1; i++)
        {
            int r = Random.Range(0, leftVal);
            leftVal -= r;
            G3Probs[i] = r;
        }
        G3Probs[G3Probs.Length - 1] = leftVal;

        leftVal = 100;
        for (int i = 0; i < G4Probs.Length - 1; i++)
        {
            int r = Random.Range(0, leftVal);
            leftVal -= r;
            G4Probs[i] = r;
        }
        G4Probs[G4Probs.Length - 1] = leftVal;

        leftVal = 100;
        for (int i = 0; i < G5Probs.Length - 1; i++)
        {
            int r = Random.Range(0, leftVal);
            leftVal -= r;
            G5Probs[i] = r;
        }
        G5Probs[G5Probs.Length - 1] = leftVal;

        leftVal = 100;
        for (int i = 0; i < G6Probs.Length - 1; i++)
        {
            int r = Random.Range(0, leftVal);
            leftVal -= r;
            G6Probs[i] = r;
        }
        G6Probs[G6Probs.Length - 1] = leftVal;

        leftVal = 100;
        for (int i = 0; i < G7Probs.Length - 1; i++)
        {
            int r = Random.Range(0, leftVal);
            leftVal -= r;
            G7Probs[i] = r;
        }
        G7Probs[G7Probs.Length - 1] = leftVal;

        leftVal = 100;
        for (int i = 0; i < G8Probs.Length - 1; i++)
        {
            int r = Random.Range(0, leftVal);
            leftVal -= r;
            G8Probs[i] = r;
        }
        G8Probs[G8Probs.Length - 1] = leftVal;

        leftVal = 100;
        for (int i = 0; i < G9Probs.Length - 1; i++)
        {
            int r = Random.Range(0, leftVal);
            leftVal -= r;
            G9Probs[i] = r;
        }
        G9Probs[G9Probs.Length - 1] = leftVal;
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
        List<int[]> chromosone = new List<int[]>();

        chromosone.Add(G0Probs);
        chromosone.Add(G1Probs);
        chromosone.Add(G2Probs);
        chromosone.Add(G3Probs);
        chromosone.Add(G4Probs);
        chromosone.Add(G5Probs);
        chromosone.Add(G6Probs);
        chromosone.Add(G7Probs);
        chromosone.Add(G8Probs);
        chromosone.Add(G9Probs);

        return chromosone;
    }

    public void NewLevelCandidate()
    {
        GenerateLevel();
    }

    public void SetNewChain(List<int[]> chain)
    {
        G0Probs = chain[0];
        G1Probs = chain[1];
        G2Probs = chain[2];
        G3Probs = chain[3];
        G4Probs = chain[4];
        G5Probs = chain[5];
        G6Probs = chain[6];
        G7Probs = chain[7];
        G8Probs = chain[8];
        G9Probs = chain[9];
    }
}
