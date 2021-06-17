using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mirror;
using Networking;
using UnityEngine;

public class NetworkInteractableManager : NetworkBehaviour
{
    [SerializeField] private GameObject banHammer;
    [SerializeField] private GameObject hands;
    [SerializeField] private GameObject gravityGun;
    [SerializeField] private GameObject shockwaveGun;

    private NetworkBanHammer banHammerComponent;
    private NetworkHands handsComponent;
    private NetworkGravityGun gravityGunComponent;
    private NetworkShockwaveGun shockwaveGunComponent;

    private NetworkCharacterMovement characterMovement;

    private void Start()
    {
        banHammerComponent = banHammer.GetComponent<NetworkBanHammer>();
        handsComponent = hands.GetComponent<NetworkHands>();
        gravityGunComponent = gravityGun.GetComponent<NetworkGravityGun>();
        shockwaveGunComponent = shockwaveGun.GetComponent<NetworkShockwaveGun>();

        characterMovement = GetComponent<NetworkCharacterMovement>();
    }

    #region Ban Hammer

    // CLIENT
    [ClientRpc]
    private void RpcSetCapsuleColliderProperties(GameObject gameObject)
    {
        gameObject.transform.GetComponent<CapsuleCollider>().height = 0.1f;
        gameObject.transform.GetComponent<CapsuleCollider>().radius = 0.1f;
    }

    // SERVER
    [Command]
    public void CmdSlamHammer(List<GameObject> _gameObjectsInTrigger, bool enableScaleEffectOnObjects,
        Vector3 _initialScale, float animationTimer)
    {
        StartCoroutine(SlamHammer(_gameObjectsInTrigger, enableScaleEffectOnObjects, _initialScale, animationTimer));
    }

    [ServerCallback]
    private IEnumerator SlamHammer(List<GameObject> _gameObjectsInTrigger, bool enableScaleEffectOnObjects,
        Vector3 _initialScale, float animationTimer)
    {
        banHammerComponent.PlayAnimation();
        banHammerComponent.SetCoroutineRunning(true);
        yield return new WaitForSeconds(animationTimer);

        RpcShakeCameraBanHammer();
        
        Debug.Log("Slam Hammer");

        foreach (var gameObjectInReach in _gameObjectsInTrigger)
        {
            //IMPORTANT
            //Todo: This line is subject to change depending on what the designers want

            if (!enableScaleEffectOnObjects)
            {
                //Info: Scale object down.
                gameObjectInReach.transform.localScale -=
                    new Vector3(0, gameObjectInReach.transform.localScale.y - 0.2f, 0);
                //Info: Send ray to ground and place object there.
                if (Physics.Raycast(gameObjectInReach.transform.position, Vector3.down, out var raycastHit,
                    float.PositiveInfinity))
                {
                    gameObjectInReach.transform.position = raycastHit.point +
                                                           new Vector3(0, gameObjectInReach.transform.lossyScale.y / 2,
                                                               0);
                }
            }
            else
            {
                gameObjectInReach.transform.DOKill();
                gameObjectInReach.transform.localScale = _initialScale;
                //gameObjectInReach.transform.DOShakeScale(1, 1.0f);
                gameObjectInReach.transform.DOScaleY(0.1f, 1);
                gameObjectInReach.transform.GetComponent<CapsuleCollider>().height = 0.1f;
                gameObjectInReach.transform.GetComponent<CapsuleCollider>().radius = 0.1f;

                RpcSetCapsuleColliderProperties(gameObjectInReach);
            }

            if (gameObjectInReach.CompareTag("AI"))
            {
                Destroy(gameObjectInReach.GetComponent<GoodMemberBlackboard>());
                Destroy(gameObjectInReach.GetComponent<BadMemberBlackboard>());
                gameObjectInReach.transform.GetChild(1).GetComponent<Animator>().enabled = false;
                LoseWinHandler.RemoveFromList(gameObjectInReach);
                
                //Reset tag and layer so this 'smashed' AI will not be further considered by other AIs.
                //This is especially important since the blackboard component is getting removed and will result in an 
                //exception otherwise.
                gameObjectInReach.tag = "Untagged";
                gameObjectInReach.layer = new int();
            }
            
            //Add impulse upwards if there's a rigidbody.
            Rigidbody rigidbody = gameObjectInReach.GetComponent<Rigidbody>();
            if (rigidbody != null)
                rigidbody.AddForce(Vector3.up * 5.0f, ForceMode.Impulse);
        }

        yield return null;
        yield return new WaitWhile(() => banHammerComponent.AnimatorStateIsInteractable());
        banHammerComponent.SetCoroutineRunning(false);
    }

