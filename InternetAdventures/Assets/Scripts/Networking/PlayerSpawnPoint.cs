using UnityEngine;

namespace Networking
{
    public class PlayerSpawnPoint : MonoBehaviour
    {
        private void Awake() => PlayerSpawnSystem.AddSpawnPoint(transform);

        private void OnDestroy() => PlayerSpawnSystem.RemoveSpawnPoint(transform);
    }
}