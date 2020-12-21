using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDisplayInvites : MonoBehaviour
{
    [SerializeField] private Transform inviteContainer;
    [SerializeField] private UIInvite uiInvitePrefab;
    [SerializeField] private UIFriendInvite uiFriendInvitePrefab;
    [SerializeField] private RectTransform contentRect;
    [SerializeField] private Vector2 originalSize;
    [SerializeField] private Vector2 inceaseSize;

    private List<UIInvite> invites;
    private List<UIFriendInvite> fInvites;

    public static Action<string> OnFriendAdded = delegate { };

    private void Awake()
    {
        invites = new List<UIInvite>();
        fInvites = new List<UIFriendInvite>();
        contentRect = inviteContainer.GetComponent<RectTransform>();
        originalSize = contentRect.sizeDelta;
        inceaseSize = new Vector2(0, uiInvitePrefab.GetComponent<RectTransform>().sizeDelta.y);
        PhotonChatController.OnRoomInvite += HandleRoomInvite;
        PhotonChatController.OnFriendshipinvite += HandleFriendshipInvite;
        UIInvite.OnInviteAccept += HandleInviteAccpet;
        UIInvite.OnInviteDecline += HandleInviteDecline;
        UIFriendInvite.OnFriendInviteAccept += HandleFriendInviteAccept;
        UIFriendInvite.OnFriendInviteDecline += HandleFriendInviteDecline;
    }

    private void OnDestroy()
    {
        PhotonChatController.OnRoomInvite -= HandleRoomInvite;
        PhotonChatController.OnFriendshipinvite -= HandleFriendshipInvite;
        UIInvite.OnInviteAccept -= HandleInviteAccpet;
        UIInvite.OnInviteDecline -= HandleInviteDecline;
        UIFriendInvite.OnFriendInviteAccept -= HandleFriendInviteAccept;
        UIFriendInvite.OnFriendInviteDecline -= HandleFriendInviteDecline;
    }

    private void HandleFriendshipInvite(string friend)
    {
        Debug.Log($"{friend} has asked to be your friend");
        UIFriendInvite invite = Instantiate(uiFriendInvitePrefab, inviteContainer);
        invite.Initialize(friend);
        contentRect.sizeDelta += inceaseSize;
        fInvites.Add(invite);
    }

    private void HandleFriendInviteAccept(UIFriendInvite invite)
    {
        if (fInvites.Contains(invite))
        {
            fInvites.Remove(invite);
            Destroy(invite.gameObject);
            OnFriendAdded?.Invoke(invite._friendName);
        }
    }

    private void HandleFriendInviteDecline(UIFriendInvite invite)
    {
        if (fInvites.Contains(invite))
        {
            fInvites.Remove(invite);
            Destroy(invite.gameObject);
        }
    }

    private void HandleRoomInvite(string friend, string room)
    {
        Debug.Log($"Room invite from {friend} to room {room}");
        UIInvite uiInvite = Instantiate(uiInvitePrefab, inviteContainer);
        uiInvite.Initialize(friend, room);
        contentRect.sizeDelta += inceaseSize;
        invites.Add(uiInvite);
    }

    private void HandleInviteAccpet(UIInvite invite)
    {
        if (invites.Contains(invite))
        {
            invites.Remove(invite);
            Destroy(invite.gameObject);
        }
    }

    private void HandleInviteDecline(UIInvite invite)
    {
        if (invites.Contains(invite))
        {
            invites.Remove(invite);
            Destroy(invite.gameObject);
        }
    }
}
