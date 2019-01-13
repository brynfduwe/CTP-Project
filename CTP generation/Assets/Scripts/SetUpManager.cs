using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetUpManager : MonoBehaviour
{
    public Slider restFreq;
    public Slider restLength;

    // Use this for initialization
    void Start()
    {

    }


    public float GetRestFreq()
    {
        return restFreq.value;
    }

    public float GetRestLength()
    {
        return restLength.value;
    }
}
