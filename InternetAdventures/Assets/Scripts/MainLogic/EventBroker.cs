using System;
using Shared.protocol.Lobby;
using UnityEngine;

public static class EventBroker
{
    // Custom Delegates
    public delegate void ExampleDelegate(int pValue);

    public delegate void SetCheckPointDelegate(Vector3 pPosition, string pCharacterName);

    public delegate void CharacterDelegate(string pCharacterName);

    public delegate void SceneChangeDelegate(string pSceneName);

    public delegate void PanelDelegate(string pPanel);

    public delegate void LobbyDataDelegate(LobbyDataResponse pLobby, int thisClientId);

    // Events
    public static event ExampleDelegate ExampleEvent;

    public static event SetCheckPointDelegate SetCheckPointEvent;

    public static event CharacterDelegate RespawnCharacterEvent;

    public static event PanelDelegate ChangePanelEvent;

    public static event Action LoadedLobbyPanelEvent;

    public static event LobbyDataDelegate UpdateLobbyDataEvent;

    public static event Action PlayerEnterMatchEndZoneEvent;
    public static event Action PlayerExitMatchEndZoneEvent;

    public static event Action ConnectToServerEvent;
    public static event Action JoinLobbyEvent;
    public static event Action HostLobbyEvent;
    public static event Action ReadyEvent;
    public static event Action LeaveEvent;
    public static event Action StartMatchEvent;

    public static event SceneChangeDelegate SceneChangeEvent;

    // Functions

    public static void CallSceneChangeEvent(string pValue)
    {
        SceneChangeEvent?.Invoke(pValue);
    }
    
    public static void CallExampleEvent(int pValue)
    {
        ExampleEvent?.Invoke(pValue);
    }

    public static void CallSetCheckPointEvent(Vector3 pPosition, string pCharacterName)
    {
        SetCheckPointEvent?.Invoke(pPosition, pCharacterName);
    }

    public static void CallRespawnCharacterEvent(string pCharacterName)
    {
        RespawnCharacterEvent?.Invoke(pCharacterName);
    }

    public static void CallChangePanelEvent(string pPanel)
    {
        ChangePanelEvent?.Invoke(pPanel);
    }

    public static void CallLoadedLobbyPanelEvent()
    {
        LoadedLobbyPanelEvent?.Invoke();
    }

    public static void CallUpdateLobbyDataEvent(LobbyDataResponse response, int thisClientId)
    {
        UpdateLobbyDataEvent?.Invoke(response, thisClientId);
    }

    public static void CallPlayerEnterMatchEndZoneEvent()
    {
        PlayerEnterMatchEndZoneEvent?.Invoke();
    }

    public static void CallPlayerExitMatchEndZoneEvent()
    {
        PlayerExitMatchEndZoneEvent?.Invoke();
    }

    public static void CallConnectToServerEvent()
    {
        ConnectToServerEvent?.Invoke();
    }

    public static void CallJoinLobbyEvent()
    {
        JoinLobbyEvent?.Invoke();
    }

    public static void CallHostLobbyEvent()
    {
        HostLobbyEvent?.Invoke();
    }

    public static void CallReadyEvent()
    {
        ReadyEvent?.Invoke();
    }

    public static void CallLeaveEvent()
    {
        LeaveEvent?.Invoke();
    }

    public static void CallStartMatchEvent()
    {
        StartMatchEvent?.Invoke();
    }
}
