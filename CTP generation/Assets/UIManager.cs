using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField] Text generation;
    [SerializeField] Text candidate;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void UpdateGeneration(int gen)
    {
        generation.text = "Generation: " + gen.ToString();
    }

    public void UpdateCandidate(int cand)
    {
        candidate.text = "Candidate: " + cand.ToString();
    }
}
