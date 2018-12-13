using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorthim : MonoBehaviour
{
    public Transform player;
    public int failedYpos;

    public List<List<int[]>> CandidateList = new List<List<int[]>>();

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (player.transform.position.y < failedYpos)
	    {
            GetComponent<LevelGenerator>().NewLevelCandidate();
	    }

	    if (player.transform.position.x >= GetComponent<LevelGenerator>().levelLength)
	    {
            AddCandidate();
	    }
	}


    void AddCandidate()
    {
        CandidateList.Add(GetComponent<LevelGenerator>().GetGeneratorChromosome());
        GetComponent<LevelGenerator>().NewLevelCandidate();
    }
}
