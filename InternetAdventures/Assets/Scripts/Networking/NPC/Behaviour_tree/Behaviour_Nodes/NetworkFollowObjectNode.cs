using UnityEngine;
using UnityEngine.AI;

public class NetworkFollowObjectNode : Node
{
    private NetworkScammerBlackboard _scammerBlackboard;
    private GameObject _objectToFollow;
    private float _distanceToObject;
    private bool _characterJumped;
    private int _placementTries;

    public NetworkFollowObjectNode(NetworkScammerBlackboard pScammerBlackboard, GameObject pObjectToFollow, float pDistanceToObject)
    {
        _scammerBlackboard = pScammerBlackboard;
        _objectToFollow = pObjectToFollow;
        _distanceToObject = pDistanceToObject;
        Debug.Log(_objectToFollow.gameObject.name);
    }
    
    public override State EvaluateState()
    {
        //Debug.Log("Evaluating State");
        Vector3 scammerToObjectVec = _objectToFollow.transform.position - _scammerBlackboard.transform.position;
        NavMeshPath navMeshPath = new NavMeshPath();
        if (scammerToObjectVec.magnitude > _distanceToObject)
        {
            //Debug.Log("Yes");
            nodeState = State.Success;
            if (_scammerBlackboard.NavAgent.CalculatePath(_objectToFollow.transform.position - scammerToObjectVec.normalized * _distanceToObject, navMeshPath)
            && (navMeshPath.corners[navMeshPath.corners.Length - 1] - _objectToFollow.transform.position).magnitude < 3.0f)
            {
                //Debug.Log("Can calculate path");
                _scammerBlackboard.NavAgent.SetPath(navMeshPath);
                _placementTries = 0;
            }
            else
            {
                //Debug.Log("No");
                if (!_objectToFollow.GetComponent<CharacterController>().isGrounded)
                {
                    //Debug.Log("Object is in the air");
                    nodeState = State.Failure;
                    _characterJumped = true;
                    _placementTries = 0;
                    return nodeState;
                }

                if (!_characterJumped)
                {
                    //Debug.Log("Character has not jumped");
                    nodeState = State.Failure;
                    return nodeState;
                }
                //Teleport
                Vector3 targetPosition;
                NavMeshHit navMeshHit;
                int itr = 0;
                do
                {
                   // Debug.Log("Sample Position");
                    targetPosition = _objectToFollow.transform.position + new Vector3(Random.Range(-2.0f, 2.0f), 
                        0, Random.Range(-2.0f, 2.0f));
                    Debug.Log(targetPosition);
                    itr++;
                } while (!NavMesh.SamplePosition(targetPosition, out navMeshHit, 3, NavMesh.AllAreas) && itr < 5);

                _placementTries++;
                if (_placementTries == 5) _characterJumped = false;
                if (itr < 5)
                {
                    _scammerBlackboard.NavAgent.enabled = false;
                    //Debug.Log("Teleporting");
                    _scammerBlackboard.transform.position = targetPosition;
                    _scammerBlackboard.NavAgent.enabled = true;   
                }
            }
        }
        else nodeState = State.Failure;

        return nodeState;
    }
}
