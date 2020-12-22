using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using System;
using UnityEngine;
using System.Collections.Generic;
using PlayFab.ClientModels;

public class PhotonChatController : MonoBehaviour, IChatClientListener
{
    [SerializeField] private string nickName;
    [SerializeField] private PlayfabFriendController fController;
    private ChatClient chatClient;

    public static Action<string, string> OnRoomInvite = delegate { };
    public static Action<string> OnFriendshipinvite = delegate { };
    public static Action<ChatClient> OnChatConnected = delegate { };
    public static Action<PhotonStatus> OnStatusUpdated = delegate { };
    public static Action<string> OnAddAddedFriend = delegate { };
    public static Action<string> OnFriendRemoved = delegate { };
    public static Action<string, string> OnError = delegate { };

    #region Unity Methods
    private void Awake()
    {
        nickName = PlayerPrefs.GetString("USERNAME");
        UIFriend.OnInviteFriend += HandleFriendInvite;
        UIFriend.OnRemoveFriend += HandleRemoveFriend;
        UIAddFriend.OnAddFriend += HandleAddFriendInvite;
        UIDisplayInvites.OnFriendAdded += HandleFriendAddedResponse;
    } 

    private void OnDestroy()
    {
        UIFriend.OnInviteFriend -= HandleFriendInvite;
        UIFriend.OnRemoveFriend -= HandleRemoveFriend;
        UIAddFriend.OnAddFriend -= HandleAddFriendInvite;
        UIDisplayInvites.OnFriendAdded -= HandleFriendAddedResponse;
    }

    private void Start()
    {
        chatClient = new ChatClient(this);
        ConnectToPhotonChat();
    }

    void Update()
    {
        chatClient.Service();
    }
    #endregion

    #region Private Methods
    private void ConnectToPhotonChat()
    {
        Debug.Log("Connecting to Photon Chat");
        chatClient.AuthValues = new Photon.Chat.AuthenticationValues(nickName);
        ChatAppSettings chatSettings = PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings();
        chatClient.ConnectUsingSettings(chatSettings);
    }
    #endregion

    #region Public Methods
    private void HandleAddFriendInvite(string recipient)
    {
        if (string.IsNullOrEmpty(recipient) || fController.IsFriend(recipient)) return;
        chatClient.SendPrivateMessage(recipient, "Add?");
        OnError?.Invoke("Add Friend", $"You have sent a friend invite to {recipient}.");
    }

    private void HandleFriendAddedResponse(string recipient)
    {
        if (string.IsNullOrEmpty(recipient)) return;
        chatClient.SendPrivateMessage(recipient, "Added");
    }

    public void HandleFriendInvite(string recipient)
    {
        if (!PhotonNetwork.InRoom) return;
        chatClient.SendPrivateMessage(recipient, PhotonNetwork.CurrentRoom.Name);
    }

    public void HandleRemoveFriend(string recipient)
    {
        if (string.IsNullOrEmpty(recipient)) return;
        chatClient.SendPrivateMessage(recipient, "Remove");
    }
    #endregion

    #region Photon Chat Callback

    public void DebugReturn(DebugLevel level, string message)
    {
        
    }

    public void OnDisconnected()
    {
        Debug.Log($"You have Disconnected to Photon Chat.");
        chatClient.SetOnlineStatus(ChatUserStatus.Offline);
    }

    public void OnConnected()
    {
        Debug.Log($"You have connected to Photon Chat.");
        OnChatConnected?.Invoke(chatClient);
        chatClient.SetOnlineStatus(ChatUserStatus.Online);
    }

    public void OnChatStateChange(ChatState state)
    {
       
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        if (!string.IsNullOrEmpty(message.ToString()))
        {
            // Channel Name format [Sender : Recipient]
            string[] splitNames = channelName.Split(new char[] { ':' });
            string senderName = splitNames[0];

            if (!sender.Equals(senderName, StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log($"{sender}: {message}");
                if (message.ToString() == "Add?")
                {
                    OnFriendshipinvite?.Invoke(sender);
                }
                else if(message.ToString() == "Added")
                {
                    OnAddAddedFriend?.Invoke(sender);
                }
                else if(message.ToString() == "Remove")
                {
                    OnFriendRemoved?.Invoke(sender);
                }
                else
                {
                    OnRoomInvite?.Invoke(sender, message.ToString());
                }
            }
        }
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        
    }

    public void OnUnsubscribed(string[] channels)
    {
        
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) {
        Debug.Log($"Photon Chat OnStatusUpdate: {user} changed to {status}: {message}");
        PhotonStatus newStatus = new PhotonStatus(user, status, (string)message);
        Debug.Log($"Status Update for {user} and its now {status}.");
        OnStatusUpdated?.Invoke(newStatus);
    }

    public void OnUserSubscribed(string channel, string user)
    {
        
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        
    }
    #endregion
}