    [TargetRpc]
    private void RpcPlayAnimation()
    {
        banHammerComponent.PlayAnimation();
    }
    
    [TargetRpc]
    private void RpcShakeCameraBanHammer()
    {
        banHammerComponent.CameraShake();
    }

    #endregion

    #region Hands

    [Command]
    public void CmdGrabObjectInFront(List<GameObject> _gameObjectsInTrigger, GameObject _grabbedObject)
    {
        float shortestDistanceGameObject = float.PositiveInfinity;
        foreach (var currentGameObject in _gameObjectsInTrigger)
        {
            if ((currentGameObject.transform.position - transform.GetChild(0).transform.position).magnitude <
                shortestDistanceGameObject)
            {
                _grabbedObject = currentGameObject;
                handsComponent.SetGrabbedObject(_grabbedObject);
                RpcSetGrabbedObject(_grabbedObject);
            }
        }

        if (_grabbedObject == null) return;

        var parent = _grabbedObject.transform.parent;
        handsComponent.SetInitialParent(parent);
        RpcSetInitialParent(parent);

        handsComponent.SetGrabbedObjectParent(transform);

        handsComponent.SetGrabbedObjectConstraints(RigidbodyConstraints.FreezeAll);

        // Set the grabbed object parent on the client
        RpcSetGrabbedObjectParamsClient(transform, RigidbodyConstraints.FreezeAll);
    }

    [ClientRpc]
    private void RpcSetGrabbedObject(GameObject pObject)
    {
        handsComponent.SetGrabbedObject(pObject);
    }

    [ClientRpc]
    private void RpcSetInitialParent(Transform pParent)
    {
        handsComponent.SetInitialParent(pParent);
    }

    [ClientRpc]
    private void RpcSetGrabbedObjectParamsClient(Transform pTransform, RigidbodyConstraints pConstraints)
    {
        handsComponent.SetGrabbedObjectParent(pTransform);
        handsComponent.SetGrabbedObjectConstraints(pConstraints);
    }

    [Command]
    public void CmdReleaseObject(Transform _initialParent)
    {
        handsComponent.SetGrabbedObjectParent(_initialParent);
        handsComponent.SetGrabbedObjectConstraints(RigidbodyConstraints.None);
        RpcSetGrabbedObjectParamsClient(_initialParent, RigidbodyConstraints.None);

        handsComponent.AddForceToGrabbedRigidbody(50);
        RpcAddForceToGrabbedRigidbody(50);

        handsComponent.SetGrabbedObject(null);
        RpcSetGrabbedObject(null);
    }

    [ClientRpc]
    private void RpcAddForceToGrabbedRigidbody(float pValue)
    {
        handsComponent.AddForceToGrabbedRigidbody(pValue);
    }

    #endregion

    #region Gravity Gun

    [Command]
    public void CmdActivateGravityGun(float gravityRadius, float range, int interactableLayers,
        float _furthestDistanceToObject)
    {
        RaycastHit[] _overlappedColliders = new RaycastHit[50];

        int foundColliders = Physics.SphereCastNonAlloc(gravityGun.transform.position, gravityRadius,
            gravityGun.transform.forward, _overlappedColliders, range, interactableLayers);
        if (foundColliders > 0)
        {
            for (int i = 0; i < foundColliders; i++)
            {
                GameObject intersectingGameObject = _overlappedColliders[i].collider.gameObject;
                Rigidbody currentRigidbody = intersectingGameObject.GetComponent<Rigidbody>();
                float currentDistance =
                    (intersectingGameObject.transform.position - transform.GetChild(0).transform.position)
                    .magnitude;

                Debug.LogWarning($"Current Distance: {currentDistance}");

                if (currentDistance > _furthestDistanceToObject) _furthestDistanceToObject = currentDistance;

                gravityGunComponent.SetFurthestDistanceToObject(_furthestDistanceToObject);
                RpcSetFurthestDistanceToObject(_furthestDistanceToObject);

                NetworkItemInformation item = new NetworkItemInformation(intersectingGameObject,
                    intersectingGameObject.transform.parent, currentRigidbody.constraints, currentDistance);

                //intersectingGameObject.transform.SetParent(transform);
                currentRigidbody.useGravity = false;
                currentRigidbody.constraints = RigidbodyConstraints.FreezeAll;

                gravityGunComponent.AddItemToPickedUpList(item, transform);
                RpcAddItemToClientPickedUpList(item);
                characterMovement.weaponInUse = true;
                RpcSetWeaponInUse(true);
            }
        }
    }

