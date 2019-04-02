using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandAloneLevelTester : MonoBehaviour
{
    public GameObject levelGeneratorPrefab;
    LevelGenerator levelGenerator;

    // Use this for initialization
    void Start ()
	{
	    StandAloneLevelManager salm = GameObject.Find("StandAloneLevelManager").GetComponent<StandAloneLevelManager>();
        GameObject lvlGen = Instantiate(levelGeneratorPrefab);
	    levelGenerator = lvlGen.GetComponent<LevelGenerator>();
	  //  levelGenerator.SetNewChain(salm.transitionMatrix);
	    levelGenerator.SetTransitionPath(salm.transitionPath, true);
	    levelGenerator.MyStart(salm.height, salm.length, salm.transitions, salm.mapping, salm.historySteps, salm.stateAmounts);
        levelGenerator.NewLevelCandidate();
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
