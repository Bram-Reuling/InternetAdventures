using System.Collections;
using System.Collections.Generic;
using Mirror;
using Networking;
using UnityEngine;
using UnityEngine.AI;

public class NetworkScammerBlackboard : NetworkAIBlackboard
{
    [SerializeField] private GameObject objectToFollow;
    [SerializeField] private float distanceToObject;

    protected override void AssembleBehaviourTree()
    {
    }

    [ServerCallback]
    public void PopulateStartingNode(NetworkCharacterMovement pNetworkCharacterMovement)
    {
        objectToFollow = pNetworkCharacterMovement.gameObject;

        NetworkFollowObjectNode followObjectNode = new NetworkFollowObjectNode(this, objectToFollow, distanceToObject);
        SequenceNode followSequence = new SequenceNode(new List<Node>() {followObjectNode});

        _startingNode = new SelectorNode(new List<Node>() {followSequence});
        RpcDisableNavMeshAgent();
    }

    [ClientRpc]
    private void RpcDisableNavMeshAgent()
    {
        NavAgent.enabled = false;
    }
}