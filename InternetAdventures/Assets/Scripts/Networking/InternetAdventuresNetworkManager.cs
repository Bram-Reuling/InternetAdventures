using System;
using System.Collections.Generic;
using Mirror;
using Networking.Character;
using Networking.Lobby;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Networking
{
    public class InternetAdventuresNetworkManager : NetworkManager
    {
        [SerializeField] private int minPlayers = 2;
        [Scene] [SerializeField] private string menuScene = string.Empty;
        [Scene] [SerializeField] private string gameScene = string.Empty;

        [Header("Room")] 
        [SerializeField] private NetworkRoomPlayerIndAd roomPlayerPrefab = null;
        
        [Header("Game")]
        [SerializeField] private NetworkGamePlayerIntAd gamePlayerPrefab = null;

        [SerializeField] private GameObject playerSpawnSystem = null;

        public static event Action OnClientConnected;
        public static event Action OnClientDisconnected;
        public static event Action<NetworkConnection> OnServerReadied; 

        public List<NetworkRoomPlayerIndAd> RoomPlayers { get; } = new List<NetworkRoomPlayerIndAd>();
        public List<NetworkGamePlayerIntAd> GamePlayers { get; } = new List<NetworkGamePlayerIntAd>();

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            OnClientConnected?.Invoke();
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            OnClientDisconnected?.Invoke();
        }

        public override void OnServerConnect(NetworkConnection conn)
        {
            if (numPlayers >= maxConnections)
            {
                conn.Disconnect();
                return;
            }

            if (SceneManager.GetActiveScene().path == menuScene) return;
            conn.Disconnect();
        }

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            if (SceneManager.GetActiveScene().path == menuScene)
            {
                bool isLeader = RoomPlayers.Count == 0;
                
                Debug.Log($"IsLeader: {isLeader}");
                
                NetworkRoomPlayerIndAd roomPlayerInstance = Instantiate(roomPlayerPrefab);

                NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
                
                roomPlayerInstance.IsLeader = isLeader;

                Debug.Log($"Players: {RoomPlayers.Count}");
            }
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            if (conn.identity != null)
            {
                var player = conn.identity.GetComponent<NetworkRoomPlayerIndAd>();
                RoomPlayers.Remove(player);

                NotifyPlayersOfReadyState();
            }
            base.OnServerDisconnect(conn);
        }

        public override void OnStopServer()
        {
            RoomPlayers.Clear();
        }

        public void NotifyPlayersOfReadyState()
        {
            foreach (var player in RoomPlayers)
            {
                player.HandleReadyToStart(IsReadyToStart());
            }
        }

        private bool IsReadyToStart()
        {
            if (numPlayers < minPlayers)
            {
                return false;
            }

            foreach (var player in RoomPlayers)
            {
                if (!player.IsReady)
                {
                    return false;
                }
            }

            return true;
        }

        public void StartGame()
        {
            if (SceneManager.GetActiveScene().path == menuScene)
            {
                if (!IsReadyToStart()) {return;}
                
                ServerChangeScene(gameScene);
            }
        }

        public override void ServerChangeScene(string newSceneName)
        {
            if (SceneManager.GetActiveScene().path == menuScene)
            {
                for (int i = RoomPlayers.Count - 1; i >= 0; i--)
                {
                    var conn = RoomPlayers[i].connectionToClient;
                    var gamePlayerInstance = Instantiate(gamePlayerPrefab);

                    gamePlayerInstance.SetDisplayName(RoomPlayers[i].DisplayName);
                    
                    NetworkServer.Destroy(conn.identity.gameObject);
                    NetworkServer.ReplacePlayerForConnection(conn, gamePlayerInstance.gameObject);
                }    
            }
            
            base.ServerChangeScene(newSceneName);
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            if (sceneName.Equals(gameScene))
            {
                GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
                NetworkServer.Spawn(playerSpawnSystemInstance);
            }
        }

        public override void OnServerReady(NetworkConnection conn)
        {
            base.OnServerReady(conn);
            
            OnServerReadied?.Invoke(conn);
        }
    }
}