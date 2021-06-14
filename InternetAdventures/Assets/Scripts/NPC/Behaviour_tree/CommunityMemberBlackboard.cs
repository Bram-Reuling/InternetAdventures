using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class CommunityMemberBlackboard : AIBlackboard
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

    public void InitializeData(float pMinTimer, float pMaxTimer, GameObject pMemberPair)
    {
        base.InitializeData();
        minTimer = pMinTimer;
        maxTimer = pMaxTimer;
        MemberPair = pMemberPair;
    }
}
