using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PhotonGameManager : MonoBehaviourPunCallbacks
{
    public enum GamePhase { HeroDraft, AbilityDraft, HEROSelect, Heal, Enhance, Recruit, Overcome, Feat, TurnResponse}
    public static GamePhase myPhase = GamePhase.HeroDraft;

    public CardDataBase CB;

    public TMP_Text TurnIndicator;

    public GameObject gCardZoom;
    public CardData gCard;
    public GameObject gCardOption;

    public GameObject PhaseDeclarationUI;
    public TMP_Text PhaseText;

    public GameObject gHEROSelect;
    public GameObject gCardCountCollect;
    public Button bCardCount1;
    public Button bCardCount2;
    public Button bCardCount3;

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
        UIConfirmation.OnHEROSelection += PhaseChange;
        CardDataBase.OnAutoDraftCollected += HandleCardCollected;
    }

    private void OnDestroy()
    {
        PlayerBase.OnBaseDestroyed -= OnBaseDestroyed;
        CardFunction.OnCardSelected -= HandleCardSelecion;
        CardFunction.OnCardDeselected -= HandleDeselection;
        CardFunction.OnCardCollected -= HandleCardCollected;
        UIConfirmation.OnHEROSelection -= PhaseChange;
        CardDataBase.OnAutoDraftCollected -= HandleCardCollected;
    }

    private void Start()
    {
        CB.autoDraft = true;
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
        Debug.Log($"Phase is being changed to {phaseToChangeTo}");
        myPhase = phaseToChangeTo;
        HandlePhaseChange();
    }

    public void SwitchTurn(bool turn)
    {
        Debug.Log("I am handling my turn from what I was told.");
        myTurn = turn;
        StartCoroutine(TurnDeclaration(myTurn));
    }

    public void SetCardCollectAmount(int amount)
    {
        Debug.Log($"Setting card Collection to {amount}.");
        for(int i = amount; i>0; i--)
        {
            Debug.Log($"Amount to draw left: {i}");
            CB.DrawCard(CardDataBase.CardDecks.P1Deck);
        }
        CardPlaySetup();
    }

    public void HandCardZoom()
    {
        zoomed = true;
        gCardZoom.SetActive(true);
        //NEED TO OVERRIDE CARD WITH THE CURRENT ACTIVE CARD FROM THE CB ACTIVECARD
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
    }

    private void HEROSelectionBegin()
    {
        gHEROSelect.SetActive(true);
    }

    private void HandlePhaseChange()
    {
        switch (myPhase)
        {
            case GamePhase.Heal:
                gHEROSelect.SetActive(false);
                //Select 1+ heros to be healed(any)
                CardPlaySetup();
                break;
            case GamePhase.Enhance:
                //Ask quantity to draw
                //Draw up to 3 cards from enhance deck
                //Play up to 3 cards from your hand
                Debug.Log("Handling the 'Enhance' Option");
                gHEROSelect.SetActive(false);
                gCardCountCollect.SetActive(true);
                int i = CB.CardsRemaining(CardDataBase.CardDecks.P1Deck);
                switch (i)
                {
                    case 0:
                        gCardCountCollect.SetActive(false);
                        CardPlaySetup();
                        break;
                    case 1:
                        bCardCount1.interactable = true;
                        bCardCount2.interactable = false;
                        bCardCount3.interactable = false;
                        break;
                    case 2:
                        bCardCount1.interactable = true;
                        bCardCount2.interactable = true;
                        bCardCount3.interactable = false;
                        break;
                    default:
                        bCardCount1.interactable = true;
                        bCardCount2.interactable = true;
                        bCardCount3.interactable = true;
                        break;
                }
                break;
            case GamePhase.Recruit:
                gHEROSelect.SetActive(false);
                //pick up to 2 Heros either from Reserve or HQ
                break;
            case GamePhase.Overcome:
                gHEROSelect.SetActive(false);
                //Declare attacking hero(s) as a single attack, directed towards a single target
                //Calculate all attack power from total heros and abilities
                //Calculate all defensive power from total hero & abilities
                //Resolve, if defeated, place in discard, all attacking are exhausted
                //Able to repeate
                break;
            case GamePhase.Feat:
                gHEROSelect.SetActive(false);
                //Resolve card
                break;
            case GamePhase.TurnResponse:
                break;
            case GamePhase.HEROSelect:
                HEROSelectionBegin();
                break;
        }
    }

    private void CardPlaySetup()
    {
        //Prompt to play up to xamount, update as played
        //
        Debug.Log("Ready to Select Cards to play.");
        gCardCountCollect.SetActive(false);
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
        TurnIndicator.text = myTurn ? "My Turn" : "Opponent's Turn";
        PhaseDeclarationUI.SetActive(true);
        yield return new WaitForSeconds(2f);
        PhaseDeclarationUI.SetActive(false);
        if (myTurn)
        {
            switch (myPhase)
            {
                case GamePhase.HeroDraft:
                    StartCoroutine(PhaseDeclaration("Hero Drafting"));
                    if (CB.autoDraft)
                    {
                        Debug.Log("Taking auto turn in Hero Draft");
                        CB.DrawDraftCard("HeroSelection");
                    }
                    break;
                case GamePhase.AbilityDraft:
                    if (!bAbilityDraftStart)
                    {
                        StartCoroutine(PhaseDeclaration("Ability Drafting"));
                        bAbilityDraftStart = true;
                    }
                    if (CB.autoDraft)
                    {
                        Debug.Log("Taking auto turn in Ability Draft");
                        CB.DrawDraftCard("AbilityDraft");
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
