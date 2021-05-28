using System.Collections.Generic;
using DG.Tweening;
using Mirror;
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

    private void Start()
    {
        banHammerComponent = banHammer.GetComponent<NetworkBanHammer>();
        handsComponent = hands.GetComponent<NetworkHands>();
        gravityGunComponent = gravityGun.GetComponent<NetworkGravityGun>();
        shockwaveGunComponent = shockwaveGun.GetComponent<NetworkShockwaveGun>();
    }

    #region Ban Hammer

    // CLIENT
    [ClientRpc]
    public void RpcSetCapsuleColliderProperties(GameObject gameObject)
    {
        gameObject.transform.GetComponent<CapsuleCollider>().height = 0.1f;
        gameObject.transform.GetComponent<CapsuleCollider>().radius = 0.1f;
    }
    
    // SERVER
    [Command]
    public void CmdSlamHammer(List<GameObject> _gameObjectsInTrigger, bool enableScaleEffectOnObjects, Vector3 _initialScale)
    {
        foreach (var gameObjectInReach in _gameObjectsInTrigger)
        {
            //IMPORTANT
            //Todo: This line is subject to change depending on what the designers want
            
            if (!enableScaleEffectOnObjects)
            {
                //Info: Scale object down.
                gameObjectInReach.transform.localScale -= new Vector3(0, gameObjectInReach.transform.localScale.y - 0.2f, 0);
                //Info: Send ray to ground and place object there.
                if (Physics.Raycast(gameObjectInReach.transform.position, Vector3.down, out var raycastHit,
                    float.PositiveInfinity))
                {
                    gameObjectInReach.transform.position = raycastHit.point + new Vector3(0,gameObjectInReach.transform.lossyScale.y / 2,0);
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
            
            //Add impulse upwards if there's a rigidbody.
            Rigidbody rigidbody = gameObjectInReach.GetComponent<Rigidbody>();
            if (rigidbody != null)
                rigidbody.AddForce(Vector3.up * 5.0f, ForceMode.Impulse);
        }
    }

    #endregion

    #region Hands

    

    #endregion

    #region Gravity Gun

    

    #endregion

    #region Shockwave Gun

    [Command]
    public void CmdShootShockwaveGun(float range, float shockwaveRadius, float shockwaveStrength, float possibleHitRadius, int interactableLayers)
    {
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

    #endregion
}