using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEditor;

public class CSVWriter : MonoBehaviour
{
    public string csvFilePath;
    public string csvConvergenceCurvePath;
    private string writeData = "";

    // Update is called once per frame
    void Awake ()
	{
	    StreamWriter writer = new StreamWriter(csvFilePath, append: false);
	    StreamWriter writer2 = new StreamWriter(csvConvergenceCurvePath, append: false);
        writer.Flush();
	    writer.Close();
	    writer2.Flush();
	    writer2.Close();
    }


    public void WriteTestInfo(int height, int levelLength, float fitnessReq)
    {
        FileStream fs = new FileStream(csvFilePath, FileMode.Append, FileAccess.Write, FileShare.Write);
        fs.Close();
        StreamWriter writer = new StreamWriter(csvFilePath, append: true);
        writer.Write("Fitness Required," + fitnessReq.ToString() + "\n"
                     + "Level Height" + "," + height.ToString() + "\n" 
                     + "Level Length" + "," + levelLength.ToString() + "\n");
        writer.Close();
    }


    public void Write(int generation, int candidate, List<float> actions)
    {
        string actionList = "ACTIONS,";
        foreach (var a in actions)
        {
            actionList += a + ",";
        }

        AddToCandidateData(actionList + "\n");

    }


    public void WriteFitness(float fitness)
    {
        AddToCandidateData("\nFitness," + fitness.ToString() + "\n\n");
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
        AddToCandidateData("\nCandidate," + (candidate+1).ToString() + "\nGeneration," + gen.ToString() + "\n" + ptm + "\n");
    }


    void AddToCandidateData(string s)
    {
        writeData += s;
    }

    public void CandidateToCSVAndClear(bool sucessfullCandidte)
    {
        if (sucessfullCandidte)
        {
            FileStream fs = new FileStream(csvFilePath, FileMode.Append, FileAccess.Write, FileShare.Write);
            fs.Close();
            StreamWriter writer = new StreamWriter(csvFilePath, append: true);
            writer.Write(writeData + "\n");
            writer.Close();
        }

        writeData = "";
    }





    public void WriteConvergenceDesignCurve(List<float> designCurve)
    {
        string actionList = "Desired Curve,";
        foreach (var a in designCurve)
        {
            actionList += a + ",";
        }

        FileStream fs = new FileStream(csvConvergenceCurvePath, FileMode.Append, FileAccess.Write, FileShare.Write);
        fs.Close();
        StreamWriter writer = new StreamWriter(csvConvergenceCurvePath, append: true);
        writer.Write(actionList + "\n");
        writer.Close();

    }


    public void WriteConvergence(List<float> actions)
    {
        string actionList = "ACTIONS,";
        foreach (var a in actions)
        {
            actionList += a + ",";
        }

        FileStream fs = new FileStream(csvConvergenceCurvePath, FileMode.Append, FileAccess.Write, FileShare.Write);
        fs.Close();
        StreamWriter writer = new StreamWriter(csvConvergenceCurvePath, append: true);
        writer.Write(actionList + "\n");
        writer.Close();

    }

    public void WriteConvergenceNewGen(int gen)
    {
        FileStream fs = new FileStream(csvConvergenceCurvePath, FileMode.Append, FileAccess.Write, FileShare.Write);
        fs.Close();
        StreamWriter writer = new StreamWriter(csvConvergenceCurvePath, append: true);
        writer.Write("\n\n" + "Generation," + gen.ToString() + "\n");
        writer.Close();
    }

    public void WriteFinal(float[] actions)
    {
        string actionList = "ACTIONS,";
        foreach (var a in actions)
        {
            actionList += a + ",";
        }

        FileStream fs = new FileStream(csvConvergenceCurvePath, FileMode.Append, FileAccess.Write, FileShare.Write);
        fs.Close();
        StreamWriter writer = new StreamWriter(csvConvergenceCurvePath, append: true);
        writer.Write("\n\n" + "END Best Candidate\n" + actionList);
        writer.Close();
    }
}

