using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PhotonGameManager : MonoBehaviourPunCallbacks
{
    public GameObject TurnDeclarationUI;
    public TMP_Text TurnText;

    public GameObject EndUI;
    public TMP_Text EndText;

    private List<Card> Hand = new List<Card>();
    private List<Card> HeroHQ = new List<Card>();
    private List<Card> HeroReserves = new List<Card>();
    private List<Card> Abilities = new List<Card>();
    private List<Card> EnhancementDeck = new List<Card>();
    private List<Card> OpponentHand = new List<Card>();

    private bool bAI = false;
    private bool myTurn = false;

    #region Unity Methods
    private void Awake()
    {
        PlayerBase.OnBaseDestroyed += OnBaseDestroyed;
    }
    private void OnDestroy()
    {
        PlayerBase.OnBaseDestroyed -= OnBaseDestroyed;
    }
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            var turnStart = UnityEngine.Random.Range(0, 1);
            if(turnStart == 1)
            {
                myTurn = true;
                StartCoroutine(TurnDeclaration(myTurn));
                //let other know
            }
            else
            {
                myTurn = false;
                StartCoroutine(TurnDeclaration(myTurn));
                //let other know
            }
        }
        LoadArena();
    }

    #endregion

    #region Public Methods
    public void OnBaseDestroyed(PlayerBase pBase)
    {
        if(pBase.type == PlayerBase.Type.Player)
        {
            EndUI.SetActive(true);
            EndText.text = "You were Overcome!";
        }
        else
        {
            EndUI.SetActive(true);
            EndText.text = "You have Overcome!";
        }
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    #endregion

    #region Private Methods
    void LoadArena()
    {
        Debug.Log("Loading Game.");
        /*if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("PhotonNetword : trying to Load a level but we are not the master Client");
        }
        Debug.LogFormat("PhotonNetwork : Loading Level : Main Menu", PhotonNetwork.CurrentRoom.PlayerCount);
        if(PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            //PhotonNetwork.LoadLevel("1VOnline");
        }
        else
        {
            //PhotonNetwork.LoadLevel("AI");
            PhotonNetwork.CurrentRoom.IsOpen = false;
            bAI = true;
        }*/
    }

    private void SwitchTurn()
    {
        myTurn = !myTurn;
        StartCoroutine(TurnDeclaration(myTurn));
    }
    #endregion

    #region Photon Callbacks
    /// <summary>
    /// Called when the local player left the room. We need to load the Main Menu.
    /// </summary>
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public override void OnPlayerEnteredRoom(Player other)
    {
        //Not seen if you are the player connecting.
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName);

        if (PhotonNetwork.IsMasterClient)
        {
            //Called before OnPlayerLeftRoom
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient", PhotonNetwork.IsMasterClient);

            LoadArena();
        }
    }

    public override void OnPlayerLeftRoom(Player other)
    {
        //Seen when other disconnects
        Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName);

        if (PhotonNetwork.IsMasterClient)
        {
            //Called before OnPlayerLeftRoom
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);

            LoadArena();
        }
    }
    #endregion

    #region IEnumerators
    private IEnumerator TurnDeclaration(bool myTurn)
    {
        TurnText.text = myTurn ? "Your Turn!" : "Opponent's Turn";
        TurnDeclarationUI.SetActive(true);
        yield return new WaitForSeconds(2f);
        TurnDeclarationUI.SetActive(false);
    }
    #endregion
}
