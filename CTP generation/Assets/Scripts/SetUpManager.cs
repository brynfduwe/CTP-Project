﻿using System.Collections;
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
    public float endFitnessReq;

    public bool spikes;
    public bool hearts;

    public enum MappingType
    {
        InputChangeRate,
        JumpsPerSecond,
        JumpsIn1SecondTo5secondRatio,
        JumpAmountDifference,
        COUNT
    }

    public MappingType mapping;


    public void SetSettingFromUI(int _height, MappingType _mapping, bool _spikes)
    {
        spikes = _spikes;
        height = _height;
        mapping = _mapping;
    }
}
