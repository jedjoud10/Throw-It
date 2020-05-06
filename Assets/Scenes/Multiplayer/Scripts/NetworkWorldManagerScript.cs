using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MLAPI.Spawning;
using MLAPI.Connection;
using MLAPI;
using System;
using MLAPI.Messaging;
//Well, a world manager, but for multiplayer. Yea, pretty epic
public class  NetworkWorldManagerScript : NetworkedBehaviour
{
    public Transform PlayerSpawnPoint;//Position where the players will spawn
    public GameObject PlayerPrefab;//Prefab of the player
    private NetworkingManager singleton;
    // Start is called before the first frame update
    void Start()
    {
        singleton = NetworkingManager.Singleton;
        //If the player isnt a server and isnt a client
        bool offline = !IsClient && !IsServer;
        if (SceneManager.GetActiveScene().name == "MultiplayerLobbyMap" && offline) //Start hosting if we are in the multiplayer lobby
        {
            singleton.StartHost(PlayerSpawnPoint.position);
        }
        if(SceneManager.GetActiveScene().name == "TestMap" && offline) 
        {
            Instantiate(PlayerPrefab, PlayerSpawnPoint.position, Quaternion.identity);
        }
    }
    //Start the host
    public void HostMultiplayerLobby() { ChangeScene("MultiplayerLobbyMap"); }
    //Join the host as a client
    public void StartClient() { singleton.StartClient(); }
    //Return to the MainMenu
    public void ReturnMainMenu() 
    {
        if (!IsServer) return;
        InvokeClientRpcOnEveryone(ReturnMainMenuClients);
        singleton.StopHost();
        ChangeScene("MainMenuMap");
    }
    //Make all clients return to main menu
    [ClientRPC]
    private void ReturnMainMenuClients() 
    {
        singleton.DisconnectClient(OwnerClientId);
        ChangeScene("MainMenuMap");
    }
    //Switches to a specific map
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && IsHost) 
        {
            ReturnMainMenu();
        }
    }
}
