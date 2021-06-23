using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetworkGoodMemberBlackboard : NetworkCommunityMemberBlackboard
{
    protected override void AssembleBehaviourTree()
    {
        //Health=====================================================================================================================================
        NetworkCheckHealthNode checkHealthNode = new NetworkCheckHealthNode(this, criticalHealthThreshold);
        SequenceNode healthSequence = new SequenceNode(new List<Node> {new InverterNode(checkHealthNode)});

        //Pairing====================================================================================================================================
        NetworkAtDestinationNode atDestinationNode = new NetworkAtDestinationNode(this);
        
        NetworkRandomTimerAtPositionNode randomTimerAtPositionNode = new NetworkRandomTimerAtPositionNode(this, minTimer, maxTimer);
        NetworkPotentialMemberNode potentialMemberNode = new NetworkPotentialMemberNode(this, memberProximity);
        NetworkTraverseToMember traverseToMember = new NetworkTraverseToMember(this, memberProximity);
        SequenceNode findNewMember = new SequenceNode(new List<Node>(){atDestinationNode, randomTimerAtPositionNode, potentialMemberNode, traverseToMember});

        NetworkHasMemberNode hasMemberNode = new NetworkHasMemberNode(this);
        NetworkRotateToMember rotateToMember = new NetworkRotateToMember(this);
        SequenceNode atMember = new SequenceNode(new List<Node>() {atDestinationNode, hasMemberNode, rotateToMember});

        SelectorNode pairing = new SelectorNode(new List<Node>() {findNewMember, atMember});
        
        //Starting Node==============================================================================================================================
        _startingNode = new SelectorNode(new List<Node>() {healthSequence, pairing});

        StartCoroutine(DisableNavMeshAgent());
    }

    IEnumerator DisableNavMeshAgent()
    {
        yield return new WaitForSeconds(2.0f);
        RpcDisableNavMeshAgent();
    }
    
    [ClientRpc]
    private void RpcDisableNavMeshAgent()
    {
        NavAgent.enabled = false;
    }

    public void TurnBad()
    {
        //Turn bad called
        Debug.Log(gameObject.name + " turned bad!");
        gameObject.AddComponent<NetworkBadMemberBlackboard>().InitializeData(minTimer, maxTimer, MemberPair);
        Destroy(this);
        gameObject.AddComponent<NetworkBadMemberBlackboard>();
    }
}