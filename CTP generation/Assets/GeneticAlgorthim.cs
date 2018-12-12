using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorthim : MonoBehaviour
{
    private List<List<LevelGenerator>> candidateChains = new List<List<LevelGenerator>>();

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void AddCandidate(List<LevelGenerator> level)
    {
        candidateChains.Add(level);
    }
}
