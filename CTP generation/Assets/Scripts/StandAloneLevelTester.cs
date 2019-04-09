using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StandAloneLevelTester : MonoBehaviour
{
    public GameObject levelGeneratorPrefab;
    LevelGenerator levelGenerator;
    public Text fitnessUI;

    // Use this for initialization
    void Start ()
	{
	    StandAloneLevelManager salm = GameObject.Find("StandAloneLevelManager").GetComponent<StandAloneLevelManager>();
        GameObject lvlGen = Instantiate(levelGeneratorPrefab);
	    levelGenerator = lvlGen.GetComponent<LevelGenerator>();
	    levelGenerator.SetTransitionPath(salm.transitionPath, true);
	    levelGenerator.MyStart(salm.height, salm.length, salm.transitions, salm.mapping, salm.historySteps, salm.stateAmounts);
        levelGenerator.NewLevelCandidate();

	    fitnessUI.text = "Fitness: " + (int)(System.Math.Round(salm.fitness, 2) * 100) + "%";
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (Input.GetKeyDown(KeyCode.Alpha0))
	    {
	        levelGenerator.NewLevelCandidate();
        }
	}
}
