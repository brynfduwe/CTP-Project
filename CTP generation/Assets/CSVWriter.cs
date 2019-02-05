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
	void Start ()
	{
	    //FileStream fs = new FileStream(csvFilePath, FileMode.Append, FileAccess.Write, FileShare.Write);
	    //fs.Close();
	    //StreamWriter writer = new StreamWriter(csvFilePath, append: true);
	    //writer.Write("GEN,CANDIDATE,ID,ACTION" + "\n");
	    //writer.Close();
    }

    public void Write(int generation, int candidate, int iD, int actions)
    {
        FileStream fs = new FileStream(csvFilePath, FileMode.Append, FileAccess.Write, FileShare.Write);
        fs.Close();
        StreamWriter writer = new StreamWriter(csvFilePath, append: true);
        writer.Write(generation.ToString() + "," + candidate.ToString() + "," + iD.ToString() + "," + actions.ToString() + "," + "\n");
        writer.Close();
    }
}

