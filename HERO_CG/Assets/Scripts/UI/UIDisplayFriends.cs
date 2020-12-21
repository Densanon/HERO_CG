using System;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using System.Linq;

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
        PhotonChatFriendController.OnDisplayFriends += HandleDisplayChatFriends;
    }

    private void OnDestroy()
    {
        PhotonChatFriendController.OnDisplayFriends -= HandleDisplayChatFriends;
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
