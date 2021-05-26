public class CheckHealthNode : Node
{
    //General
    private readonly AIBlackboard _aiBlackboard;
    
    //Health
    private readonly float _criticalHealthThreshold;

    public CheckHealthNode(AIBlackboard pAIBlackboard, float pCriticalHealthThreshold)
    {
        _aiBlackboard = pAIBlackboard;
        _criticalHealthThreshold = pCriticalHealthThreshold;
    }

    public override State EvaluateState()
    {
        nodeState = _aiBlackboard.CurrentHealth >= _criticalHealthThreshold ? State.Success : State.Failure;
        return nodeState;
    }
}
