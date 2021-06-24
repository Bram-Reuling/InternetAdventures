using System;
using Shared.protocol.Lobby;
using UnityEngine;

public static class EventBroker
{
    // Custom Delegates
    public delegate void ChangeFmodParamDelegate(int pValue);

    public delegate void SetCheckPointDelegate(Vector3 pPosition, string pCharacterName);

    public delegate void LobbyDataDelegate(LobbyDataResponse pLobby, int thisClientId);

    public delegate void StringDelegate(string pValue);

    public delegate void IntStringDelegate(int pIntValue, string pStringValue);

    // Events
    public static event ChangeFmodParamDelegate ChangeFmodParamEvent;

    public static event SetCheckPointDelegate SetCheckPointEvent;

    public static event StringDelegate RespawnCharacterEvent;

    public static event StringDelegate ChangePanelEvent;

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

    public static event Action MatchEndEvent;

    public static event StringDelegate SceneChangeEvent;

    public static event StringDelegate LoseWinEvent;

    public static event IntStringDelegate ChangeMembersCount;

    // Functions

    public static void CallMatchEndEvent()
    {
        MatchEndEvent?.Invoke();
    }
    
    public static void CallChangeMembersCount(int pIntValue, string pStringValue)
    {
        ChangeMembersCount?.Invoke(pIntValue, pStringValue);
    }
    
    public static void CallLoseWinEvent(string pValue)
    {
        LoseWinEvent?.Invoke(pValue);
    }
    
    public static void CallSceneChangeEvent(string pValue)
    {
        SceneChangeEvent?.Invoke(pValue);
    }
    
    public static void CallFmodParamEvent(int pValue)
    {
        ChangeFmodParamEvent?.Invoke(pValue);
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
