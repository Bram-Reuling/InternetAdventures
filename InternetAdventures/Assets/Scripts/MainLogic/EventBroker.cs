using UnityEngine;

public static class EventBroker
{
    // Custom Delegates
    public delegate void ExampleDelegate(int pValue);

    // Events
    public static event ExampleDelegate ExampleEvent;

    // Functions
    public static void CallExampleEvent(int pValue)
    {
        ExampleEvent?.Invoke(pValue);
    }
}
