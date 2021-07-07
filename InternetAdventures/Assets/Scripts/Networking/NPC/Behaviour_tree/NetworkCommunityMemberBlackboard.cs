using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Networking.NPC;
using UnityEngine.AI;

public abstract class NetworkCommunityMemberBlackboard : NetworkAIBlackboard
{
    [HideInInspector] public GameObject MemberPair;
    [SerializeField] protected float minTimer;
    [SerializeField] protected float maxTimer;
    [SerializeField] protected float memberProximity;
    [SerializeField] public NetworkChatBubble networkChatBubble;
    
    public virtual List<GameObject> GetAllNPCs()
    {
        List<GameObject> allNPCS = GameObject.FindGameObjectsWithTag("AI").ToList();
        allNPCS.Remove(gameObject);
        return allNPCS;
    }
    
    public void InitializeData(float pMinTimer, float pMaxTimer, GameObject pMemberPair, NavMeshData pNavMesh)
    {
        minTimer = pMinTimer;
        maxTimer = pMaxTimer;
        MemberPair = pMemberPair;
        _navMesh = pNavMesh;
        networkChatBubble = GetComponent<NetworkChatBubble>();
        base.InitializeData();
    }
}
