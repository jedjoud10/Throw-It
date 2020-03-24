using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
//Loops over maps when pressing a key
public class DebugMapSwitcher : MonoBehaviour
{
    public string[] maps;//Maps that we can load by clicking the key "H"
    private int index;//Index of current map
    void Start() 
    {
        index = System.Array.IndexOf(maps, SceneManager.GetActiveScene().name);
    }
    // Update is called once per frame    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)) 
        {
            index += 1;
            SceneManager.LoadScene(maps[index % maps.Length], LoadSceneMode.Single);
        }
    }
}
