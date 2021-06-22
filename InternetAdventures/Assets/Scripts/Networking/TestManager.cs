using System;
using Mirror;
using UnityEngine;

namespace Networking
{
    public class TestManager : NetworkManager
    {
        private bool SceneChanged = false;
        private string sceneChangedTo = "";
        private int sceneChangedForClients = 0;
        private bool loadedObjects = false;

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