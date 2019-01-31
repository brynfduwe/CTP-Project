using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneticAlgManagerSingleTM : MonoBehaviour
{

    public GameObject LevelGenerator;

    public int candidateGoal = 10;
    public int offSpringPopulation = 25;
    private float failedYpos;
    public UIManager UImanager;
    public int generation = 1;
    private int candidate = 1;
    private float FitnessTimer;
    private List<List<int[]>> CandidateList = new List<List<int[]>>();
    public List<float> CandidateFitness = new List<float>();
    private List<List<int[]>> CurrentOffspring = new List<List<int[]>>();
    int offspringIter;

    public List<GameObject> levelGMs = new List<GameObject>();

    public List<GameObject> failedJumpPlats = new List<GameObject>();

    int TimeScale = 1;

    List<int[]> finalCandidate = new List<int[]>();

    private int finalGen = 0;

    public Text transitionMatrixVis;

    public SetUpManager setUp;

    public List<int[]> currentProbabilityTransMatrix = new List<int[]>();

    public int candidateScore = 0;
    public int testersDone = 0;
    public int testersDoneOverall = 0;


    // Use this for initialization
    void Start()
    {
        candidateGoal = setUp.candidateReq;
        offSpringPopulation = setUp.populationOffspring;

        failedYpos = transform.position.y - 1;

        generation = 1;
        candidate = 0;

        UImanager.UpdateCandidate(candidate);
        UImanager.UpdateGeneration(generation);


        List<int> x = new List<int>();

        for (int j = 0; j < setUp.height; j++)
        {
            x.Add(100 / setUp.height);
        }

        currentProbabilityTransMatrix = new List<int[]>();
        for (int i = 0; i < setUp.height; i++)
        {
            currentProbabilityTransMatrix.Add(x.ToArray());
        }

        candidateScore = 0;
        levelGMs.Clear();
        int y = 0;

        for (int i = 0; i < setUp.testersOnScreen; i++)
        {
            y += setUp.height;
            Vector2 pos = transform.position - new Vector3(0, y);
            GameObject gobj = Instantiate(LevelGenerator, pos, transform.rotation);
            levelGMs.Add(gobj);
            y += 10;
        }

        RandomChain();

        foreach (var LGM in levelGMs)
        {
            LGM.GetComponent<LevelGenerator>().MyStart(setUp.height, setUp.length);
            LGM.GetComponent<LevelGenerator>().SetRests(setUp.GetRestCov());

            LGM.GetComponent<LevelGenerator>().SetNewChain(currentProbabilityTransMatrix);
            LGM.GetComponent<LevelGenerator>().NewLevelCandidate();
        }

        transitionMatrixVis.text = "";
        foreach (var ptl in currentProbabilityTransMatrix)
        {
            foreach (var i in ptl)
            {
                transitionMatrixVis.text += (i.ToString() + ", ");
            }
            transitionMatrixVis.text += "\n";
        }
    }

    public void RandomChain()
    {
        List<int> x = new List<int>();

        for (int j = 0; j < setUp.height; j++)
        {
            x.Add(100 / setUp.height);
        }

        currentProbabilityTransMatrix = new List<int[]>();

        for (int i = 0; i < setUp.height; i++)
        {
            currentProbabilityTransMatrix.Add(x.ToArray());
        }

        foreach (var pa in currentProbabilityTransMatrix)
        {
            int leftVal = 100;
            int decremter = currentProbabilityTransMatrix.Count - 1;
            List<bool> usedCheck = new List<bool>();

            foreach (var a in pa)
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
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            IncreaseTimeScale();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            DecreaseTimeScale();
        }


        for (int i = 0; i < levelGMs.Count; i++)
        {
            if (!levelGMs[i].GetComponent<LevelGenerator>().CheckPlayerLocked())
            {

                //if success
                if (levelGMs[i].GetComponent<EventTracker>().SuccessCheck())
                {
                    levelGMs[i].GetComponent<LevelGenerator>().LockPlayer();
                    candidateScore++;
                    testersDone++;
                }

                //if fail
                if (levelGMs[i].GetComponent<EventTracker>().FailCheck())
                {
                    levelGMs[i].GetComponent<LevelGenerator>().LockPlayer();
                    testersDone++;
                }
            }
        }

        if (testersDone >= levelGMs.Count - 1)
        {
            testersDoneOverall += testersDone;
            foreach (var LGM in levelGMs)
            {
                LGM.GetComponent<LevelGenerator>().NewLevelCandidate();
            }

            testersDone = 0;
        }

        if (testersDoneOverall >= setUp.testers)
        {
            float fitness = ((float) candidateScore / testersDoneOverall);
            if (fitness > 0.25)
            {
                CandidateList.Add(currentProbabilityTransMatrix);
                CandidateFitness.Add(fitness);

                candidate++;
                UImanager.UpdateCandidate(candidate);
            }

            testersDoneOverall = 0;
           // testersDone = 0;
            candidateScore = 0;

            if (generation == 1)
            {
                RandomChain();

                foreach (var LGM in levelGMs)
                {
                    LGM.GetComponent<LevelGenerator>().SetNewChain(currentProbabilityTransMatrix);
                    LGM.GetComponent<LevelGenerator>().NewLevelCandidate();
                }

                transitionMatrixVis.text = "";
                foreach (var ptl in currentProbabilityTransMatrix)
                {
                    foreach (var i in ptl)
                    {
                        transitionMatrixVis.text += (i.ToString() + ", ");
                    }
                    transitionMatrixVis.text += "\n";
                }
            }
            else
            {
                offspringIter++;
                //  if (offspringIter >= CurrentOffspring.Count)
                //     offspringIter = 0;

                currentProbabilityTransMatrix = CurrentOffspring[Random.Range(0, CurrentOffspring.Count)];

                foreach (var LGM in levelGMs)
                {
                    LGM.GetComponent<LevelGenerator>().SetNewChain(currentProbabilityTransMatrix);
                    LGM.GetComponent<LevelGenerator>().NewLevelCandidate();
                }

                transitionMatrixVis.text = "";
                foreach (var ptl in currentProbabilityTransMatrix)
                {
                    foreach (var i in ptl)
                    {
                        transitionMatrixVis.text += (i.ToString() + ", ");
                    }
                    transitionMatrixVis.text += "\n";
                }
            }

            //new Generation
            if (CandidateList.Count >= setUp.candidateReq)
            {
                generation++;
                UImanager.UpdateGeneration(generation);
                candidate = 0;
                UImanager.UpdateCandidate(candidate);


                CurrentOffspring.Clear();

                for (int i = 0; i < offSpringPopulation; i++)
                {
                    Selection();
                }

                CandidateList.Clear();
                CandidateFitness.Clear();

                //   offspringIter++;
                //  if (offspringIter >= CurrentOffspring.Count)
                //      offspringIter = 0;

                currentProbabilityTransMatrix = CurrentOffspring[Random.Range(0, CurrentOffspring.Count)];

                foreach (var LGM in levelGMs)
                {
                    LGM.GetComponent<LevelGenerator>().SetNewChain(currentProbabilityTransMatrix);
                    LGM.GetComponent<LevelGenerator>().NewLevelCandidate();
                }

                transitionMatrixVis.text = "";
                foreach (var ptl in currentProbabilityTransMatrix)
                {
                    foreach (var i in ptl)
                    {
                        transitionMatrixVis.text += (i.ToString() + ", ");
                    }
                    transitionMatrixVis.text += "\n";
                }
            }
        }
    }


    void Selection()
    {

        List<int[]> Offspring = new List<int[]>();

        int p1 = 0;
        int p2 = 0;

        float totalFitness = 0;
        foreach (var f in CandidateFitness)
        {
            totalFitness += f;
        }

        float r = Random.Range(0, totalFitness);
        float iter = 0;
        int selected = 0;

        for (int i = 0; i < CandidateFitness.Count; i++)
        {
            if (iter < r)
            {
                iter += CandidateFitness[i];
                p1 = i;
            }
        }

        r = Random.Range(0, totalFitness);
        iter = 0;
        selected = 0;

        for (int i = 0; i < CandidateFitness.Count; i++)
        {
            if (iter < r)
            {
                iter += CandidateFitness[i];
                p2 = i;
            }
        }

        //Random.seed = System.DateTime.Now.Millisecond;
        //p1 = Random.Range(0, CandidateList.Count);
        //Random.seed = System.DateTime.Now.Millisecond;
        //p2 = Random.Range(0, CandidateList.Count);

        while (p1 == p2)
        {
            p2 = Random.Range(0, CandidateList.Count);
        }

        Debug.Log(p1.ToString() + " - " + p2.ToString());

        Offspring = Crossover(CandidateList[p1], CandidateList[p2]);

        CurrentOffspring.Add(Offspring);

    }


    List<int[]> Crossover(List<int[]> parent1, List<int[]> parent2)
    {
        List<int[]> Offspring = new List<int[]>();

        bool flipped = false;
        int f = (Random.Range(0, 10));

        if (f >= 5)
            flipped = true;   

        int r = Random.Range(0, parent1.Count);

        for (int i = 0; i < parent1.Count; i++)
        {
            if (!flipped)
            {
                if (r < i)
                {
                    Offspring.Add(parent1[i]);
                }
                else
                {
                    Offspring.Add(parent2[i]);
                }
            }
            else
            {
                if (r < i)
                {
                    Offspring.Add(parent2[i]);
                }
                else
                {
                    Offspring.Add(parent1[i]);
                }
            }
        }

   //     Debug.Log("Flipped = " + flipped.ToString());
   //    Debug.Log("CrossoverPoint = " + r.ToString());
        return Offspring;
    }


    public void IncreaseTimeScale()
    {
        if (TimeScale < 10)
        {
            TimeScale++;
            Time.timeScale = TimeScale;
            UImanager.UpdateTimeScale(TimeScale);
        }
    }


    public void DecreaseTimeScale()
    {
        if (TimeScale > 1)
        {
            TimeScale--;
            Time.timeScale = TimeScale;
            UImanager.UpdateTimeScale(TimeScale);
        }
    }
}
