using System.IO;
using UnityEngine;

public class Timelapsed : MonoBehaviour
{
    public string m_Filename = "Screenshot";
    public float m_RepeatTime = 1.0f;
    public float m_Delay = 1.0f;

    public string m_Path;

    private void Start()
    {   
        Directory.CreateDirectory($"{Application.dataPath}/../Save");
        InvokeRepeating("Screenshot", m_Delay, m_RepeatTime);
    }

    private void Screenshot()
    {
        ScreenCapture.CaptureScreenshot($"{Application.dataPath}/../Save/{m_Filename}_{Time.time}.png");
    }
}