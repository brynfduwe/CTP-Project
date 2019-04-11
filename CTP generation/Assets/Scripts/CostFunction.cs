using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CostFunction : MonoBehaviour
{
    private SetUpManager.MappingType mapping;

    public void SetMapping(SetUpManager.MappingType set)
    {
        mapping = set;
    }

    public float CalculateCost(List<float> curveDesign, List<float> curveAgent)
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
                error = 1;
            }

            error = Mathf.Abs(error);

            totalError += error;
        }

        float max = curveDesign.Count; ;

        float cost = (totalError / max); //noramlise data
        return cost;
    }
}
