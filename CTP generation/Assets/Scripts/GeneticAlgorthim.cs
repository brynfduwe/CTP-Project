using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorthim : MonoBehaviour
{
    public Transform player;
    public int failedYpos;

    public UIManager UImanager;
    private int generation = 1;
    private int candidate = 1;
    private float FitnessTimer;

    public List<List<int[]>> CandidateList = new List<List<int[]>>();
    public List<float> CandidateFitness = new List<float>();

    // Use this for initialization
    void Start ()
    {
        UImanager.UpdateCandidate(candidate);
        UImanager.UpdateGeneration(generation);
    }
	
	// Update is called once per frame
	void Update ()
	{
	    FitnessTimer += Time.deltaTime;

        if (player.transform.position.y < failedYpos)
	    {
	        if (generation < 2)
	        {
	            GetComponent<LevelGenerator>().NewLevelCandidate(true);
	        }
	        else
	        {
	            GetComponent<LevelGenerator>().NewLevelCandidate(false);
	        }

	        FitnessTimer = 0;
	        candidate++;
	        UImanager.UpdateCandidate(candidate);
        }

	    if (player.transform.position.x >= GetComponent<LevelGenerator>().levelLength)
	    {
            AddCandidate();
	        FitnessTimer = 0;

	        if (CandidateList.Count > 5)
	        {
                player.transform.position = new Vector2(-100,-100);

	            generation++;
	            UImanager.UpdateGeneration(generation);
	            candidate = 1;

                Selection();
	        }
        }
	}

    void AddCandidate()
    {
        candidate++;
        UImanager.UpdateCandidate(candidate);

        CandidateList.Add(GetComponent<LevelGenerator>().GetGeneratorChromosome());
        CandidateFitness.Add(FitnessTimer);

        if (generation < 2)
        {
            GetComponent<LevelGenerator>().NewLevelCandidate(true);
        }
        else
        {
            GetComponent<LevelGenerator>().NewLevelCandidate(false);
        }
    }


    void Selection()
    {
        List<List<int[]>> SelectedChains = new List<List<int[]>>();

        //parent 1
        float best = 10000;
        int bestIter = CandidateFitness.Count;

        for (int i = 0; i < CandidateFitness.Count; i++)
        {
            if (CandidateFitness[i] < best)
            {
                best = CandidateFitness[i];
                bestIter = i;
            }
        }

        SelectedChains.Add(CandidateList[bestIter]);
        CandidateList.Remove(CandidateList[bestIter]);
        CandidateFitness.Remove(CandidateFitness[bestIter]);

        //parent2
        float best2 = 10000;
        int bestIter2 = CandidateFitness.Count;

        for (int i = 0; i < CandidateFitness.Count; i++)
        {
            if (CandidateFitness[i] < best2)
            {
                best2 = CandidateFitness[i];
                bestIter2 = i;
            }
        }

        SelectedChains.Add(CandidateList[bestIter2]);
        CandidateList.Clear();
        CandidateFitness.Clear();

        SelectedChains = Crossover(SelectedChains);

        GetComponent<LevelGenerator>().SetNewChain(SelectedChains);
        GetComponent<LevelGenerator>().NewLevelCandidate(false);

    }


    List<List<int[]>> Crossover(List<List<int[]>> parents)
    {
        List<List<int[]>> Offspring = new List<List<int[]>>();
        Offspring.Add(parents[0]);
        Offspring.Add(parents[1]);

        int r = Random.Range(0, parents[0][0].Length);

        for (int i = 0; i < parents[0][0].Length; i++)
        {
            if (r < i)
            {
                Offspring[0][i] = parents[1][i];
                Offspring[1][i] = parents[0][i];
            }
        }

        return Offspring;
    }
}
