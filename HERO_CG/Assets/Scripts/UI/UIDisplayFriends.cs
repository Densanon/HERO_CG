using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class UIDisplayFriends : MonoBehaviour
{
    [SerializeField] private Transform friendContainer;
    [SerializeField] private UIFriend uiFriendPrefab;
    [SerializeField] private RectTransform contentRect;
    [SerializeField] private Vector2 orginalSize;
    [SerializeField] private Vector2 increaseSize;

    private void Awake()
    {
        contentRect = friendContainer.GetComponent<RectTransform>();
        orginalSize = contentRect.sizeDelta;
        increaseSize = new Vector2(0, uiFriendPrefab.GetComponent<RectTransform>().sizeDelta.y);
        PhotonFriendController.OnDisplayFriends += HandleDisplayFriends;
        PhotonChatFriendController.OnDisplayFriends += HandleDisplayChatFriends;
    }

    private void OnDestroy()
    {
        PhotonFriendController.OnDisplayFriends -= HandleDisplayFriends;
        PhotonChatFriendController.OnDisplayFriends -= HandleDisplayChatFriends;
    }

    private void HandleDisplayFriends(List<FriendInfo> friends)
    {
        foreach(Transform child in friendContainer)
        {
            Destroy(child.gameObject);
        }

        foreach(FriendInfo friend in friends)
        {
            UIFriend uifriend = Instantiate(uiFriendPrefab, friendContainer);
            uifriend.Initialize(friend);
        }
    }

    private void HandleDisplayChatFriends(List<string> friends)
    {
        Debug.Log("UI remove prior friends displayed");
        foreach (Transform child in friendContainer)
        {
            Destroy(child.gameObject);
        }

        Debug.Log($"UI instantiate friends display {friends.Count}");
        contentRect.sizeDelta = orginalSize;

        foreach (string friend in friends)
        {
            UIFriend uifriend = Instantiate(uiFriendPrefab, friendContainer);
            uifriend.Initialize(friend);
            contentRect.sizeDelta += increaseSize;
        }
    }
}
