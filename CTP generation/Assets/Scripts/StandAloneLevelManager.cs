using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandAloneLevelManager : MonoBehaviour
{
    public int height;
    public int length;
    public int transitions;
    public int stateAmounts;
    public int historySteps;

    public int[] transitionPath;
    public List<int[]> transitionMatrix = new List<int[]>();

    public SetUpManager.MappingType mapping;

    public float fitness;


    // Use this for initialization
    void Start ()
	{
	    DontDestroyOnLoad(this);
    }


    public void SetUp(int _height, int _length, int _transitions, int _stateAmnts, int _historySteps,
        int[] path, List<int[]> matrix, SetUpManager.MappingType map, float fit)
    {
        height = _height;
        length = _length;
        transitions = _transitions;
        stateAmounts = _stateAmnts;
        historySteps = _historySteps;
        transitionPath = path;
        transitionMatrix = matrix;
        mapping = map;
        fitness = fit;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
