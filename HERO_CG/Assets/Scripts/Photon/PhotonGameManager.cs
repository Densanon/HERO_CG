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
    public static GamePhase myPhase = GamePhase.HeroDraft;

    public CardDataBase CB;

    public GameObject gCardZoom;
    public CardData gCard;
    public GameObject gCardOption;

    public GameObject PhaseDeclarationUI;
    public TMP_Text PhaseText;

    public GameObject gHEROSelect;

    public GameObject EndUI;
    public TMP_Text EndText;

    private bool bAI = false;
    public static bool myTurn { get; private set; }
    private bool bAbilityDraftStart = false;
    private bool selecting = false;
    private bool zoomed = false;
    private bool heroDrafted = false;

    public static Action<Card, GamePhase> OnCardCollected = delegate { };

    #region Unity Methods
    private void Awake()
    {
        PlayerBase.OnBaseDestroyed += OnBaseDestroyed;
        CardFunction.OnCardSelected += HandleCardSelecion;
        CardFunction.OnCardDeselected += HandleDeselection;
        CardFunction.OnCardCollected += HandleCardCollected;
    }

    private void OnDestroy()
    {
        PlayerBase.OnBaseDestroyed -= OnBaseDestroyed;
        CardFunction.OnCardSelected -= HandleCardSelecion;
        CardFunction.OnCardDeselected -= HandleDeselection;
        CardFunction.OnCardCollected -= HandleCardCollected;
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
                CB.HandleTurnDeclaration(!myTurn);
                CB.HandleBuildHeroDraft();
            }
            else
            {
                Debug.Log("I go last.");
                myTurn = false;
                StartCoroutine(TurnDeclaration(myTurn));
                CB.HandleTurnDeclaration(!myTurn);
                CB.HandleBuildHeroDraft();
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

    public void PhaseChange(GamePhase phaseToChangeTo)
    {
        myPhase = phaseToChangeTo;
    }

    public void SwitchTurn(bool turn)
    {
        Debug.Log("I am handling my turn from what I was told.");
        myTurn = turn;
        StartCoroutine(TurnDeclaration(myTurn));
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    #endregion

    #region Private Methods
    private void HandleDeselection()
    {
        zoomed = false;
    }

    private void HandleCardSelecion(CardData card)
    {
        switch (myPhase)
        {
            case GamePhase.HeroDraft:
                if (!zoomed)
                {
                    Debug.Log($"Zooming Card: {card.Name}");
                    CardZoom(card);
                }
                break;
            case GamePhase.AbilityDraft:
                if (!zoomed)
                {
                    Debug.Log($"Zooming Card: {card.Name}");
                    CardZoom(card);
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
        zoomed = true;
        gCardZoom.SetActive(true);
        gCard.CardOverride(card);
        if (myTurn) {
            gCardOption.SetActive(true);
        }
        else
        {
            gCardOption.SetActive(false);
        }
        Debug.Log("Card Overriden.");
    }

    private void HandleCardCollected(Card card)
    {
        zoomed = false;
        Debug.Log("Sending Card Collected.");
        CB.HandleCardCollected(card, myPhase);
        SwitchTurn();
        CB.HandleTurnDeclaration(!myTurn);
    }

    private void SwitchTurn()
    {
        myTurn = !myTurn;
        StartCoroutine(TurnDeclaration(myTurn));
        if(myPhase == GamePhase.HEROSelect)
        {
            HEROSelectionBegin();
        }
    }

    private void HEROSelectionBegin()
    {

    }
    #endregion

    #region IEnumerators
    private IEnumerator PhaseDeclaration(string phase)
    {
        PhaseText.text = phase;
        PhaseDeclarationUI.SetActive(true);
        yield return new WaitForSeconds(2f);
        PhaseDeclarationUI.SetActive(false);
    }

    private IEnumerator TurnDeclaration(bool myTurn)
    {
        PhaseText.text = myTurn ? "Your Turn!" : "Opponent's Turn";
        PhaseDeclarationUI.SetActive(true);
        yield return new WaitForSeconds(2f);
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
}
