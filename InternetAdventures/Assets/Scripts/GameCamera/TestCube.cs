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
            
            if (Input.GetKey(KeyCode.D))
            {
                transformPosition.x += 5 * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.A))
            {
                transformPosition.x -= 5 * Time.deltaTime;
            }

            gameObject.transform.position = transformPosition;
        }
    }
}
