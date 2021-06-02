public abstract class Node
{
    public abstract State EvaluateState();
    protected State nodeState;
    public State NodeState => nodeState;
    protected AIBlackboard aiBlackboard;
}

public enum State
{
    Success, Failure, Running
}