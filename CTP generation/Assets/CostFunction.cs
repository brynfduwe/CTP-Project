using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CostFunction : MonoBehaviour
{
    private SetUpManager.MappingType mapping;

    public void SetMapping(SetUpManager.MappingType set)
    {
        mapping = set;
    }

    public float CalculateCost(List<int> curveDesign, List<int> curveAgent)
    {
        float totalError = 0;

        for (int i = 0; i < curveDesign.Count - 1; i++)
        {
            float error = 0;

            if (i < curveAgent.Count)
            {
                error = curveDesign[i] - curveAgent[i];
            }
            else
            {
                error = 2;
            }

            error = Mathf.Abs(error);

            totalError += error;

        }

        float max = 0;
        switch (mapping)
        {
            case SetUpManager.MappingType.InputChangeRate:
                max = 2 * curveDesign.Count;
                break;
            case SetUpManager.MappingType.ReactionDelay:
                max = 2 * curveDesign.Count;
                break;
            case SetUpManager.MappingType.Health:
                max = 2 * curveDesign.Count;
                break;
            default:
                max = 2 * curveDesign.Count;
                break;
        }



        float cost = (totalError/max); //noramlise data
        return cost;
    }
}
