using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDisplayInvites : MonoBehaviour
{
    [SerializeField] private Transform inviteContainer;
    [SerializeField] private UIInvite uiInvitePrefab;
    [SerializeField] private RectTransform contentRect;
    [SerializeField] private Vector2 originalSize;
    [SerializeField] private Vector2 inceaseSize;

    private List<UIInvite> invites;

    private void Awake()
    {
        invites = new List<UIInvite>();
        contentRect = inviteContainer.GetComponent<RectTransform>();
        originalSize = contentRect.sizeDelta;
        inceaseSize = new Vector2(0, uiInvitePrefab.GetComponent<RectTransform>().sizeDelta.y);
        PhotonChatController.OnRoomInvite += HandleRoomInvite;
        UIInvite.OnInviteAccept += HandleInviteAccpet;
        UIInvite.OnInviteDecline += HandleInviteDecline;
    }

    private void OnDestroy()
    {
        PhotonChatController.OnRoomInvite -= HandleRoomInvite;
        UIInvite.OnInviteAccept -= HandleInviteAccpet;
        UIInvite.OnInviteDecline -= HandleInviteDecline;
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
