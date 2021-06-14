using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkScammerBlackboard : NetworkAIBlackboard
{
    [SerializeField] private GameObject objectToFollow;
    [SerializeField] private float distanceToObject;
    
    protected override void AssembleBehaviourTree()
    {
        NetworkFollowObjectNode followObjectNode = new NetworkFollowObjectNode(this, objectToFollow, distanceToObject);
        SequenceNode followSequence = new SequenceNode(new List<Node>() {followObjectNode});

        _startingNode = new SelectorNode(new List<Node>() {followSequence});
    }
}