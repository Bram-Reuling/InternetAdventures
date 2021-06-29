using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PressurePlateCableHelper : MonoBehaviour
{
    [SerializeField] private GameObject pressurePlate, platform;
    [SerializeField] private GameObject cable;
    
    private void Update()
    {
        Vector3 post1ToPost2 = (platform.transform.position - new Vector3(0, 0.1f, 0)) - pressurePlate.transform.position;
        cable.transform.position = pressurePlate.transform.position + post1ToPost2 * 0.5f;
        cable.transform.rotation = Quaternion.LookRotation(post1ToPost2);
        cable.transform.localScale = new Vector3(1, 1, post1ToPost2.magnitude);
    }
}
