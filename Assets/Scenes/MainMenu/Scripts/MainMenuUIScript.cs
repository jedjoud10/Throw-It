using MLAPI;
using MLAPI.Transports.Tasks;
using RufflesTransport;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
//Handler for main menu UI
public class MainMenuUIScript : MonoBehaviour
{
    public GameObject multiplayerSelectScreen;
    public RufflesTransport.RufflesTransport transport;//The networking transport to use
    public InputField IPField;//Field where we will write the ip we want to connect to
    public float autoDisconnectTimeout;//If a player has started an accidental client and there was no host, then disconnect after this ammount of seconds
    public List<string> selectableScenes;//Scene names that the player can select that will cahnge the map when they host a game
    public Dropdown selectHostScene;//The selection menu to select a scene that the player will go to when they start hosting
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SetupSceneChoices();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Setup the scene choices
    private void SetupSceneChoices() 
    {
        selectHostScene.ClearOptions();//Clear the selection options
        selectHostScene.AddOptions(selectableScenes);
        selectHostScene.RefreshShownValue();//Refresh the value just in case
    }
    //Join a server
    public void JoinServer() 
    {
        string address = IPField.text;
        PingReply ping = Ping(address);
        if (ping != null)
        {
            Debug.Log("RoundTrip time: " + ping.RoundtripTime);
            transport.ConnectAddress = address;
            NetworkingManager.Singleton.StartClient();
            Invoke("DisconnectClientTimeout", autoDisconnectTimeout);
        }
    }
    private PingReply Ping(string address) 
    {
        System.Net.NetworkInformation.Ping pingSender = new System.Net.NetworkInformation.Ping();
        PingOptions options = new PingOptions();

        // Use the default Ttl value which is 128,
        // but change the fragmentation behavior.
        options.DontFragment = true;

        // Create a buffer of 32 bytes of data to be transmitted.
        string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        byte[] buffer = System.Text.Encoding.ASCII.GetBytes(data);
        int timeout = 120;
        PingReply reply = pingSender.Send(address, timeout, buffer, options);
        if(reply.Status == IPStatus.Success) 
        {
            return reply;
        }
        else
        {
            return null;
        }
    }
    //Start as host
    public void StartHost() 
    {
        SceneManager.LoadScene(selectableScenes[selectHostScene.value]);
    }
    //Exit the game
    public void ExitGame() { Application.Quit(); }
    //Disconnect the player because they started a client and there was no host
    private void DisconnectClientTimeout() 
    {
        NetworkingManager.Singleton.StopClient();
        Debug.LogError("Timeout. No host found.");
    }
    //Show multiplayer select screen
    public void ShowMultiplayerUI() { multiplayerSelectScreen.SetActive(true); }
    //Hides multiplayer select screen
    public void HideMultiplayerUI() { multiplayerSelectScreen.SetActive(false); }
}
