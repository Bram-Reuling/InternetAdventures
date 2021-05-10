using UnityEngine;

namespace GameCamera
{
    public class TestCube : MonoBehaviour
    {
        public bool goesPositive = false;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            int multiplier = 1;
            
            if (!goesPositive)
            {
                multiplier *= -1;
            }
            
            Vector3 transformPosition = gameObject.transform.position;
            
            transformPosition.x += 5 * Time.deltaTime * multiplier;

            gameObject.transform.position = transformPosition;
        }
    }
}
