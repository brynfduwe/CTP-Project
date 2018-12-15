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
        Ground4
    }

    public int[] G0Probs = new int[] {20, 20, 20, 20, 20};
    public int[] G1Probs = new int[] {20, 20, 20, 20, 20};
    public int[] G2Probs = new int[] {20, 20, 20, 20, 20};
    public int[] G3Probs = new int[] {20, 20, 20, 20, 20};
    public int[] G4Probs = new int[] {20, 20, 20, 20, 20};

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
                GameObject plat = Instantiate(ground, new Vector3(xPos, (int) currentState, 0),
                    ground.transform.rotation);
                platsformObjects.Add(plat);
            }

            xPos++;
        }

        //end space
        if (currentState == States.NoGround)
        {
            currentState = States.Ground1;
        }
        GameObject end = Instantiate(endFlag, new Vector3(xPos, (int)currentState, 0), ground.transform.rotation);
        platsformObjects.Add(end);
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
    }
}
