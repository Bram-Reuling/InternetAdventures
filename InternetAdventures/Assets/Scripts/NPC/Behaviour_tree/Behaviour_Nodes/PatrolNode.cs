using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PatrolNode : Node
{
    //General
    private AIBlackboard _aiBlackboard;
    
    //Positional
    private float _minimalOffset = 2.0f;
    
    //Times
    private float _minWaitAtPosition = 2.5f;
    private float _maxWaitAtPosition = 8.0f;
    private float _timePassed;
    private float _timeToWait;

    public PatrolNode(AIBlackboard pAIBlackboard)
    {
        _aiBlackboard = pAIBlackboard;
        _timeToWait = Random.Range(_minWaitAtPosition, _maxWaitAtPosition);
    }
    
    public override State EvaluateState()
    {
        //This will be incorrect if behaviour tree is not evaluated every frame.
        if (_aiBlackboard.NavAgent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            _timePassed += Time.deltaTime;
        }

        if (_timePassed > _timeToWait)
        {
            GenerateNewPath();
            _timeToWait = Random.Range(_minWaitAtPosition, _maxWaitAtPosition);
            _timePassed = 0;
        }

        return State.Success;
    }

    private void GenerateNewPath()
    {
        NavMeshPath navMeshPath = new NavMeshPath();
        Vector3 newPosition = Vector3.zero;
        int i = 0;
        do
        {
            Vector2 randomXZDirection = Random.insideUnitCircle * Random.Range(_minimalOffset, 12.5f);
            newPosition = new Vector3(randomXZDirection.x, _aiBlackboard.transform.position.y, randomXZDirection.y);
            Debug.Log("Recalculated path");
            _aiBlackboard.NavAgent.CalculatePath(newPosition, navMeshPath);
            i++;
        } while (navMeshPath.status == NavMeshPathStatus.PathPartial && i < 10);
        
        _aiBlackboard.NavAgent.SetDestination(newPosition);
    }
}
