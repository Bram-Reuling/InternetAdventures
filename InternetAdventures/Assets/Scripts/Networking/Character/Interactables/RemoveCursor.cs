using Mirror;
using UnityEngine;

public class RemoveCursor : NetworkBehaviour
{
    [Client]
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
