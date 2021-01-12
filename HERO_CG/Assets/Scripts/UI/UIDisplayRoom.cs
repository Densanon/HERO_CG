using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using TMPro;
using UnityEngine;

public class UIDisplayRoom : MonoBehaviour
{
    //[SerializeField] private GameObject _startButton;
    [SerializeField] private GameObject _roomContainer;
    [SerializeField] private GameObject[] _hideObjects;
    [SerializeField] private GameObject[] _showObjects;

    //public static Action OnStartGame = delegate { };
    public static Action OnLeaveRoom = delegate { };

    private void Awake()
    {
        PhotonRoomController.OnJoinRoom += HandleJoinRoom;
        PhotonRoomController.OnRoomLeft += HandleRoomLeft;
        //PhotonRoomController.OnMasterOfRoom += HandleMasterOfRoom;
        //PhotonRoomController.OnCountingDown += HandleCountingDown;
    }

    private void OnDestroy()
    {
        PhotonRoomController.OnJoinRoom -= HandleJoinRoom;
        PhotonRoomController.OnRoomLeft -= HandleRoomLeft;
        //PhotonRoomController.OnMasterOfRoom -= HandleMasterOfRoom;
        //PhotonRoomController.OnCountingDown -= HandleCountingDown;
    }

    private void HandleJoinRoom(GameMode gameMode)
    {
        foreach (GameObject obj in _hideObjects)
        {
            obj.SetActive(false);
        }
    }

    private void HandleRoomLeft()
    {
        _roomContainer.SetActive(false);
        foreach (GameObject obj in _showObjects)
        {
            obj.SetActive(true);
        }
    }

    /*private void HandleMasterOfRoom(Player masterPlayer)
    {
        _roomGameModeText.SetText(PhotonNetwork.CurrentRoom.CustomProperties["GAMEMODE"].ToString());

        if (PhotonNetwork.LocalPlayer.Equals(masterPlayer))
        {
            _startButton.SetActive(true);
        }
        else
        {
            _startButton.SetActive(false);
        }
    }

    private void HandleCountingDown(float count)
    {
        //_startButton.SetActive(false);
        _exitButton.SetActive(false);
        _roomGameModeText.SetText(count.ToString("F0"));
    }*/

    public void LeaveRoom()
    {
        OnLeaveRoom?.Invoke();
    }

    /*public void StartGame()
    {
        Debug.Log($"Starting game...");
        OnStartGame?.Invoke();
    }*/
}
