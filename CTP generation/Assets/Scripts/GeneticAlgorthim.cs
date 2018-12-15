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

    public List<List<int[]>> CurrentOffspring = new List<List<int[]>>();

    private int offspringIter;

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
	        if (generation == 1)
	        {
                AddCandidate();
	        FitnessTimer = 0;

	            if (CandidateList.Count > 5)
	            {
	                player.transform.position = new Vector2(-100, -100);

	                generation++;
	                UImanager.UpdateGeneration(generation);
	                candidate = 1;
	                UImanager.UpdateCandidate(candidate);

                    Selection();
	            }
	        }
	        else
	        {
                AddCandidate();
	            FitnessTimer = 0;

                if (CandidateList.Count > 5)
	            {
	                player.transform.position = new Vector2(-100, -100);

	                generation++;
	                UImanager.UpdateGeneration(generation);
	                candidate = 1;
	                UImanager.UpdateCandidate(candidate);

                    Selection();
	            }
                else
                {
                    
                    GetComponent<LevelGenerator>().SetNewChain(CurrentOffspring[offspringIter]);
                    GetComponent<LevelGenerator>().NewLevelCandidate(false);
                    offspringIter++;
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
        List<List<int[]>> OffspringCandidates = new List<List<int[]>>();


        //change to roulette wheel

        SelectedChains.Add(CandidateList[4]);
        SelectedChains.Add(CandidateList[0]);

        CandidateList.Clear();
        CandidateFitness.Clear();


        OffspringCandidates = Crossover(SelectedChains);

        offspringIter = 0;

        CurrentOffspring = OffspringCandidates;

        GetComponent<LevelGenerator>().SetNewChain(CurrentOffspring[0]);
        GetComponent<LevelGenerator>().NewLevelCandidate(false);

        foreach (var o in CurrentOffspring)
        {
            foreach (var i in o)
            {
           //     Debug.Log(i[0].ToString() + "," + i[1].ToString() + "," + i[2].ToString() + "," + i[3].ToString() + "," + i[4].ToString());
            }
        }
    }


    List<List<int[]>> Crossover(List<List<int[]>> parents)
    {
        List<List<int[]>> Offsprings = new List<List<int[]>>();
      //  Offspring.Add(parents[0]);
      //  Offspring.Add(parents[1]);

        for (int j = 0; j < 10; j++)
        {
            bool flipped = false;

            if ((Random.Range(0, 10)) >= 5)
            {
                flipped = true;
            }

            int r = Random.Range(0, 5);

            List<int[]> Offspring = new List<int[]>();

            for (int i = 0; i < 5; i++)
            {
                if (!flipped)
                {
                    if (r < i)
                    {
                        Offspring.Add(parents[1][i]);
                    }
                    else
                    {
                        Offspring.Add(parents[0][i]);
                    }
                }
                else
                {
                    if (r < i)
                    {
                        Offspring.Add(parents[0][i]);
                    }
                    else
                    {
                        Offspring.Add(parents[1][i]);
                    }
                }
            }

            Offsprings.Add(Offspring);
        }

        return Offsprings;
    }
}
