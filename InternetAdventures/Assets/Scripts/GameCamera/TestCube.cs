using UnityEngine;

namespace GameCamera
{
    public class TestCube : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 transformPosition = gameObject.transform.position;
            
            //transformPosition.z += 5 * Time.deltaTime;

            gameObject.transform.position = transformPosition;
        }
    }
}
