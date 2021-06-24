using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetworkLoseWinHandler : NetworkBehaviour
{
    public static readonly List<GameObject> goodCommunityMembers = new List<GameObject>();
    public static readonly List<GameObject> badCommunityMembers = new List<GameObject>();

    private int previousGoodCount = 0;
    private int previousBadCount = 0;

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

        previousBadCount = badCommunityMembers.Count;
        previousGoodCount = goodCommunityMembers.Count;
        
        Debug.Log("Good: " + goodCommunityMembers.Count);
        Debug.Log("Bad: " + badCommunityMembers.Count);
        
        SendChangeMemberCountEvent();
        
        OnNoGoodMembers += GameLose;
        OnNoBadMembers += GameWon;
    }

    [ServerCallback]
    private void Update()
    {
        if (previousBadCount != badCommunityMembers.Count || previousGoodCount != goodCommunityMembers.Count)
        {
            SendChangeMemberCountEvent();
        }
    }

    [ServerCallback]
    public static void AddToBadList(in GameObject pMember)
    {
        Debug.Log("Adding gameobject to bad list");
        if (goodCommunityMembers.Contains(pMember) && !badCommunityMembers.Contains(pMember))
        {
            goodCommunityMembers.Remove(pMember);
            badCommunityMembers.Add(pMember);
        }

        if (goodCommunityMembers.Count == 0)
        {
            Debug.LogError("NO GOOD MEMBERS");
            OnNoGoodMembers?.Invoke(new object(), new EventArgs());   
            EventBroker.CallLoseWinEvent("Lost");
        }
    }

    [ServerCallback]
    public static void RemoveFromList(in GameObject pMember)
    {
        Debug.Log("Removing player from list");
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

        if (badCommunityMembers.Count <= 0)
        {
            Debug.LogError("NO BAD MEMBERS");
            OnNoBadMembers?.Invoke(new object(), new EventArgs()); 
            EventBroker.CallLoseWinEvent("Won");
        }
        
        if (goodCommunityMembers.Count <= 0)
        {
            Debug.LogError("NO GOOD MEMBERS");
            OnNoGoodMembers?.Invoke(new object(), new EventArgs());   
            EventBroker.CallLoseWinEvent("Lost");
        }
    }

    [ServerCallback]
    public static void GameLose(object pSender, EventArgs pEventArgs)
    {
        Debug.LogError("Game's over!");
        EventBroker.CallLoseWinEvent("Lost");
        //RpcLoseWin("Lost");
    }    
    
    [ServerCallback]
    public static void GameWon(object pSender, EventArgs pEventArgs)
    {
        Debug.LogError("Game's won!");
        EventBroker.CallLoseWinEvent("Won");
        //RpcLoseWin("Won");
    }

    [ClientRpc]
    private void RpcLoseWin(string pState)
    {
        Debug.Log("TEST");
        EventBroker.CallLoseWinEvent(pState);
    }

    [ServerCallback]
    private void SendChangeMemberCountEvent()
    {
        EventBroker.CallChangeMembersCount(goodCommunityMembers.Count, "good");
        EventBroker.CallChangeMembersCount(badCommunityMembers.Count, "bad");
        RpcSendChangeMemberCountEvent();
    }

    [ClientRpc]
    private void RpcSendChangeMemberCountEvent()
    {
        EventBroker.CallChangeMembersCount(goodCommunityMembers.Count, "good");
        EventBroker.CallChangeMembersCount(badCommunityMembers.Count, "bad");
    }
}
