using System.Collections;
using System.Collections.Generic;
using GameCamera;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public GameObject characterPrefab;
    public GameObject cameraRigPrefab;

    void Start()
    {
        GameObject character1 = PlayerInput.Instantiate(characterPrefab, controlScheme: "CharacterWASD", pairWithDevice: Keyboard.current).gameObject;
        GameObject cameraRigPlayer1 = GameObject.Instantiate(cameraRigPrefab);
        cameraRigPlayer1.GetComponent<CameraRig>().Target = character1;
        
        GameObject character2 = PlayerInput.Instantiate(characterPrefab, controlScheme: "CharacterArrows", pairWithDevice: Keyboard.current).gameObject;
        character2.transform.position += new Vector3(2, 0, 0);
    }
}
