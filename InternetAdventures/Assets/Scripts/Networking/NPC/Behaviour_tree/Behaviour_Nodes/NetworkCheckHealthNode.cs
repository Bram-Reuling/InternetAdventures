public class NetworkCheckHealthNode : Node
{
    //General
    private readonly NetworkAIBlackboard _aiBlackboard;
    
    //Health
    private readonly float _criticalHealthThreshold;

    public NetworkCheckHealthNode(NetworkAIBlackboard pAIBlackboard, float pCriticalHealthThreshold)
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
