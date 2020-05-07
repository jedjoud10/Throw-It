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
    private WorldManager worldManager;//The world manager
    private bool singleplayer;//Is the game in singleplayer ?
    // Start is called before the first frame update
    void Start()
    {
        worldManager = FindObjectOfType<WorldManager>();
        singleton = NetworkingManager.Singleton;
        singleplayer = !singleton.IsHost && !singleton.IsClient;
        //If the player isnt a server and isnt a client
        if (singleplayer)
        {
            if (worldManager.IsScene("MultiplayerLobbyMap")) //Start hosting if we are in the multiplayer lobby
            {
                PlayerSpawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawnPoint").transform;
                singleton.StartHost(PlayerSpawnPoint.position);//Spawn host in soon to be multiplayer session                
            }
            else if(!worldManager.IsScene("MainMenuMap"))
            {
                PlayerSpawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawnPoint").transform;
                singleton.StartHost(PlayerSpawnPoint.position);//Spawn host in singleplayer
            }
        }
    }
    #region Scene management
    //Start the host
    public void HostMultiplayerLobby() { worldManager.ChangeScene("MultiplayerLobbyMap"); }
    //Return to the MainMenu
    public void ReturnMainMenu() 
    {
        if (!IsServer) return;
        InvokeClientRpcOnEveryoneExcept(ReturnMainMenuClient, OwnerClientId);
        singleton.StopHost();
        worldManager.ChangeScene("MainMenuMap");
    }
    //Make all clients return to main menu
    [ClientRPC]
    private void ReturnMainMenuClient() 
    {
        singleton.StopClient();
        worldManager.ChangeScene("MainMenuMap");
        Debug.Log("Client return to main menu");
    }

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
