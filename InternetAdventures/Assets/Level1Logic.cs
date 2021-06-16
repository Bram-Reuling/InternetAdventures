using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Logic : MonoBehaviour
{

    public GameObject CamTrigger, Camera;
    Vector3 ogPosition;
    Quaternion ogRotation;
    bool isOk = false;
    float x=0;

    // Start is called before the first frame update
    void Start()
    {
        ogPosition = Camera.transform.localPosition;
        ogRotation = Camera.transform.localRotation;

    }

    // Update is called once per frame
    void Update()
    {
        
        if(isOk)
        {
            Camera.transform.localPosition = Vector3.Lerp(ogPosition, new Vector3(0, 7, -7), x);
            Camera.transform.localRotation = Quaternion.Lerp(ogRotation, Quaternion.Euler(40, 0, 0), x);
            x += 0.5f * Time.deltaTime;
        }

        if (x >= 5f)
            isOk = false;


    }

    private void OnTriggerEnter(Collider other)
    {
        isOk = true;
    }
}
