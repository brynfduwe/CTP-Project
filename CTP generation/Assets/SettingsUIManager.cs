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

    public int[] heights;

    public Text heightText;
    public Text mappingText;
    public Text spikeText;

    // Use this for initialization
    void Start ()
	{
	    heightText.text = heights[heightSelected].ToString();
	    mappingText.text = mappingSelected.ToString();
	    spikeText.text = "Spikes: Off";
	}


    public void startGeneticAlgorthim()
    {
        setUpManager.SetSettingFromUI(heights[heightSelected], mappingSelected, spikesSelected);

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
    }

    public void lastHeight()
    {
        heightSelected--;
        if (heightSelected < 0)
        {
            heightSelected = heights.Length - 1;
        }
        heightText.text = heights[heightSelected].ToString();
    }
}
