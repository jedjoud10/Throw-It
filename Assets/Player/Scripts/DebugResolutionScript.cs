using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Loops over resolutions and sets the current screen resolution to the selected one
public class DebugResolutionScript : MonoBehaviour
{
    private DebugControls debugControls;
    private Resolution[] resolutions = { new Resolution(1080, 1920), new Resolution(900, 1600), new Resolution(768, 1366), new Resolution(720, 1280), new Resolution(480, 640) };
    private int currentResolution;

    private bool switchResolution;
    private void Awake()
    {
        debugControls = new DebugControls();
        debugControls.Resolution.ChangeResolution.performed += ctx => switchResolution = ctx.ReadValueAsButton();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (switchResolution) 
        {
            currentResolution++;
            Resolution current = resolutions[currentResolution % resolutions.Length];
            Screen.SetResolution(current.width, current.height, true);
        }
    }
    #region Enable/Disable
    private void OnEnable()
    {
        debugControls.Enable();
    }
    private void OnDisable()
    {
        debugControls.Disable();
    }
    #endregion 
}
public class Resolution 
{
    public int width;
    public int height;
    public Resolution(int _height, int _width)
    {
        width = _width;
        height = _height;
    }
}

