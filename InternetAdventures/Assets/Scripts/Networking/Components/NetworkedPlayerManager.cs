using System.Collections.Generic;
using System.ComponentModel;
using GameCamera;
using UnityEngine;
using UnityEngine.InputSystem;

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

        private Dictionary<int, GameObject> players;

        [SerializeField] private GameObject cameraRig;
        private CameraRig cameraRigComponent;

        private void Start()
        {
            cameraRigComponent = cameraRig.GetComponent<CameraRig>();
            cameraRigComponent.setTargetExternally = true;
            players = new Dictionary<int, GameObject>();
        }

        public void SetConnectionId(int ID)
        {
            ConnectionId = ID;
        }
        
        public void SpawnPlayer(PlayerInfo player)
        {
            Debug.Log("Spawning player with ID: " + player.ID);
            GameObject playerGameObject;
            Vector3 spawnPosition = new Vector3 { x = player.position.x, y =  player.position.y, z = player.position.z };

            if (player.ID == ConnectionId)
            {
                Debug.Log("Has Control: Yes");
                playerGameObject = PlayerInput.Instantiate(localCharacterPrefab, controlScheme: "CharacterWASD",
                    pairWithDevice: Keyboard.current).gameObject;
                playerGameObject.transform.position = spawnPosition;

                cameraRigComponent.Target = playerGameObject;
            }
            else
            {
                Debug.Log("Has Control: No");
                playerGameObject = Instantiate(characterPrefab, spawnPosition, Quaternion.identity).gameObject;
            }

            playerGameObject.AddComponent<NetworkPlayerInformation>().playerId = player.ID;

            players.Add(player.ID, playerGameObject);
        }

        public void RemovePlayer(PlayerInfo player)
        {
            Debug.Log("Trying to remove player");
            if (players.ContainsKey(player.ID))
            {
                Debug.Log("Removing player");
                // Remove a player
                GameObject playerToRemove = players[player.ID].gameObject;

                players.Remove(player.ID);
            
                Destroy(playerToRemove);   
            }
            else
            {
                Debug.Log("No such player");
            }
        }

        public void MovePlayer(PlayerInfo player)
        {
            players[player.ID].transform.SetPositionAndRotation(player.position, player.rotation);
        }
    }
}