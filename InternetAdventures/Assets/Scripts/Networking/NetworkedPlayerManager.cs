using System;
using System.Collections.Generic;
using System.ComponentModel;
using GameCamera;
using Shared;
using UnityEngine;

namespace Networking
{
    public class NetworkedPlayerManager : MonoBehaviour
    {
        private int ConnectionId;
        
        [SerializeField, Description("Character that the player can control.")] 
        private GameObject localCharacterPrefab;
        [SerializeField, Description("Character that the player cannot control (Other players).")] 
        private GameObject characterPrefab;

        [SerializeField] private GameObject cameraRigPrefab;

        private Dictionary<PlayerInfo, GameObject> players;

        [SerializeField] private GameObject cameraRig;
        private CameraRig cameraRigComponent;

        private void Start()
        {
            cameraRigComponent = cameraRig.GetComponent<CameraRig>();
            players = new Dictionary<PlayerInfo, GameObject>();
        }

        public void SetConnectionId(int ID)
        {
            ConnectionId = ID;
        }
        
        public void SpawnPlayer(PlayerInfo player)
        {
            // TODO: Check if the player is this client, if it is give that player control.
            // Spawn a player
            Debug.Log("Spawning player with ID: " + player.ID);
            
            Vector3 spawnPosition = new Vector3 { x = player.position.X, y =  player.position.Y, z = player.position.Z };
            
            GameObject playerGameObject = Instantiate(localCharacterPrefab, spawnPosition, Quaternion.identity).gameObject;
            
            players.Add(player, playerGameObject);
        }

        public void RemovePlayer(PlayerInfo player)
        {
            // Remove a player
            
            Destroy(players[player].gameObject);

            players.Remove(player);
        }
    }
}