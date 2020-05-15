using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;
using System;
using MLAPI.Transports;
using MLAPI.Transports.Tasks;

namespace DiscordTransport
{
    public class DiscordTransport : Transport
    {
        public long clientId;
        private Discord.Discord discord;
        private NetworkManager networkManager;
        public DiscordTransportChannel[] Channels;


        public struct DiscordTransportChannel 
        {
            bool reliable;
        }

        public void InitDiscord() 
        {
           discord = new Discord.Discord(clientId, (UInt64)Discord.CreateFlags.Default);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            discord.RunCallbacks();
        }

        // LateUpdate is called at the end of each frame
        void LateUpdate()
        {
            networkManager.Flush();
        }
        public override bool IsSupported => Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.LinuxPlayer;
        public override void DisconnectLocalClient()
        {
            throw new NotImplementedException();
        }
        public override void DisconnectRemoteClient(ulong clientId)
        {
            throw new NotImplementedException();
        }
        public override ulong GetCurrentRtt(ulong clientId)
        {
            throw new NotImplementedException();
        }
        public override void Init()
        {
            throw new NotImplementedException();
        }
        public override NetEventType PollEvent(out ulong clientId, out string channelName, out ArraySegment<byte> payload, out float receiveTime)
        {
            throw new NotImplementedException();
        }
        public override void Send(ulong clientId, ArraySegment<byte> data, string channelName)
        {
            throw new NotImplementedException();
        }
        public override void Shutdown()
        {
            throw new NotImplementedException();
        }
        public override SocketTasks StartClient()
        {
            throw new NotImplementedException();
        }
        public override SocketTasks StartServer()
        {
            throw new NotImplementedException();
        }
        public override ulong ServerClientId => throw new NotImplementedException();
    }
}
