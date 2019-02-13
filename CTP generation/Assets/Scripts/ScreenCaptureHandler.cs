using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenCaptureHandler : MonoBehaviour
{
    public string folderPath;

    public void ScreenGrab(Vector3 pos, int gen, int cand, int ID)
    {
        PositionCamera(pos);
        string fileName = "gen" + gen + "_can" + cand + "_lvl" + ID.ToString() + ".png";
        ScreenCapture.CaptureScreenshot(folderPath + fileName);
    }

    public void PositionCamera(Vector3 pos)
    {
        Camera.main.transform.position = new Vector3(pos.x + 45, pos.y, Camera.main.transform.position.z);
    }


    public void MoveUIHighlight(Vector3 pos)
    {

    }
}
