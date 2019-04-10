using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyTracker : MonoBehaviour
{
    private List<int> restLengths = new List<int>(); // number of platforms in that rest
    private List<int> rhythmGroupLengths = new List<int>();

    private float fitness = 0;
    public Text DifficultyUI;

    private float restsNumOf;
    private float restLengthAvg;


    public void CheckLevelDifficulty(GameObject[] levelPlats)
    {
        restLengths.Clear();
        rhythmGroupLengths.Clear();

        bool resting = false;

        int restCount = 0;
        int rbmCount = 0;

        for (int i = 0; i < levelPlats.Length; i++)
        {
            if (i > 0)
            {
                resting = false;
                if (Vector2.Distance(new Vector2(levelPlats[i - 1].transform.position.x, 0),
                        new Vector2(levelPlats[i].transform.position.x, 0)) < 1.25f)
                {
                    if (Vector2.Distance(new Vector2(0, levelPlats[i - 1].transform.position.y),
                            new Vector2(0, levelPlats[i].transform.position.y)) < 0.2f)
                    {
                        resting = true;
                    }
                }

                if (resting)
                {
                    restCount++;

                    //check to log rest
                    if (rbmCount > 0)
                    {
                        rhythmGroupLengths.Add(rbmCount + 1);

                        rbmCount = 0;
                    }
                }
                else
                {
                    rbmCount++;

                    //check to log rest
                    if (restCount > 0)
                    {
                        restLengths.Add(restCount + 1);
                        restCount = 0;
                    }
                }
            }
        }

        string s = "";
        foreach (var rl in restLengths)
        {
            s += rl.ToString();
            s += ", ";
        }

        string r = "";
        foreach (var rl in rhythmGroupLengths)
        {
            r += rl.ToString();
            r += ", ";
        }

        int rlScore = 0;
        int rglScore = 0;

        foreach (var rl in restLengths)
        {
            rlScore += rl;
        }

        restsNumOf = restLengths.Count;

        restLengthAvg = rlScore / restLengths.Count;

        foreach (var rgl in rhythmGroupLengths)
        {
            rglScore += rgl;
        }
    }


    public float GetRestNumOf()
    {
        return restsNumOf;
    }


    public float GetRestAvgLength()
    {
        return restLengthAvg;
    }

}
