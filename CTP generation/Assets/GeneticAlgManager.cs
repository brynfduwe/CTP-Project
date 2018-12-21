using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgManager : MonoBehaviour
{
    private float failedYpos;

    public UIManager UImanager;
    private int generation = 1;
    private int candidate = 1;
    private float FitnessTimer;

    private List<List<int[]>> CandidateList = new List<List<int[]>>();
    private List<float> CandidateFitness = new List<float>();

    private List<List<int[]>> CurrentOffspring = new List<List<int[]>>();

    int offspringIter;

     public GameObject[] levelGMs;


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

        Time.timeScale = Time.timeScale * 2.5f;
    }


    // Update is called once per frame
    void Update()
    {
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
                AddCandidate(i);
                FitnessTimer = 0;

                if (CandidateList.Count > 5)
                {
                    newGen = true;
                }
                else
                {
                    if (generation != 1)
                    {
                        levelGMs[i].GetComponent<LevelGenerator>().SetNewChain(CurrentOffspring[offspringIter]);
                        levelGMs[i].GetComponent<LevelGenerator>().NewLevelCandidate();
                        offspringIter++;
                        if (offspringIter >= CurrentOffspring.Count)
                            offspringIter = 0;
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


            for (int i = 0; i < levelGMs.Length; i++)
            {
                Selection(i);
            }

            CandidateList.Clear();
            newGen = false;

        }
    }


    void AddCandidate(int lm)
    {
        candidate++;
        UImanager.UpdateCandidate(candidate);

        CandidateList.Add(levelGMs[lm].GetComponent<LevelGenerator>().GetGeneratorChromosome());
      //  CandidateFitness.Add(FitnessTimer);

        if (generation == 1)
        {
            levelGMs[lm].GetComponent<LevelGenerator>().RandomChain();
            levelGMs[lm].GetComponent<LevelGenerator>().NewLevelCandidate();
        }
        else
        {
            levelGMs[lm].GetComponent<LevelGenerator>().NewLevelCandidate();
        }
    }


    void Selection(int lm)
    {
        Debug.Log("SELECTION");
        //change to roulette wheel

        CandidateFitness.Clear();
        CurrentOffspring.Clear();

        for (int i = 0; i < 10; i++)
        {
            List<int[]> Offspring = new List<int[]>();
            int p1 = Random.Range(0, CandidateList.Count);

            int p2 = Random.Range(0, CandidateList.Count);

            //check to ensure parents arent the same 
            while (p2 == p1)
            {
                p2 = Random.Range(0, CandidateList.Count);
            }

            Offspring = Crossover(CandidateList[p1], CandidateList[p2]);

            CurrentOffspring.Add(Offspring);
        }

       // CandidateList.Clear();

        levelGMs[lm].GetComponent<LevelGenerator>().SetNewChain(CurrentOffspring[0]);
        levelGMs[lm].GetComponent<LevelGenerator>().NewLevelCandidate();
        offspringIter++;

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

        int r = Random.Range(0, 5);

        Offspring.Clear();

        for (int i = 0; i < parent1.Count; i++)
        {
            if (!flipped)
            {
                if (r < i)
                {
                    Offspring.Add(parent1[1]);
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

        Debug.Log("Flipped = " + flipped.ToString());
        Debug.Log("CrossoverPoint = " + r.ToString());
        return Offspring;
    }
}
