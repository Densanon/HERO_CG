using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class UIFriendInvite : MonoBehaviour
{
    public string _friendName;
    [SerializeField] private TMP_Text _friendNameText;

    public static Action<UIFriendInvite> OnFriendInviteAccept = delegate { };
    public static Action<UIFriendInvite> OnFriendInviteDecline = delegate { };

    public void Initialize(string friendName)
    {
        _friendName = friendName;

        _friendNameText.SetText(_friendName);
    }

    public void AcceptInvite()
    {
        OnFriendInviteAccept?.Invoke(this);
    }

    public void DeclineInvite()
    {
        OnFriendInviteDecline?.Invoke(this);
    }
}
