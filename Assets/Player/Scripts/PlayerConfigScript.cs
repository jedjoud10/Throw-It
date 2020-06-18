using UnityEngine;
using System.Collections;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkedVar;
//Handles player config like username
public class PlayerConfigScript : NetworkedBehaviour
{
    public PlayerUIManagerScript UIManager;
    private PlayerConfig currentPlayerConfig;
    [HideInInspector]
    public NetworkedVarString nickname = new NetworkedVarString(new NetworkedVarSettings() { WritePermission = NetworkedVarPermission.OwnerOnly, ReadPermission = NetworkedVarPermission.Everyone });
    // Use this for initialization
    void Start()
    {
        if (IsLocalPlayer) 
        {
            currentPlayerConfig = (PlayerConfig) SaverLoader.Load("playerconfig.json", new PlayerConfig(), typeof(PlayerConfig));

            nickname.Value = currentPlayerConfig.nickname;
            InvokeServerRpc(InitPlayerOnServer, currentPlayerConfig.nickname, OwnerClientId, gameObject);
            FindObjectOfType<NetworkWorldManagerScript>().playerConfigScript = this;
        }
        else
        {
            UIManager.UpdatePlayerNicknameBillboard(nickname.Value);//If the nickname was already set then use it
        }
    }
    //Updates the player data on the server
    [ServerRPC]
    private void InitPlayerOnServer(string _nickname, ulong clientID, GameObject playerObject)
    {
        FindObjectOfType<NetworkWorldManagerScript>().RegisterPlayer(_nickname, clientID, playerObject);
        InvokeClientRpcOnEveryoneExcept(UpdateBillboardNicknameOnClients, clientID, _nickname);//Ignore the local client that told us this nickname because they cannot see their nickname anyways
    }
    //Updates the client nickname billboard on the clients
    [ClientRPC]
    private void UpdateBillboardNicknameOnClients(string _nickname) 
    {
        UIManager.UpdatePlayerNicknameBillboard(_nickname);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
//Saves data about the player like their nickname
public class PlayerConfig 
{
    public string nickname = "DefaultUser";
}
