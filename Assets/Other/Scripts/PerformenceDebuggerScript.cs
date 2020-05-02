using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformenceDebuggerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("updateFPS", 0, 0.2f);
    }

    // Update is called once per frame
    void Update()
    {

    }
    void updateFPS() 
    {
        fps = Mathf.Lerp(fps, (1f / Time.unscaledDeltaTime), 0.5f);
        deltatime = Mathf.Lerp(deltatime, Time.unscaledDeltaTime, 0.5f);
    }
    float fps;//Frames per second
    float deltatime;//Delay in seconds between each frame
    private void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 500, 100), "FPS : " + Mathf.RoundToInt(fps));
    }

}
