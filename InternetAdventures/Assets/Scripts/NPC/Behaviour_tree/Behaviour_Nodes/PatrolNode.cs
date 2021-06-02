using UnityEngine;
using UnityEngine.AI;

public class PatrolNode : Node
{
    //General
    private AIBlackboard _aiBlackboard;
    
    //Positional
    private float _minimalOffset = 2.0f;
    
    public PatrolNode(AIBlackboard pAIBlackboard)
    {
        _aiBlackboard = pAIBlackboard;
    }
    
    public override State EvaluateState()
    {
        GenerateNewPath();
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
            _aiBlackboard.NavAgent.CalculatePath(newPosition, navMeshPath);
            i++;
        } while (navMeshPath.status == NavMeshPathStatus.PathPartial && i < 10);
        
        _aiBlackboard.NavAgent.SetDestination(newPosition);
    }
}
