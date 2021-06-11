using UnityEngine;
using UnityEngine.AI;

public class FollowObjectNode : Node
{
    private ScammerBlackboard _scammerBlackboard;
    private GameObject _objectToFollow;
    private float _distanceToObject;

    public FollowObjectNode(ScammerBlackboard pScammerBlackboard, GameObject pObjectToFollow, float pDistanceToObject)
    {
        _scammerBlackboard = pScammerBlackboard;
        _objectToFollow = pObjectToFollow;
        _distanceToObject = pDistanceToObject;
    }
    
    public override State EvaluateState()
    {
        Vector3 scammerToObjectVec = _objectToFollow.transform.position - _scammerBlackboard.transform.position;
        NavMeshPath navMeshPath = new NavMeshPath();
        if (scammerToObjectVec.magnitude > _distanceToObject)
        {
            nodeState = State.Success;
            if (_scammerBlackboard.NavAgent.CalculatePath(
                _objectToFollow.transform.position - scammerToObjectVec.normalized * _distanceToObject, navMeshPath))
            {
                _scammerBlackboard.NavAgent.SetPath(navMeshPath);
            }
            else
            {
                if (!_objectToFollow.GetComponent<CharacterController>().isGrounded)
                {
                    nodeState = State.Failure;
                    return nodeState;
                }
                //Teleport
                Vector3 targetPosition;
                NavMeshHit navMeshHit;
                int itr = 0;
                do
                {
                    targetPosition = _objectToFollow.transform.position + new Vector3(Random.Range(-4.0f, 4.0f), 
                        _objectToFollow.transform.position.y, Random.Range(-4.0f, 4.0f));
                    itr++;
                } while (!NavMesh.SamplePosition(targetPosition, out navMeshHit, float.PositiveInfinity, NavMesh.AllAreas) && itr < 5);

                _scammerBlackboard.NavAgent.enabled = false;
                _scammerBlackboard.transform.position = targetPosition;
                _scammerBlackboard.NavAgent.nextPosition = targetPosition;
                _scammerBlackboard.NavAgent.enabled = true;
            }
        }
        else nodeState = State.Failure;

        return nodeState;
    }
}
