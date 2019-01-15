using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetUpManager : MonoBehaviour
{
    public Slider restCov;
    public Slider testers;
    public Button start;
    public InputField height;
    public InputField length;
    public InputField populationOffspring;
    public InputField candidateReq;

    // Use this for initialization
    void Start()
    {

    }

    public float GetRestCov()
    {
        return restCov.value;
    }


    public float GetTesterNum()
    {
        return testers.value;
    }


    public void DisableStartUI()
    {
        restCov.interactable = false;
        start.interactable = false;
        testers.interactable = false;
        height.interactable = false;
        length.interactable = false;
        candidateReq.interactable = false;
        populationOffspring.interactable = false;
    }
}
