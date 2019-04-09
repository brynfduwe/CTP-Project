using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUIManager : MonoBehaviour
{
    public GameObject geneticAlgorthimObj;
    public SetUpManager setUpManager;
    public Button[] setUpButtons;

    private SetUpManager.MappingType mappingSelected = 0;
    private int heightSelected = 1;
    private bool spikesSelected = false;
    private int curveSelected = 1;

    public int[] heights;
    public TextAsset[] curveFiles;

    public Text heightText;
    public Text mappingText;
    public Text spikeText;
    public Text curveText;

    // Use this for initialization
    void Start ()
	{
	    heightText.text = heights[heightSelected].ToString();
	    mappingText.text = mappingSelected.ToString();
	    spikeText.text = "Spikes: Off";
	    curveText.text = curveFiles[curveSelected].name;
	}


    public void startGeneticAlgorthim()
    {
        setUpManager.SetSettingFromUI(heights[heightSelected], mappingSelected, spikesSelected, curveFiles[curveSelected]);

        foreach (var btn in setUpButtons)
        {
            btn.interactable = false;
        }

        geneticAlgorthimObj.SetActive(true);
    }


    public void spikeToggle()
    {
        spikesSelected = !spikesSelected;

        if (spikesSelected)
        {
            spikeText.text = "Spikes: On";
            for (int i = 0; i < heights.Length - 1; i++)
            {
                if (heights[i] == 4)
                {
                    heightSelected = i;
                    heightText.text = heights[heightSelected].ToString();
                }
            }
        }
        else
        {
            spikeText.text = "Spikes: Off";
        }
    }


    public void nextMapping()
    {
        mappingSelected++;
        if (mappingSelected >= SetUpManager.MappingType.COUNT)
        {
            mappingSelected = 0;
        }
        mappingText.text = mappingSelected.ToString();
    }

    public void lastMapping()
    {
        mappingSelected--;
        if (mappingSelected < 0)
        {
            mappingSelected = SetUpManager.MappingType.COUNT - 1;
        }
        mappingText.text = mappingSelected.ToString();
    }


    public void nextHeight()
    {
        heightSelected++;
        if (heightSelected >= heights.Length)
        {
            heightSelected = 0;
        }
        heightText.text = heights[heightSelected].ToString();

        spikesSelected = false;
        spikeText.text = "Spikes: Off";
    }

    public void lastHeight()
    {
        heightSelected--;
        if (heightSelected < 0)
        {
            heightSelected = heights.Length - 1;
        }
        heightText.text = heights[heightSelected].ToString();

        spikesSelected = false;
        spikeText.text = "Spikes: Off";
    }

    public void nextCurve()
    {
        curveSelected++;
        if (curveSelected >= curveFiles.Length)
        {
            curveSelected = 0;
        }
        curveText.text = curveFiles[curveSelected].name;
    }

    public void lastCurve()
    {
        curveSelected--;
        if (curveSelected < 0)
        {
            curveSelected = curveFiles.Length - 1;
        }
        curveText.text = curveFiles[curveSelected].name;
    }
}
