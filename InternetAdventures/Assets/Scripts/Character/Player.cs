using UnityEngine;

namespace Character
{
    public class Player : MonoBehaviour
    {
        private Vector3 checkPoint;
        
        public void SetCheckPoint(Vector3 pPosition)
        {
            Debug.Log("Set Check Point to: " + pPosition.ToString());
            checkPoint = pPosition;
        }

        public void RespawnPlayer()
        {
            Debug.Log("Respawning to: " + checkPoint.ToString());
            gameObject.transform.position = checkPoint;
        }
    }
}