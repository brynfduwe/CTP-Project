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
    public float minimumFitnessReq;
    public int historySteps;
    public int endAfterGen;
    public float mutationRate;

    public bool spikes;
    public bool hearts;

    public enum MappingType
    {
        InputChangeRate,
        ReactionDelay,
        Health,
        JumpsPerSecond,
        JumpsIn1SecondTo5secondRatio,
        JumpAmountDifference,
    }

    public MappingType mapping;

    // Use this for initialization
    void Start()
    {

    }
}
