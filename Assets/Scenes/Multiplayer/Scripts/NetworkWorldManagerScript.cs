using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MLAPI.Spawning;
using MLAPI.Connection;
using MLAPI;
using System.IO;
using MLAPI.Messaging;
using System.Text;
using System.Linq;
using MLAPI.Serialization;
//Well, a world manager, but for multiplayer. Yea, pretty epic
public class  NetworkWorldManagerScript : NetworkedBehaviour
{
    private Transform PlayerSpawnPoint;//Position where the players will spawn
    public GameObject PlayerPrefab;//Prefab of the player
    private NetworkingManager singleton;
    private bool singleplayer;//Is the game in singleplayer ?
    // Start is called before the first frame update
    void Start()
    {
        singleton = NetworkingManager.Singleton;
        singleplayer = !singleton.IsHost && !singleton.IsClient;
        //If the player isnt a server and isnt a client
        if (singleplayer)
        {
            if (IsScene("MultiplayerLobbyMap")) //Start hosting if we are in the multiplayer lobby
            {
                PlayerSpawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawnPoint").transform;
                singleton.StartHost(PlayerSpawnPoint.position);//Spawn host in soon to be multiplayer session                
            }
            else if(!IsScene("MainMenuMap"))
            {
                PlayerSpawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawnPoint").transform;
                singleton.StartHost(PlayerSpawnPoint.position);//Spawn host in soon to be multiplayer session   
            }
        }
    }
    //When the server has start
    #region Custom Messaging
    //Sends a message to every client from a client
    public void SendMessageToServer(string message) 
    {
        if (!IsClient) return;
        Debug.Log("Sent message :" + message);

        InvokeServerRpc(ReceiveMessageServer, message);
    }
    //Message first goes to the server then to the clients
    [ServerRPC]
    private void ReceiveMessageServer(string message) 
    {
        InvokeClientRpcOnEveryone(ReceiveMessage, message);
    }

    //Receives a message from a client
    [ClientRPC]
    public void ReceiveMessage(string message) 
    {
        Debug.Log("Received message :" + message);
    }

    #endregion
    #region Scene management
    //Start the host
    public void HostMultiplayerLobby() { ChangeScene("MultiplayerLobbyMap"); }
    //Join the host as a client
    public void StartClient() { singleton.StartClient(); }
    //Return to the MainMenu
    public void ReturnMainMenu() 
    {
        if (!IsServer) return;
        InvokeClientRpcOnEveryone(ReturnMainMenuClient);
        singleton.StopHost();
        ChangeScene("MainMenuMap");
    }
    //Make all clients return to main menu
    [ClientRPC]
    private void ReturnMainMenuClient() 
    {
        //if (IsHost) return;
        singleton.StopClient();
        ChangeScene("MainMenuMap");
    }
    //Switches to a specific map
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
    //Are we in that scene ?
    public bool IsScene(string sceneName) { return SceneManager.GetActiveScene().name == sceneName; }
    #endregion
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.O)) 
        {
            if (IsHost) 
            {            
                ReturnMainMenu();
                return;
            }
            if (IsClient) 
            {
                ReturnMainMenuClient();
                return;
            }
        }        
    }
}
