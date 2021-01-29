using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PhotonGameManager : MonoBehaviourPunCallbacks
{
    public enum GamePhase { HeroDraft, AbilityDraft, HEROSelect, Heal, Enhance, Recruit, Overcome, Feat}
    public GamePhase myPhase = GamePhase.HeroDraft;

    public GameObject gCardZoom;
    public CardData gCard;

    public GameObject PhaseDeclarationUI;
    public TMP_Text PhaseText;

    public GameObject EndUI;
    public TMP_Text EndText;

    private bool bAI = false;
    public static bool myTurn { get; private set; }
    private bool bAbilityDraftStart = false;
    private bool selecting = false;
    private bool zoomed = false;

    public static Action<bool> OnTurnDecided = delegate { };
    public static Action OnBuildHeroDraft = delegate { };
    public static Action<Card, GamePhase> OnCardCollected = delegate { };

    #region Unity Methods
    private void Awake()
    {
        PlayerBase.OnBaseDestroyed += OnBaseDestroyed;
        CardDataBase.OnTurnDelcarationReceived += SwitchTurn;
        CardFunction.OnCardSelected += HandleCardSelecion;
    }
    private void OnDestroy()
    {
        PlayerBase.OnBaseDestroyed -= OnBaseDestroyed;
        CardDataBase.OnTurnDelcarationReceived -= SwitchTurn;
        CardFunction.OnCardSelected -= HandleCardSelecion;
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            var turnStart = UnityEngine.Random.Range(0, 2);
            if(turnStart == 1)
            {
                Debug.Log("I go first.");
                myTurn = true;
                StartCoroutine(TurnDeclaration(myTurn));                
                OnTurnDecided?.Invoke(!myTurn);
                OnBuildHeroDraft?.Invoke();
            }
            else
            {
                Debug.Log("I go last.");
                myTurn = false;
                StartCoroutine(TurnDeclaration(myTurn));
                OnTurnDecided?.Invoke(!myTurn);
                OnBuildHeroDraft?.Invoke();
            }
        }
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
    private void HandleCardSelecion(CardData card)
    {
        switch (myPhase)
        {
            case GamePhase.HeroDraft:
                if (!zoomed)
                {
                    CardZoom(card);
                }
                else
                {
                    CardCollected(card, GamePhase.HeroDraft);
                }
                break;
            case GamePhase.AbilityDraft:
                if (!zoomed)
                {
                    CardZoom(card);
                }
                else
                {
                    CardCollected(card, GamePhase.AbilityDraft);
                }
                break;
            case GamePhase.HEROSelect:
                break;
            case GamePhase.Heal:
                break;
            case GamePhase.Enhance:
                break;
            case GamePhase.Recruit:
                break;
            case GamePhase.Overcome:
                break;
            case GamePhase.Feat:
                break;
        }
    }

    private void CardZoom(CardData card)
    {
        gCardZoom.SetActive(true);
        gCard.CardOverride(card);
    }

    private void CardCollected(CardData card, GamePhase phase)
    {
        OnCardCollected?.Invoke(card.myCard, phase);
    }

    private void SwitchTurn()
    {
        myTurn = !myTurn;
        StartCoroutine(TurnDeclaration(myTurn));
    }
    private void SwitchTurn(bool turn)
    {
        Debug.Log("I am handling my turn from what I was told.");
        myTurn = turn;
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
        }
    }
    #endregion

    #region IEnumerators
    private IEnumerator PhaseDeclaration(string phase)
    {
        PhaseText.text = phase;
        PhaseDeclarationUI.SetActive(true);
        yield return new WaitForSeconds(4f);
        PhaseDeclarationUI.SetActive(false);
    }

    private IEnumerator TurnDeclaration(bool myTurn)
    {
        PhaseText.text = myTurn ? "Your Turn!" : "Opponent's Turn";
        PhaseDeclarationUI.SetActive(true);
        yield return new WaitForSeconds(3f);
        PhaseDeclarationUI.SetActive(false);
        if (myTurn)
        {
            switch (myPhase)
            {
                case GamePhase.HeroDraft:
                    StartCoroutine(PhaseDeclaration("Hero Drafting"));
                    break;
                case GamePhase.AbilityDraft:
                    if (!bAbilityDraftStart)
                    {
                        StartCoroutine(PhaseDeclaration("Ability Drafting"));
                        bAbilityDraftStart = true;
                    }
                    break;
                case GamePhase.HEROSelect:
                    StartCoroutine(PhaseDeclaration("H.E.R.O. Decision"));
                    break;
            }
        }
    }
    #endregion
}
