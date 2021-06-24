using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetworkLoseWinHandler : NetworkBehaviour
{
    public static readonly List<GameObject> goodCommunityMembers = new List<GameObject>();
    public static readonly List<GameObject> badCommunityMembers = new List<GameObject>();

    public static event EventHandler OnNoGoodMembers, OnNoBadMembers;
    
    [ServerCallback]
    private void Start()
    {
        foreach(var communityMember in GameObject.FindGameObjectsWithTag("AI"))
        {
            NetworkGoodMemberBlackboard test;
            if (communityMember.TryGetComponent<NetworkGoodMemberBlackboard>(out test))
                goodCommunityMembers.Add(communityMember);
            else badCommunityMembers.Add(communityMember);
        }

        Debug.Log("Good: " + goodCommunityMembers.Count);
        Debug.Log("Bad: " + badCommunityMembers.Count);
        
        OnNoGoodMembers += GameLose;
        OnNoBadMembers += GameWon;
    }
    
    [ServerCallback]
    public static void AddToBadList(in GameObject pMember)
    {
        if (goodCommunityMembers.Contains(pMember) && !badCommunityMembers.Contains(pMember))
        {
            goodCommunityMembers.Remove(pMember);
            badCommunityMembers.Add(pMember);
        }
        if(goodCommunityMembers.Count == 0)
            OnNoGoodMembers?.Invoke(new object(), new EventArgs());
    }

    [ServerCallback]
    public static void RemoveFromList(in GameObject pMember)
    {
        NetworkGoodMemberBlackboard lol;
        if (pMember.TryGetComponent<NetworkGoodMemberBlackboard>(out lol))
        {
            if (goodCommunityMembers.Contains(pMember))
                goodCommunityMembers.Remove(pMember);
        }
        else
        {
            if (badCommunityMembers.Contains(pMember))
                badCommunityMembers.Remove(pMember);
        }
        
        if(badCommunityMembers.Count <= 0)
            OnNoBadMembers?.Invoke(new object(), new EventArgs());
        else if(goodCommunityMembers.Count <= 0)
            OnNoGoodMembers?.Invoke(new object(), new EventArgs());
    }

    [ServerCallback]
    public static void GameLose(object pSender, EventArgs pEventArgs)
    {
        Debug.LogError("Game's over!");
    }    
    
    [ServerCallback]
    public static void GameWon(object pSender, EventArgs pEventArgs)
    {
        Debug.LogError("Game's won!");
    }
}