    [ClientRpc]
    private void RpcSetFurthestDistanceToObject(float pValue)
    {
        gravityGunComponent.SetFurthestDistanceToObject(pValue);
    }

    [ClientRpc]
    private void RpcAddItemToClientPickedUpList(NetworkItemInformation item)
    {
        gravityGunComponent.AddItemToPickedUpList(item, transform);
    }

    [ClientRpc]
    private void RpcSetWeaponInUse(bool pValue)
    {
        characterMovement.weaponInUse = pValue;
    }

    [Command]
    public void CmdChangeAttractionDistance(float _currentAttractionDistance, float _furthestDistanceToObject,
        float yValue)
    {
        if (_currentAttractionDistance <= -_furthestDistanceToObject && yValue < 0)
            return;
        gravityGunComponent.ChangeDistance(yValue);
        RpcChangeDistance(yValue);
    }

    [ClientRpc]
    private void RpcChangeDistance(float pValue)
    {
        gravityGunComponent.ChangeDistance(pValue);
    }

    [Command]
    public void CmdDeactivateGravityGun(List<NetworkItemInformation> _pickedUpObjects)
    {
        //Sets parent to null again and clears list.
        foreach (var pickedObject in _pickedUpObjects)
        {
            //pickedObject.CurrentGameObject.transform.SetParent(pickedObject.Parent);

            gravityGunComponent.ResetObjectParent(pickedObject);
            RpcResetObjectParent(pickedObject);

            Rigidbody currentRigidbody = pickedObject.CurrentGameObject.GetComponent<Rigidbody>();
            currentRigidbody.constraints = pickedObject.RigidbodyConstraints;
            currentRigidbody.useGravity = true;
        }

        gravityGunComponent.ClearObjectList();
        RpcClearObjectList();
        characterMovement.weaponInUse = false;
        RpcSetWeaponInUse(false);
        gravityGunComponent.ResetDistance();
        RpcResetDistance();
    }

    [ClientRpc]
    private void RpcClearObjectList()
    {
        gravityGunComponent.ClearObjectList();
    }

    [ClientRpc]
    private void RpcResetObjectParent(NetworkItemInformation pickedUpObject)
    {
        gravityGunComponent.ResetObjectParent(pickedUpObject);
    }

    [ClientRpc]
    private void RpcResetDistance()
    {
        gravityGunComponent.ResetDistance();
    }

    [ServerCallback]
    private void Update()
    {
        //Move objects towards player only if there's at least one.
        List<NetworkItemInformation> items = gravityGunComponent.GetItems();

        if (items.Count > 0)
        {
            foreach (var pickedObject in items)
            {
                GameObject currentGameObject = pickedObject.CurrentGameObject;
                Vector3 movementDirection =
                    currentGameObject.transform.position - transform.GetChild(0).transform.position;
                Debug.LogWarning($"Current Distance: {movementDirection}");
                float goalDistance = pickedObject.InitialDistance + gravityGunComponent.GetCurrentDistance();
                float deltaDistance = goalDistance - movementDirection.magnitude;
                if (movementDirection.magnitude +
                    gravityGunComponent.GetAttractionSpeed() * deltaDistance * Time.deltaTime <
                    gravityGunComponent.GetClosestDistance()) continue;
                if (Mathf.Abs(deltaDistance) > 0.1f)
                    currentGameObject.transform.Translate(
                        gravityGunComponent.GetAttractionSpeed() * deltaDistance * Time.deltaTime *
                        movementDirection.normalized, Space.World);
            }
        }
    }

    #endregion

    #region Shockwave Gun

    [Command]
    public void CmdShootShockwaveGun(float range, float shockwaveRadius, float shockwaveStrength,
        float possibleHitRadius, int interactableLayers)
    {
        RpcPlayEffect();
        
        RaycastHit[] overlapColliders =
            Physics.SphereCastAll(transform.position, possibleHitRadius, transform.forward, range, interactableLayers);

        if (overlapColliders.Length <= 0) return;
        Vector3 hitPosition = overlapColliders[0].point;
        Collider[] collidersInRange = Physics.OverlapSphere(hitPosition, shockwaveRadius);
        foreach (var collider in collidersInRange)
        {
            if (collider.TryGetComponent(typeof(Rigidbody), out var rigidbody))
                ((Rigidbody) rigidbody).AddExplosionForce(shockwaveStrength, hitPosition, shockwaveRadius);
        }
    }

    [TargetRpc]
    private void RpcPlayEffect()
    {
        shockwaveGunComponent.PlayEffect();
    }

    #endregion
}