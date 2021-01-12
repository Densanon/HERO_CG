using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;

public class PhotonConnector : MonoBehaviourPunCallbacks
{
    [SerializeField] private string nickName;
    private string gameVersion = "1";
    public static Action GetPhotonFriends = delegate { };
    public static Action OnLobbyJoined = delegate { };

    #region Unity Method
    private void Awake()
    {
        nickName = PlayerPrefs.GetString("USERNAME");
        UIInvite.OnRoomInviteAccept += HandleRoomInviteAccept;
    }
    private void Start()
    {
        ConnectToPhoton();
    }

    private void OnDestroy()
    {
        UIInvite.OnRoomInviteAccept -= HandleRoomInviteAccept;
    }
    #endregion

    #region Private Method
    private void ConnectToPhoton()
    {
        Debug.Log($"Connect to Photon as {nickName}");
        PhotonNetwork.AuthValues = new AuthenticationValues(nickName);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NickName = nickName;
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = gameVersion;
    }

    private void HandleRoomInviteAccept(string roomName)
    {
        PlayerPrefs.SetString("PHOTONROOM", roomName);
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            if (PhotonNetwork.InLobby)
            {
                JoinPlayerRoom();
            }
        }
    }

    private void JoinPlayerRoom()
    {
        string roomName = PlayerPrefs.GetString("PHOTONROOM");
        PlayerPrefs.SetString("PHOTONROOM", "");
        PhotonNetwork.JoinRoom(roomName);
    }
    #endregion

    #region Public Methods
    public void OnRoomLeaveClicked()
    {
        PhotonNetwork.LeaveRoom();
    }
    #endregion

    #region Photon Callbacks
    public override void OnConnectedToMaster()
    {
        Debug.Log($"You have connected to the Photon Master Server");
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }
    public override void OnJoinedLobby()
    {
        Debug.Log($"You have connected to a Photon Lobby");
        GetPhotonFriends?.Invoke();
        string roomName = PlayerPrefs.GetString("PHOTONROOM");
        if (!string.IsNullOrEmpty(roomName))
        {
            JoinPlayerRoom();
        }
    }
    #endregion
}
