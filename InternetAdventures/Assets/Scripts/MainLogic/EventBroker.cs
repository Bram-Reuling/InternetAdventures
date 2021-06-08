using System;
using UnityEngine;

public static class EventBroker
{
    // Custom Delegates
    public delegate void ExampleDelegate(int pValue);

    public delegate void SetCheckPointDelegate(Vector3 pPosition, string pCharacterName);

    public delegate void CharacterDelegate(string pCharacterName);

    public delegate void PanelDelegate(string pPanel);

    // Events
    public static event ExampleDelegate ExampleEvent;

    public static event SetCheckPointDelegate SetCheckPointEvent;

    public static event CharacterDelegate RespawnCharacterEvent;

    public static event PanelDelegate ChangePanelEvent;

    // Functions
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
}
