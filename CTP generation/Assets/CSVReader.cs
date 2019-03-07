using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class CSVReader : MonoBehaviour
{

    public TextAsset file;
    public List<string[]> values = new List<string[]>();


    // Use this for initialization
    void Start ()
	{
		ReadValues();
	}


    void ReadValues()
    {
        string[] rows = file.text.Split('\n');

        foreach (var r in rows)
        {
            string[] cols = r.Split(',');
            List<string> splitCol = new List<string>();
            foreach (var c in cols)
            {
                splitCol.Add(c);
            }
            values.Add(splitCol.ToArray());
        }
    }


    public List<int> getOrderedCurveValues()
    {
        List<int> orderedValues = new List<int>();

        for(int i = 0; i < values.Count - 1; i++)
        {
            //add second column = value
            orderedValues.Add(int.Parse(values[i][1]));
        }

        return orderedValues;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
