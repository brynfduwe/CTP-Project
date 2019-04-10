using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    private int finalGen = 0;

    public List<int[]> currentProbabilityTransMatrix = new List<int[]>();

   // public int candidateScore = 0;
    public int testersDone = 0;

    private int transitions = 0;
    private int stateAmounts = 0;

    List<List<float>> candidateAllActions = new List<List<float>>(); //to learn cost

    List<List<float>> generationBestActions = new List<List<float>>(); // convergence graph

    private List<int[]> candidateAllPaths = new List<int[]>();

    //
    public int[] bestTransitionPath;
    public float bestFitnessOverall = -1000;
    public List<int[]> bestTransitionMatrix = new List<int[]>();
    public float[] bestCandidateActions;
    //


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
            transitions = transitions + setUp.height; // spike gaps
        }

        stateAmounts = transitions;
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

            LGM.GetComponent<LevelGenerator>().SetNewChain(currentProbabilityTransMatrix);
            LGM.GetComponent<LevelGenerator>().NewLevelCandidate();
        }

        GetComponent<CSVWriter>().WriteConvergenceDesignCurve(GetComponent<CSVReader>().getOrderedCurveValues());
        GetComponent<CSVWriter>().WriteConvergenceNewGen(generation);

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
                pa[rT] = r;
                usedCheck[rT] = true;
                decremter--;
            }
        }
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            IncreaseTimeScale();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
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
                    levelGMs[i].GetComponent<LevelGenerator>().LockPlayer();
                    //candidateScore++;
                    testersDone++;
                    UImanager.UpdateTests(testersDone);

                    if (setUp.saveImgs)
                        GetComponent<ScreenCaptureHandler>().ScreenGrab(levelGMs[i].transform.position, candidate,
                            generation, testersDone);

                    GetComponent<CSVWriter>().Write(generation, candidate,
                        levelGMs[i].GetComponent<LevelGenerator>().player.gameObject
                            .GetComponent<SimpleAIController>().GetAllActions());
                    levelGMs[i].GetComponent<LevelGenerator>().NewLevelCandidate();

                    candidateAllActions.Add(levelGMs[i].GetComponent<LevelGenerator>().player.gameObject
                        .GetComponent<SimpleAIController>().GetAllActions());

                    candidateAllPaths.Add(levelGMs[i].GetComponent<LevelGenerator>().getHistory());
                }
            }

        }

        if (testersDone >= setUp.testers)
        {
            float currentBestFit = 0;
            //fitness
          //  Debug.Log("cOUNT " + candidateAllActions.Count);
            float totalCost = 0;
            for (int i = 0; i < candidateAllActions.Count; i++)
            {
              //  Debug.Log("CANNI cOUNT " + candidateAllActions[i].Count);
                float cost = GetComponent<CostFunction>()
                    .CalculateCost(GetComponent<CSVReader>().getOrderedCurveValues(), candidateAllActions[i]);
                // Debug.Log(1 - cost);
                totalCost += cost;

                float fits = 1 - cost;

                if (fits > (float) bestFitnessOverall)
                {
                    //   Debug.Log("NEWBEST " + fits);

                    UImanager.UpdateFitness((int)(System.Math.Round(fits, 2) * 100));

                    bestFitnessOverall = fits;

                    //  bestTransitionPath = levelGMs[i].GetComponent<LevelGenerator>().getHistory();
                    bestTransitionMatrix = currentProbabilityTransMatrix;
                    bestCandidateActions = candidateAllActions[i].ToArray();
                    bestTransitionPath = candidateAllPaths[i];
                }

                if (fits > currentBestFit)
                {
                    currentBestFit = fits;
                }

            }

            float totalAvg = totalCost / testersDone;
           // float fitness = 1 - totalAvg;

            if (currentBestFit >= setUp.minimumFitnessReq)
            {
                GetComponent<CSVWriter>().WriteConvergence(candidateAllActions[Random.Range(0, testersDone)]);

                CandidateList.Add(currentProbabilityTransMatrix);
                CandidateFitness.Add((currentBestFit * currentBestFit));  //multiplcation to weed out worse candidates futher in Selection()
                GetComponent<CSVWriter>().WriteFitness(currentBestFit);

                GetComponent<CSVWriter>().CandidateToCSVAndClear(true);

                candidate++;
                UImanager.UpdateCandidate(candidate);
            }
            else
            {
                GetComponent<CSVWriter>().CandidateToCSVAndClear(false);
            }

            candidateAllActions.Clear();
            candidateAllPaths.Clear();

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
                generationBestActions.Clear();
                generation++;
                
                if (generation > setUp.endAfterGen || bestFitnessOverall >= setUp.endFitnessReq)
                {
                    EndGeneration();
                }
                GetComponent<CSVWriter>().WriteConvergenceNewGen(generation); //write out generation data.

                UImanager.UpdateGeneration(generation);
                candidate = 0;
                UImanager.UpdateCandidate(candidate);
                UImanager.UpdateTests(0);

                //clear old offspring
                CurrentOffspring.Clear();
                //generate new offspring
                for (int i = 0; i < offSpringPopulation; i++)
                {
                    Selection();
                }

                //preperation for new generation
                CandidateList.Clear();
                CandidateFitness.Clear();
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

    /// <summary>
    /// Selects 2 parents for offspring matrix based on fitness.
    /// </summary>
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

        for (int i = 0; i < CandidateFitness.Count; i++)
        {
            if (iter < r)
            {
                iter += CandidateFitness[i];
                p2 = i;
            }
        }
        while (p1 == p2)
        {
            p2 = Random.Range(0, CandidateList.Count);
        }

        Offspring = Crossover(CandidateList[p1], CandidateList[p2]);
        CurrentOffspring.Add(Offspring);
    }

    /// <summary>
    /// Crossover for genetic algothim
    /// </summary>
    /// <param name="parent1"></param>
    /// <param name="parent2"></param>
    /// <returns></returns>
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
            //if less than mutation rate, the gene(transition array) is randomised
            if (Random.Range(0, 1) < setUp.mutationRate)
            {
                List<int> x = new List<int>();

                for (int j = 0; j < transitions; j++)
                {
                    x.Add(100 / transitions);
                }

                int leftVal = 100;
                int decremter = currentProbabilityTransMatrix.Count - 1;
                List<bool> usedCheck = new List<bool>();

                foreach (var a in x)
                {
                    usedCheck.Add(false);
                }

                while (decremter >= 0)
                {
                    int rT = Random.Range(0, x.Count);
                    while (usedCheck[rT] == true)
                    {
                        rT = Random.Range(0, x.Count);
                    }

                    int rd = Random.Range(0, leftVal + 1);
                    leftVal -= rd;
                    x[rT] = rd;
                    usedCheck[rT] = true;
                    decremter--;
                }

                Offspring.Add(x.ToArray());
            }
            else //otherwise it is formed from the 2 parents.
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
        }
        return Offspring;
    }

    /// <summary>
    /// Ends generation process, setsup and loads stand alone level.
    /// </summary>
    void EndGeneration()
    {
        GetComponent<CSVWriter>().WriteFinal(bestCandidateActions);
        GameObject.Find("StandAloneLevelManager").GetComponent<StandAloneLevelManager>().SetUp(setUp.height, setUp.length, transitions, stateAmounts, setUp.historySteps, bestTransitionPath, bestTransitionMatrix, setUp.mapping, bestFitnessOverall);
        GameObject.Find("SceneManager").GetComponent<SceneManager>().LoadScene(1);
    }


    public void IncreaseTimeScale()
    {
        if (TimeScale < 5)
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
