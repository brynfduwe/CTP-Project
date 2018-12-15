﻿using System.Collections;
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

    public List<List<int[]>> CurrentOffspring = new List<List<int[]>>();

    public int offspringIter;

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
	        if (generation  == 1)
	        {
	            GetComponent<LevelGenerator>().RandomChain();
                GetComponent<LevelGenerator>().NewLevelCandidate();
	        }
	        else
	        {
	            GetComponent<LevelGenerator>().NewLevelCandidate();
	        }

	        FitnessTimer = 0;
	        candidate++;
	        UImanager.UpdateCandidate(candidate);
        }

	    if (player.transform.position.x >= GetComponent<LevelGenerator>().levelLength)
	    {
	        if (generation == 1)
	        {
                AddCandidate();
	        FitnessTimer = 0;

	            if (CandidateList.Count >= 5)
	            {
	                player.transform.position = new Vector2(-100, -100);

	                generation++;
	                UImanager.UpdateGeneration(generation);
	                candidate = 1;
	                UImanager.UpdateCandidate(candidate);

	                Selection();

	                CandidateList.Clear(); ;
                }
	        }
	        else
	        {
                AddCandidate();
	            FitnessTimer = 0;

                if (CandidateList.Count >= 5)
	            {
	                player.transform.position = new Vector2(-100, -100);

	                generation++;
	                UImanager.UpdateGeneration(generation);
	                candidate = 1;
	                UImanager.UpdateCandidate(candidate);

                    Selection();

                    CandidateList.Clear();;
	            }
                else
                {
                    
                    GetComponent<LevelGenerator>().SetNewChain(CurrentOffspring[offspringIter]);
                    GetComponent<LevelGenerator>().NewLevelCandidate();
                    offspringIter++;
                    if (offspringIter >= CurrentOffspring.Count)
                        offspringIter = 0;
                }
            }
	    }
	}

    void AddCandidate()
    {
        candidate++;
        UImanager.UpdateCandidate(candidate);

        CandidateList.Add(GetComponent<LevelGenerator>().GetGeneratorChromosome());
        CandidateFitness.Add(FitnessTimer);

        if (generation == 1)
        {
            GetComponent<LevelGenerator>().RandomChain();
            GetComponent<LevelGenerator>().NewLevelCandidate();
        }
        else
        {
            GetComponent<LevelGenerator>().NewLevelCandidate();
        }
    }


    void Selection()
    {
        Debug.Log("SELECTION");
        //change to roulette wheel

        CandidateFitness.Clear();
        CurrentOffspring.Clear();

        for (int i = 0; i < 10; i++)
        {
            List<int[]> Offspring = new List<int[]>();
            Offspring = Crossover(CandidateList[Random.Range(0, CandidateList.Count)], CandidateList[Random.Range(0, CandidateList.Count)]);
            CurrentOffspring.Add(Offspring);
        }

        CandidateList.Clear();

        GetComponent<LevelGenerator>().SetNewChain(CurrentOffspring[0]);
        GetComponent<LevelGenerator>().NewLevelCandidate();
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
