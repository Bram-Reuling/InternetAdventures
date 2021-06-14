using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class NetworkCommunityMemberBlackboard : NetworkAIBlackboard
{
    [HideInInspector] public GameObject MemberPair;
    [SerializeField] protected float minTimer;
    [SerializeField] protected float maxTimer;
    [SerializeField] protected float memberProximity;
    
    public virtual List<GameObject> GetAllNPCs()
    {
        List<GameObject> allNPCS = GameObject.FindGameObjectsWithTag("AI").ToList();
        allNPCS.Remove(gameObject);
        return allNPCS;
    }
}
