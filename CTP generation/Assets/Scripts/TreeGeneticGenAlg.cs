using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class TreeGeneticGenAlg : MonoBehaviour
{
    public GameObject levelGeneratorPrefab;
    public List<GameObject> levelGenList = new List<GameObject>();

    // Use this for initialization
    public void InitTreeGen (List<TreeSegment> posList, int heightTrans, int levelLength, int restCov) 
    {
        for (int i = 0; i < posList.Count; i++)
        {
            GameObject gobj = Instantiate(levelGeneratorPrefab, posList[i].position, transform.rotation);

            if (posList[i].branch = true)
            {
                gobj.GetComponent<LevelGenerator>().SetBranchDirection(posList[i].branchDirection);
            }

            levelGenList.Add(gobj);
        }

        foreach (var LG in levelGenList)
        {
            LG.GetComponent<LevelGenerator>().NewLevelCandidate();
        }
    }
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
