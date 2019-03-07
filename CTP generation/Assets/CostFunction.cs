using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CostFunction : MonoBehaviour
{


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

        float max = 2 * curveDesign.Count - 1;
        float cost = (totalError/max); //noramlise data
        return cost;
    }


}
