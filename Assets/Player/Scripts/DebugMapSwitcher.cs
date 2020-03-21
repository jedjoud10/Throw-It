using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//Switches between the two maps using two keys
public class DebugMapSwitcher : MonoBehaviour
{
    public string[] maps;//Maps that we can load by clicking the key "H"
    private int index;//Index of current map
    private DebugControls debugControls;
    private bool switchMap;
    private void Awake()
    {
        debugControls = new DebugControls();
        debugControls.Map.SwitchMap.performed += ctx => switchMap = ctx.ReadValueAsButton();
    }
    // Update is called once per frame
    void Update()
    {
        if (switchMap) 
        {
            index += 1;
            index = index % maps.Length;
            SceneManager.LoadScene(maps[index], LoadSceneMode.Single);
        }
    }
}
