using System;
using System.Collections.Generic;
using UnityEngine;

public class LoseWinHandler : MonoBehaviour
{
    public static readonly List<GameObject> goodCommunityMembers = new List<GameObject>();
    public static readonly List<GameObject> badCommunityMembers = new List<GameObject>();

    public static event EventHandler OnNoGoodMembers, OnNoBadMembers;
    
    
    private void Start()
    {
        foreach(var communityMember in GameObject.FindGameObjectsWithTag("AI"))
        {
            GoodMemberBlackboard test;
            if (communityMember.TryGetComponent<GoodMemberBlackboard>(out test))
                goodCommunityMembers.Add(communityMember);
            else badCommunityMembers.Add(communityMember);
        }

        OnNoGoodMembers += GameLose;
        OnNoBadMembers += GameWon;
    }
    
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

    public static void RemoveFromList(in GameObject pMember)
    {
        GoodMemberBlackboard lol;
        if (pMember.TryGetComponent<GoodMemberBlackboard>(out lol))
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

    public static void GameLose(object pSender, EventArgs pEventArgs)
    {
        Debug.Log("Game's over!");
    }    
    
    public static void GameWon(object pSender, EventArgs pEventArgs)
    {
        Debug.Log("Game's won!");
    }
}