using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.GameCenter;

public class SceneManager : MonoBehaviour
{
    public List<int[]> markovChromo = new List<int[]>();

	// Use this for initialization
	void Start ()
	{
		DontDestroyOnLoad(this);
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}


    public void LoadScene(int indx)
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene(indx);
    }


    public void SetChrmo(List<int[]> x)
    {
        markovChromo = x;
    }


    public List<int[]> GetChromo()
    {
        return markovChromo;
    }
}
