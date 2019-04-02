using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField] Text generation;
    [SerializeField] Text candidate;
    [SerializeField] Text timeScale;
    [SerializeField] Text fitness;

    [SerializeField] GameObject EndSlate;

    [SerializeField] SetUpManager setUp;

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
        candidate.text = "Candidates: " + cand.ToString() + "/" + setUp.candidateReq;
    }

    public void UpdateTimeScale(int ts)
    {
        timeScale.text = "Time Scale: x" + ts.ToString();
    }

    public void UpdateFitness(int fit)
    {
        fitness.text = "Best accuracy: " + fit.ToString() + "%";
    }

    public void ShowEndSlate()
    {
        EndSlate.gameObject.SetActive(true);
    }
}
