using Mirror;
using UnityEngine;

namespace Networking
{
    [ExecuteInEditMode]
    public class NetworkAnimationTransform : NetworkBehaviour
    {
        [SerializeField] private GameObject animationGameObject;

        private void Update()
        {
            transform.GetChild(0).position = animationGameObject.transform.position;
            transform.GetChild(0).rotation = animationGameObject.transform.rotation;
        }
    }
}