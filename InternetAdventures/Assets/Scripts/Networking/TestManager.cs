﻿using System;
using Mirror;
using Networking.Character;
using Shared.protocol.Match;
using Unity.VisualScripting;
using UnityEngine;

namespace Networking
{
    public class TestManager : NetworkManager
    {
        private bool SceneChanged = false;
        private string sceneChangedTo = "";
        private int sceneChangedForClients = 0;
        private bool loadedObjects = false;
        private int playerIndex = 0;

        [SerializeField] private Material playerOneMaterialHead;
        [SerializeField] private Material playerOneMaterialFace;
        [SerializeField] private Material playerOneMaterialBody;
        
        [SerializeField] private Material playerTwoMaterialHead;
        [SerializeField] private Material playerTwoMaterialFace;
        [SerializeField] private Material playerTwoMaterialBody;

        [SerializeField] private GameObject playerOneSpawn;
        [SerializeField] private GameObject playerTwoSpawn;

        public override void OnStartServer()
        {
            base.OnStartServer();
            
            NetworkServer.RegisterHandler<CreateCharacterMessage>(OnCreateCharacter);
        }

        private void OnCreateCharacter(NetworkConnection conn, CreateCharacterMessage message)
        {
            Debug.Log(message.skinIndex);
            GameObject playerSpawn = message.skinIndex == 0 ? playerOneSpawn : playerTwoSpawn;
            
            GameObject gameObject = Instantiate(playerPrefab, playerSpawn.transform.position, playerSpawn.transform.rotation);

            SkinnedMeshRenderer meshRenderer = gameObject.transform.GetChild(1).GetChild(0).GetComponent<SkinnedMeshRenderer>();

            if (meshRenderer == null)
            {
                Debug.Log("Mesh thingy is null");
            }
            
            if (message.skinIndex == 0)
            {
                Debug.Log("Applying skin 1");
                
                Material[] mats = new Material[] {playerOneMaterialHead, playerOneMaterialFace, playerOneMaterialBody};
                meshRenderer.materials = mats;
            }
            else
            {
                Debug.Log("Applying skin 2");
                
                Material[] mats = new Material[] {playerTwoMaterialHead, playerTwoMaterialFace, playerTwoMaterialBody};
                meshRenderer.materials = mats;
            }

            NetworkServer.AddPlayerForConnection(conn, gameObject);
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            CreateCharacterMessage createCharacterMessage = new CreateCharacterMessage {skinIndex = playerIndex};
            
            //conn.Send(createCharacterMessage);

            playerIndex++;
        }

        public override void Start()
        {
            base.Start();

            EventBroker.SceneChangeEvent += ChangeSceneOnServer;
        }

        private void ChangeSceneOnServer(string pScene)
        {
            ServerChangeScene(pScene);
            SceneChanged = true;
            sceneChangedTo = pScene;
        }

        public override void OnClientSceneChanged(NetworkConnection conn)
        {
            base.OnClientSceneChanged(conn);

            if (SceneChanged)
            {
                sceneChangedForClients++;
            }
        }

        private void Update()
        {
            if (!loadedObjects)
            {
                NetworkServer.SpawnObjects();
                loadedObjects = true;
            }
        }
    }
}