using UnityEngine;
using DG.Tweening;
using Mirror;
using UnityEngine.InputSystem;

public abstract class NetworkInteractable : Interactable
{
    [Header("Generic network-interactable attributes")] [SerializeField]
    protected NetworkInteractableManager networkInteractableManager;
}
