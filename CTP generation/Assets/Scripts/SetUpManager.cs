using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetUpManager : MonoBehaviour
{
    public bool saveImgs;
    public int testers;
    public int testersOnScreen;
    //  public Button start;
    public int height;
    public int length;
    public int populationOffspring;
    public int candidateReq;

    public bool spikes;

    // Use this for initialization
    void Start()
    {

    }


    public float GetTesterNum()
    {
        return testers;
    }


    public void DisableStartUI()
    {
        //restCov.interactable = false;
        //start.interactable = false;
        //testers.interactable = false;
        //height.interactable = false;
        //length.interactable = false;
        //candidateReq.interactable = false;
        //populationOffspring.interactable = false;
    }
}
