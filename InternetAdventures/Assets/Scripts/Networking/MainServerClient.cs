using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Shared;
using Shared.protocol;
using UnityEngine;

public class MainServerClient : MonoBehaviour
{
    [SerializeField] private string _server = "localhost";
    [SerializeField] private int _port = 55555;

    private TcpClient _client;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        ConnectToServer();
    }

    private void ConnectToServer()
    {
        try
        {
            _client = new TcpClient();
            _client.Connect(_server, _port);
            Debug.Log("Connected to server.");
        }
        catch (Exception e)
        {
            Debug.LogError("Could not connect to server:");
            Debug.LogError(e.Message);
        }
    }

    private void Update()
    {
        try
        {
            if (_client.Available > 0)
            {
                Debug.Log("Bytes available! Reading...");
                byte[] inBytes = StreamUtil.Read(_client.GetStream());
                Packet inPacket = new Packet(inBytes);
                ISerializable inObject = inPacket.ReadObject();

                switch (inObject)
                {
                    case ClientDataRequest request:
                        Debug.Log("Received: " + inObject);
                        break;
                    default:
                        break;
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            _client.Close();
            ConnectToServer();
        }
    }
}
