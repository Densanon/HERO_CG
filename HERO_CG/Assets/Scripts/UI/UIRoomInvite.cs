using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIRoomInvite : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject RoomMenu;
    public TMP_Text roomText; 

    private void Awake()
    {
        UIInvite.OnRoomInviteAccept += HandleRoomInviteAccepted;
    }

    private void OnDestroy()
    {
        UIInvite.OnRoomInviteAccept -= HandleRoomInviteAccepted;
    }

    private void HandleRoomInviteAccepted(string room)
    {
        roomText.text = "CUSTOM MATCH";
        RoomMenu.SetActive(true);
        MainMenu.SetActive(false);
    }
}
