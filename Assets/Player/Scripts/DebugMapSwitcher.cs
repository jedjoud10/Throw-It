using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//Switches between the two maps using two keys
public class DebugMapSwitcher : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))//Load map 1
        {
            SceneManager.LoadScene("Map1", LoadSceneMode.Single);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))//Load map 2
        {
            SceneManager.LoadScene("Map2", LoadSceneMode.Single);
        }
        if(Input.GetKeyDown(KeyCode.Alpha3))//Load map 3
        {
            SceneManager.LoadScene("world", LoadSceneMode.Single);
        }
    }
}
