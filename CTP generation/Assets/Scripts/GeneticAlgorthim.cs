using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorthim : MonoBehaviour
{
    public Transform player;
    public int failedYpos;

    private List<List<LevelGenerator>> candidateChains = new List<List<LevelGenerator>>();

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (player.transform.position.y < failedYpos)
	    {
	        GetComponent<LevelGenerator>().GetGeneratorChromosome();
	    }
		
	}


    public void AddCandidate(List<LevelGenerator> level)
    {
        candidateChains.Add(level);
    }
}
