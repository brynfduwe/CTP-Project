using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneticAlgManagerSingleTM : MonoBehaviour
{

    public GameObject LevelGenerator;

    int candidateGoal = 10;
    int offSpringPopulation = 25;
    public UIManager UImanager;
    public SetUpManager setUp;
    public int generation = 1;
    private int candidate = 1;
    private float FitnessTimer;
    private List<List<int[]>> CandidateList = new List<List<int[]>>();
    public List<float> CandidateFitness = new List<float>();
    private List<List<int[]>> CurrentOffspring = new List<List<int[]>>();
    int offspringIter;

    public List<GameObject> levelGMs = new List<GameObject>();

    int TimeScale = 1;

    List<int[]> finalCandidate = new List<int[]>();

    private int finalGen = 0;

    public List<int[]> currentProbabilityTransMatrix = new List<int[]>();

   // public int candidateScore = 0;
    public int testersDone = 0;

    private int transitions = 0;

    List<List<int>> candidateAllActions = new List<List<int>>(); //to learn cost

    List<List<int>> generationBestActions = new List<List<int>>(); // convergence graph

    public List<Vector2> bestTransitionPath;
    public float bestFitnessOverall = 0;




    // Use this for initialization
    void Start()
    {
        GetComponent<CostFunction>().SetMapping(setUp.mapping);


        candidateGoal = setUp.candidateReq;
        offSpringPopulation = setUp.populationOffspring;

        generation = 1;
        candidate = 0;

        UImanager.UpdateCandidate(candidate);
        UImanager.UpdateGeneration(generation);


        List<int> x = new List<int>();

        transitions = setUp.height; // ground 
        transitions += setUp.height; // gaps

        if (setUp.spikes == true)
        {
            transitions = transitions + setUp.height; // spikes
        }

        if (setUp.hearts == true)
        {
            transitions = transitions + setUp.height; // hearts
        }

        int stateAmounts = transitions;
        transitions = (int)Mathf.Pow(transitions, setUp.historySteps + 1);

        for (int j = 0; j < transitions; j++)
        {
            x.Add(100 / transitions);
        }

        currentProbabilityTransMatrix = new List<int[]>();
        for (int i = 0; i < transitions; i++)
        {
            currentProbabilityTransMatrix.Add(x.ToArray());
        }

        //candidateScore = 0;
        levelGMs.Clear();
        int y = 0;

        for (int i = 0; i < setUp.testersOnScreen; i++)
        {      
            Vector2 pos = transform.position - new Vector3(0, y);
            GameObject gobj = Instantiate(LevelGenerator, pos, transform.rotation);
            levelGMs.Add(gobj);
            y += setUp.height + 10;
        }

        RandomChain();

        foreach (var LGM in levelGMs)
        {
            LGM.GetComponent<LevelGenerator>().MyStart(setUp.height, setUp.length, transitions, setUp.mapping, setUp.historySteps, stateAmounts);
          //  LGM.GetComponent<LevelGenerator>().SetRests(setUp.GetRestCov());

            LGM.GetComponent<LevelGenerator>().SetNewChain(currentProbabilityTransMatrix);
            LGM.GetComponent<LevelGenerator>().NewLevelCandidate();
        }


        GetComponent<CSVWriter>().WriteTestInfo(setUp.height, setUp.length, setUp.minimumFitnessReq);
        GetComponent<CSVWriter>().WriteCandidate(currentProbabilityTransMatrix, candidate, generation);

        GetComponent<ScreenCaptureHandler>().ClearFolder();
    }

    public void RandomChain()
    {
        List<int> x = new List<int>();

        for (int j = 0; j < transitions; j++)
        {
            x.Add(100 / transitions);
        }

        currentProbabilityTransMatrix = new List<int[]>();

        for (int i = 0; i < transitions; i++)
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
            if (testersDone < setUp.testers)
            {
                //if success
                if (levelGMs[i].GetComponent<EventTracker>().SuccessCheck() ||
                    levelGMs[i].GetComponent<EventTracker>().FailCheck())
                {

                    float cost = GetComponent<CostFunction>().CalculateCost(GetComponent<CSVReader>().getOrderedCurveValues(), levelGMs[i].GetComponent<LevelGenerator>().player.gameObject
                                     .GetComponent<SimpleAIController>().GetAllActions());
                    cost = 1 - cost;

                    if (cost > bestFitnessOverall + 0.1f)
                    {
                        // Debug.Log(cost);
                        bestFitnessOverall = cost;
                        bestTransitionPath = levelGMs[i].GetComponent<LevelGenerator>().getHistory();
                    }




                    levelGMs[i].GetComponent<LevelGenerator>().LockPlayer();
                    //candidateScore++;
                    testersDone++;

                    if (setUp.saveImgs)
                        GetComponent<ScreenCaptureHandler>().ScreenGrab(levelGMs[i].transform.position, candidate,
                            generation, testersDone);

                    GetComponent<CSVWriter>().Write(generation, candidate,
                        levelGMs[i].GetComponent<LevelGenerator>().player.gameObject
                            .GetComponent<SimpleAIController>().GetAllActions());
                    levelGMs[i].GetComponent<LevelGenerator>().NewLevelCandidate();


                    candidateAllActions.Add(levelGMs[i].GetComponent<LevelGenerator>().player.gameObject
                        .GetComponent<SimpleAIController>().GetAllActions());
                }
            }

        }

        if (testersDone >= setUp.testers)
        {
            //fitness
            float totalCost = 0;
            foreach (var testerActions in candidateAllActions)
            {
                float cost = GetComponent<CostFunction>().CalculateCost(GetComponent<CSVReader>().getOrderedCurveValues(), testerActions);
               // Debug.Log(1 - cost);
                totalCost += cost;
            }

            float totalAvg = totalCost / testersDone;
            float fitness = 1 - totalAvg;

            if (fitness >= setUp.minimumFitnessReq)
            {
                generationBestActions.Add(candidateAllActions[0]);
                Debug.Log(candidateAllActions[0].Count);

                CandidateList.Add(currentProbabilityTransMatrix);
                CandidateFitness.Add(fitness * 5);
                GetComponent<CSVWriter>().WriteFitness(fitness);

                GetComponent<CSVWriter>().CandidateToCSVAndClear(true);

                candidate++;
                UImanager.UpdateCandidate(candidate);
            }
            else
            {
                GetComponent<CSVWriter>().CandidateToCSVAndClear(false);
            }

            candidateAllActions.Clear();

            testersDone = 0;
          //  candidateScore = 0;

            if (generation == 1)
            {
                RandomChain();

                foreach (var LGM in levelGMs)
                {
                    LGM.GetComponent<LevelGenerator>().SetNewChain(currentProbabilityTransMatrix);
                    LGM.GetComponent<LevelGenerator>().NewLevelCandidate();
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
            }

            //new Generation
            if (CandidateList.Count >= setUp.candidateReq)
            {
                GetComponent<CSVWriter>().WriteConvergence(generationBestActions[Random.Range(0, generationBestActions.Count)]);
                generationBestActions.Clear();

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
            }

            GetComponent<CSVWriter>().WriteCandidate(currentProbabilityTransMatrix, candidate, generation);
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
            totalFitness += f; //multiplcation to weed out worse candidates futher
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
