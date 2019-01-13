using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetUpManager : MonoBehaviour
{
    public Slider restCov;


    // Use this for initialization
    void Start()
    {

    }


    public float GetRestCov()
    {
        return restCov.value;
    }

}
