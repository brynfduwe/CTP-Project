using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class TreeGeneticGenAlg : MonoBehaviour
{
    public GameObject levelGeneratorPrefab;
    public List<GameObject> levelGenList = new List<GameObject>();

    // Use this for initialization
    public void InitTreeGen (List<Vector2> posList, int heightTrans, int levelLength, int restCov) 
    {
        for (int i = 0; i < posList.Count; i++)
        {
            GameObject gobj = Instantiate(levelGeneratorPrefab, posList[i], transform.rotation);
            levelGenList.Add(gobj);
        }

        foreach (var LG in levelGenList)
        {
            LG.GetComponent<LevelGenerator>().MyStart(heightTrans, levelLength);
            LG.GetComponent<LevelGenerator>().SetRests(restCov);
            LG.GetComponent<LevelGenerator>().RandomChain();
            LG.GetComponent<LevelGenerator>().NewLevelCandidate();
        }
    }
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
