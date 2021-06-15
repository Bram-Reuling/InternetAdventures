using UnityEngine;

[ExecuteInEditMode]
public class ZiplineCableHelper : MonoBehaviour
{
    [SerializeField] private GameObject post1, post2;
    [SerializeField] private GameObject cable;
    
    private void Update()
    {
        cable.transform.position = post1.transform.position + new Vector3(0, GetComponent<Zipline>().YOffset.y, 0);
        Vector3 post1ToPost2 = post2.transform.position - post1.transform.position;
        cable.transform.rotation = Quaternion.LookRotation(post1ToPost2);
        cable.transform.localScale = new Vector3(1, 1, post1ToPost2.magnitude);
    }
}
