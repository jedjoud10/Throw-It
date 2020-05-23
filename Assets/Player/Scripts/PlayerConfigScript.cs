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
    public NetworkedVarString nickname = new NetworkedVarString();
    // Use this for initialization
    void Start()
    {
        if (IsLocalPlayer) 
        {
            currentPlayerConfig = (PlayerConfig) SaverLoader.Load("playerconfig.json", new PlayerConfig(), typeof(PlayerConfig));
            InvokeServerRpc(UpdateBillboardNicknameOnServer, currentPlayerConfig.nickname, OwnerClientId);
        }
        else
        {
            UIManager.UpdatePlayerNicknameBillboard(nickname.Value);//If the nickname was already set then use it
        }
    }
    //Updates the client nickname billboard on the server
    [ServerRPC]
    private void UpdateBillboardNicknameOnServer(string _nickname, ulong clientID)
    {
        nickname.Value = _nickname;
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
