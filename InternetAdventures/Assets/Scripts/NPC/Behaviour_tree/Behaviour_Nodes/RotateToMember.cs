using UnityEngine;

public class RotateToMember : Node
{
    public RotateToMember(in AIBlackboard pAIBlackboard)
    {
        aiBlackboard = pAIBlackboard;
    }


    public override State EvaluateState()
    {
        nodeState = State.Success;
        if(aiBlackboard.MemberPair != null && aiBlackboard.NavAgent.velocity.magnitude < 0.1f && aiBlackboard.NavAgent.enabled == false){
            Vector3 vecToMember = aiBlackboard.MemberPair.transform.position - aiBlackboard.transform.position;
            if(Vector3.Angle(aiBlackboard.transform.forward, vecToMember.normalized) > 5){
                aiBlackboard.transform.rotation = Quaternion.Slerp(aiBlackboard.transform.rotation, Quaternion.LookRotation(vecToMember, aiBlackboard.transform.up), 0.02f);
            }
        }
        return nodeState;
    }
}
