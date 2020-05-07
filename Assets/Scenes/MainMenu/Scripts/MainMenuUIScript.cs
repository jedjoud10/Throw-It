using MLAPI;
using MLAPI.Transports.Tasks;
using RufflesTransport;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Handler for main menu UI
public class MainMenuUIScript : MonoBehaviour
{
    public GameObject MultiplayerSelectScreen;
    public RufflesTransport.RufflesTransport transport;//The networking transport to use
    public InputField IPField;//Field where we will write the ip we want to connect to
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
    //Join a server
    public void JoinServer() 
    {
        transport.ConnectAddress = IPField.text;
        SocketTasks joinServerTasks = NetworkingManager.Singleton.StartClient();
        if (!joinServerTasks.Success) 
        { 
            NetworkingManager.Singleton.StopClient();
            Debug.LogError("Error: Invalid IP Adress");
        }
    }
    //Show multiplayer select screen
    public void ShowMultiplayerUI() { MultiplayerSelectScreen.SetActive(true); }
    //Hides multiplayer select screen
    public void HideMultiplayerUI() { MultiplayerSelectScreen.SetActive(false); }
}
