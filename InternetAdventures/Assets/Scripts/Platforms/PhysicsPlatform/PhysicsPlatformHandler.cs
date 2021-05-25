using DG.Tweening;
using UnityEngine;

public class PhysicsPlatformHandler : MonoBehaviour
{
    [SerializeField] private GameObject platform1, platform2;
    [SerializeField] private bool movePlatformsBack;
    [SerializeField] private float movementDuration;
    private GameObject _actuatedPlatform;
    private float _platformActuatorMass;

    private GameObject _higherPlatform;

    //Platform positions
    private Vector3 _platform1InitPosition;
    private Vector3 _platform2InitPosition;
    private Vector3 _platform1Actuated;
    private Vector3 _platform2Actuated;
    
    private void Start()
    {
        //Save initial position to check difference when moving back.
        _platform1InitPosition = platform1.transform.position;
        _platform2InitPosition = platform2.transform.position;
        
        //Calculate difference vector
        Vector3 differenceVector = _platform2InitPosition - _platform1InitPosition;
        _platform1Actuated = _platform1InitPosition + new Vector3(0, differenceVector.y, 0);
        _platform2Actuated = _platform2InitPosition - new Vector3(0, differenceVector.y, 0);
        
        _higherPlatform = _platform1InitPosition.y > _platform2InitPosition.y ? platform1 : platform2;
    }
    
    
    public void OnActuation()
    {
        float platform1Mass = platform1.transform.GetChild(0).GetComponent<PhysicsPlatform>().GetTotalMass();
        float platform2Mass = platform2.transform.GetChild(0).GetComponent<PhysicsPlatform>().GetTotalMass();

        //Debug.Log("Platform 1 mass was " + platform1Mass);
        Debug.Log("Platform 2 mass was " + platform2Mass);
        
        if (platform1Mass <= 0 && platform2Mass <= 0)
        {
            StopActuation();
            return;
        }

        GameObject platformToActuate = platform1Mass < platform2Mass ? platform2 : platform1;
        
        platform1.GetComponent<Rigidbody>().DOMoveY(platformToActuate == _higherPlatform ? _platform1Actuated.y : _platform1InitPosition.y, movementDuration);
        platform2.GetComponent<Rigidbody>().DOMoveY(platformToActuate == _higherPlatform ? _platform2Actuated.y : _platform2InitPosition.y, movementDuration);
    }

    public void StopActuation()
    { 
        
        if (movePlatformsBack)
            MovePlatformsBack();
        _actuatedPlatform = null;
    }
    
    private void MovePlatformsBack()
    {
        platform1.GetComponent<Rigidbody>().DOMoveY(_platform1InitPosition.y, movementDuration);
        platform2.GetComponent<Rigidbody>().DOMoveY(_platform2InitPosition.y, movementDuration);
    }
}
