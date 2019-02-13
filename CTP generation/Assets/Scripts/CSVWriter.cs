using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEditor;

public class CSVWriter : MonoBehaviour
{
    public string csvFilePath;
	
	// Update is called once per frame
	void Awake ()
	{
	    StreamWriter writer = new StreamWriter(csvFilePath, append: false);
        writer.Flush();
	    writer.Close();
    }


    public void WriteTestInfo(int height, int levelLength)
    {
        FileStream fs = new FileStream(csvFilePath, FileMode.Append, FileAccess.Write, FileShare.Write);
        fs.Close();
        StreamWriter writer = new StreamWriter(csvFilePath, append: true);
        writer.Write("Timestamp rate, 0.1s" + "\n" 
                     + "Level Height" + "," + height.ToString() + "\n" 
                     + "Level Length" + "," + levelLength.ToString() + "\n"
                      +"Actions Key, 0 = move right, 1 = small jump, 2 = high jump \n");
        writer.Close();
    }


    public void Write(int generation, int candidate, List<int> actions)
    {
        string actionList = "ACTIONS,";
        foreach (var a in actions)
        {
            actionList += a + ",";
        }

        FileStream fs = new FileStream(csvFilePath, FileMode.Append, FileAccess.Write, FileShare.Write);
        fs.Close();
        StreamWriter writer = new StreamWriter(csvFilePath, append: true);
        writer.Write(actionList + "\n");
        writer.Close();
    }


    public void WriteFitness(float fitness)
    {
        FileStream fs = new FileStream(csvFilePath, FileMode.Append, FileAccess.Write, FileShare.Write);
        fs.Close();
        StreamWriter writer = new StreamWriter(csvFilePath, append: true);
        writer.Write("\nFitness," + fitness.ToString() + "\n\n");
        writer.Close();
    }

    public void WriteCandidate(List<int[]> transitionMatrix, int candidate, int gen)
    {
        string ptm = "\n";
        foreach (var ptl in transitionMatrix)
        {
            foreach (var i in ptl)
            {
                ptm += (i.ToString() + ", ");
            }
            ptm += "\n";
        }

        FileStream fs = new FileStream(csvFilePath, FileMode.Append, FileAccess.Write, FileShare.Write);
        fs.Close();
        StreamWriter writer = new StreamWriter(csvFilePath, append: true);
        writer.Write("\nCandidate," + (candidate+1).ToString() + "\nGeneration," + gen.ToString() + "\n" + ptm + "\n");
        writer.Close();
    }
}

