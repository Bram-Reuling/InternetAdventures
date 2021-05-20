using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Logic : MonoBehaviour
{

    public GameObject CamTrigger, Camera;
    Vector3 ogposition;
    Quaternion ogrotation;
    bool ok = false;
    float x=0;

    // Start is called before the first frame update
    void Start()
    {
        ogposition = Camera.transform.localPosition;
        ogrotation = Camera.transform.localRotation;

    }

    // Update is called once per frame
    void Update()
    {
        
        if(ok)
        {
            Camera.transform.localPosition = Vector3.Lerp(ogposition, new Vector3(0, 7, -7), x);
            Camera.transform.localRotation = Quaternion.Lerp(ogrotation, Quaternion.Euler(40, 0, 0), x);
            x = x + 0.5f * Time.deltaTime;
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        ok = true;
    }
}
