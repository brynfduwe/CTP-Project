using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GeneticAlgManager : MonoBehaviour
{
    public int candidateGoal = 10;
    public int offSpringPopulation = 25;
    private float failedYpos;
    public UIManager UImanager;
    private int generation = 1;
    private int candidate = 1;
    private float FitnessTimer;
    private List<List<int[]>> CandidateList = new List<List<int[]>>();
    public List<float> CandidateFitness = new List<float>();
    private List<List<int[]>> CurrentOffspring = new List<List<int[]>>();
    int offspringIter;

    public GameObject[] levelGMs;

    int TimeScale = 1;


    // Use this for initialization
    void Start()
    {
        failedYpos = transform.position.y - 1;

        generation = 1;
        candidate = 0;

        UImanager.UpdateCandidate(candidate);
        UImanager.UpdateGeneration(generation);

        foreach (var LGM in levelGMs)
        {
            LGM.GetComponent<LevelGenerator>().RandomChain();
            LGM.GetComponent<LevelGenerator>().NewLevelCandidate();
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (TimeScale < 20)
            {
                TimeScale++;
                Time.timeScale = TimeScale;
                UImanager.UpdateTimeScale(TimeScale);
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (TimeScale > 1)
            {
                TimeScale--;
                Time.timeScale = TimeScale;
                UImanager.UpdateTimeScale(TimeScale);
            }
        }


        FitnessTimer += Time.deltaTime;
        for (int i = 0; i < levelGMs.Length; i++)
        {
            //if fail
            if (levelGMs[i].GetComponent<EventTracker>().FailCheck())
            {
                if (generation == 1)
                {
                    levelGMs[i].GetComponent<LevelGenerator>().RandomChain();
                    levelGMs[i].GetComponent<LevelGenerator>().NewLevelCandidate();
                }
                else
                {
                    offspringIter++;
                    if (offspringIter >= CurrentOffspring.Count)
                        offspringIter = 0;

                    levelGMs[i].GetComponent<LevelGenerator>().SetNewChain(CurrentOffspring[offspringIter]);
                    levelGMs[i].GetComponent<LevelGenerator>().NewLevelCandidate();
                }
            }
        }

        bool newGen = false;

        for (int i = 0; i < levelGMs.Length; i++)
        {
            //if success
            if (levelGMs[i].GetComponent<EventTracker>().SuccessCheck())
            {
                if (levelGMs[i].GetComponent<EventTracker>().GetFitness() < 160)
                {
                    AddCandidate(i);
                    FitnessTimer = 0;

                    if (CandidateList.Count >= candidateGoal)
                    {
                        newGen = true;
                    }
                }
                else
                {
                    if (generation == 1)
                    {
                        levelGMs[i].GetComponent<LevelGenerator>().RandomChain();
                        levelGMs[i].GetComponent<LevelGenerator>().NewLevelCandidate();
                    }
                    else
                    {
                        offspringIter++;
                        if (offspringIter >= CurrentOffspring.Count)
                            offspringIter = 0;

                        levelGMs[i].GetComponent<LevelGenerator>().SetNewChain(CurrentOffspring[offspringIter]);
                        levelGMs[i].GetComponent<LevelGenerator>().NewLevelCandidate();
                    }
                }
            }
        }

        if (newGen)
        {
            generation++;
            UImanager.UpdateGeneration(generation);
            candidate = 0;
            UImanager.UpdateCandidate(candidate);

            offspringIter = 0;

            CurrentOffspring.Clear();

            for (int i = 0; i < offSpringPopulation; i++)
            {
                Selection();
            }

            CandidateList.Clear();
            CandidateFitness.Clear();

            foreach (var lm in levelGMs)
            {
                lm.GetComponent<LevelGenerator>().SetNewChain(CurrentOffspring[offspringIter]);
                lm.GetComponent<LevelGenerator>().NewLevelCandidate();

                offspringIter++;
                if (offspringIter >= CurrentOffspring.Count)
                    offspringIter = 0;
            }

            newGen = false;

        }
    }


    void AddCandidate(int lm)
    {
        candidate++;
        UImanager.UpdateCandidate(candidate);

        CandidateList.Add(levelGMs[lm].GetComponent<LevelGenerator>().GetGeneratorChromosome());
        CandidateFitness.Add(levelGMs[lm].GetComponent<EventTracker>().GetFitness());

        if (generation == 1)
        {
            levelGMs[lm].GetComponent<LevelGenerator>().RandomChain();
            levelGMs[lm].GetComponent<LevelGenerator>().NewLevelCandidate();
        }
        else
        {
            offspringIter++;
            if (offspringIter >= CurrentOffspring.Count)
                offspringIter = 0;

            levelGMs[lm].GetComponent<LevelGenerator>().SetNewChain(CurrentOffspring[offspringIter]);
            levelGMs[lm].GetComponent<LevelGenerator>().NewLevelCandidate();
        }
    }


    void Selection()
    {
        
        //uses routlette wheel, TODO - order chain by ascending fitness, use this to determine diffucutly?
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

        Offspring = Crossover(CandidateList[p1], CandidateList[p2]);

        CurrentOffspring.Add(Offspring);
    }


    List<int[]> Crossover(List<int[]> parent1, List<int[]> parent2)
    {
        List<int[]> Offspring = new List<int[]>();

        bool flipped = false;
        int f = (Random.Range(0, 10));

        if (f >= 5)
        {
            flipped = true;
        }

        int r = Random.Range(0, parent1.Count);

        Offspring.Clear();

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

  //      Debug.Log("Flipped = " + flipped.ToString());
  //      Debug.Log("CrossoverPoint = " + r.ToString());
        return Offspring;
    }
}
