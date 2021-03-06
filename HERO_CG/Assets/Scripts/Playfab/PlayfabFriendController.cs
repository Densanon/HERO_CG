﻿using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Linq;
using UnityEngine;

public class PlayfabFriendController : MonoBehaviour
{
    public static Action<List<FriendInfo>> OnFriendListUpdated = delegate { };
    public static Action<string, string> OnError = delegate { };

    private List<FriendInfo> friends;

    private void Awake()
    {
        friends = new List<FriendInfo>();
        PhotonConnector.GetPhotonFriends += HandleGetFriends;
        UIFriend.OnRemoveFriend += HandleRemoveFriend;
        UIDisplayInvites.OnFriendAdded += HandleAddPlayfabFriend;
        PhotonChatController.OnAddAddedFriend += HandleAddPlayfabFriend;
        PhotonChatController.OnFriendRemoved += HandleRemoveFriend;
    }
    private void OnDestroy()
    {
        PhotonConnector.GetPhotonFriends -= HandleGetFriends;
        UIFriend.OnRemoveFriend -= HandleRemoveFriend;
        UIDisplayInvites.OnFriendAdded -= HandleAddPlayfabFriend;
        PhotonChatController.OnAddAddedFriend -= HandleAddPlayfabFriend;
        PhotonChatController.OnFriendRemoved -= HandleRemoveFriend;

    }

    private void HandleGetFriends()
    {
        GetPlayfabFriends();
    }

    private void HandleRemoveFriend(string name)
    {
        string id = friends.FirstOrDefault(f => f.TitleDisplayName == name).FriendPlayFabId;
        var request = new RemoveFriendRequest { FriendPlayFabId = id };
        PlayFabClientAPI.RemoveFriend(request, OnFriendRemoveSuccess, OnFailure);
    }

    private void HandleAddPlayfabFriend(string name)
    {
        var request = new AddFriendRequest { FriendTitleDisplayName = name };
        PlayFabClientAPI.AddFriend(request, OnFriendAddedSuccess, OnFailure);
    }

    private void GetPlayfabFriends()
    {
        var request = new GetFriendsListRequest { IncludeSteamFriends = false, IncludeFacebookFriends = false, XboxToken = null };
        PlayFabClientAPI.GetFriendsList(request, OnFriendsListSuccess, OnFailure);
    }

    private void OnFriendAddedSuccess(AddFriendResult result)
    {
        GetPlayfabFriends();
    }

    private void OnFriendsListSuccess(GetFriendsListResult result)
    {
        friends = result.Friends;
        OnFriendListUpdated?.Invoke(result.Friends);
    }

    private void OnFriendRemoveSuccess(RemoveFriendResult result)
    {
        GetPlayfabFriends();
    }

    private void OnFailure(PlayFabError error)
    {
        Debug.Log($"Error occured when adding a friend {error.GenerateErrorReport()}");

    }

    public bool IsFriend(string friend)
    {
        GetPlayfabFriends();
        foreach(FriendInfo info in friends)
        {
            if (info.TitleDisplayName == friend)
            {
                OnError?.Invoke("Adding a Friend", $"You are already friends with {friend}.");
                return true;
            }
        }
        return false;
    }
}
