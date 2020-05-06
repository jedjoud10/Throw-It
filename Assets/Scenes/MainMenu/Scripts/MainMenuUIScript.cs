using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Handler for main menu UI
public class MainMenuUIScript : MonoBehaviour
{
    public GameObject MultiplayerSelectScreen;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Show multiplayer select screen
    public void ShowMultiplayerUI() { MultiplayerSelectScreen.SetActive(true); }
    //Hides multiplayer select screen
    public void HideMultiplayerUI() { MultiplayerSelectScreen.SetActive(false); }
}
