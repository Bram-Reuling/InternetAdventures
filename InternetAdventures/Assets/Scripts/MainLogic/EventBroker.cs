using UnityEngine;

public class EventBroker : MonoBehaviour
{
    // Custom Delegates
    public delegate void ExampleDelegate(int pValue);

    // Events
    public static event ExampleDelegate ExampleEvent;

    // Functions
    private static void CallExampleEvent(int pValue)
    {
        ExampleEvent?.Invoke(pValue);
    }
}
