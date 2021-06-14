using System.Collections.Generic;
using UnityEngine;

public class NetworkBadMemberBlackboard : NetworkCommunityMemberBlackboard
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
        NetworkConvertToBadNode convertToBadNode = new NetworkConvertToBadNode(this);
        SequenceNode atMember = new SequenceNode(new List<Node>() {atDestinationNode, hasMemberNode, rotateToMember, convertToBadNode});

        SelectorNode pairing = new SelectorNode(new List<Node>() {findNewMember, atMember});
        
        //Starting Node==============================================================================================================================
        _startingNode = new SelectorNode(new List<Node>() {healthSequence, pairing});
    }
}
