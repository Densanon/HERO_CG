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
    public enum GamePhase { HeroDraft, AbilityDraft, HEROSelect, Heal, Enhance, Recruit, Overcome, Feat, TurnResponse, Wait}
    public static GamePhase myPhase = GamePhase.HeroDraft;
    public enum PlayerNum { P1, P2, P3, P4}
    public static PlayerNum player;

    public CardDataBase CB;

    public TMP_Text TurnActionIndicator;
    public TMP_Text TurnIndicator;
    public TMP_Text PhaseIndicator;

    public GameObject gCardZoom;
    public CardData gCard;
    public GameObject gCardOption;
    public GameObject gCardSelect;
    public GameObject gCardPlay;

    public GameObject PhaseDeclarationUI;
    public TMP_Text PhaseText;

    public TMP_Text tOpponentHandCount;

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
    private bool zoomed = false;
    private bool handZoomed = false;
    private bool heroDrafted = false;
    private int iTurnCounter = 0;

    public static Action<Card, GamePhase> OnCardCollected = delegate { };

    #region Unity Methods
    private void Awake()
    {
        PlayerBase.OnBaseDestroyed += OnBaseDestroyed;
        CardFunction.OnCardSelected += HandleCardSelecion;
        CardFunction.OnHeroSelected += HandleHeroSelected;
        CardFunction.OnCardDeselected += HandleDeselection;
        CardFunction.OnCardCollected += HandleCardCollected;
        CardFunction.OnCardPlayed += HandlePlayCard;
        UIConfirmation.OnHEROSelection += PhaseChange;
        CardDataBase.OnAiDraftCollected += HandleCardCollected;
    }

    private void OnDestroy()
    {
        PlayerBase.OnBaseDestroyed -= OnBaseDestroyed;
        CardFunction.OnCardSelected -= HandleCardSelecion;
        CardFunction.OnHeroSelected -= HandleHeroSelected;
        CardFunction.OnCardDeselected -= HandleDeselection;
        CardFunction.OnCardCollected -= HandleCardCollected;
        CardFunction.OnCardPlayed -= HandlePlayCard;
        UIConfirmation.OnHEROSelection -= PhaseChange;
        CardDataBase.OnAiDraftCollected -= HandleCardCollected;
    }

    private void Start()
    {
        //CB.AiDraft = true;
        PhaseIndicator.text = "Hero Draft";
        tOpponentHandCount.text = "0";
        TurnActionIndicator.text = "1";
        if (PhotonNetwork.IsMasterClient)
        {
            var turnStart = UnityEngine.Random.Range(0, 2);
            if(turnStart == 1)
            {
                Debug.Log("I go first.");
                myTurn = false;
                player = PlayerNum.P1;
                CB.HandlePlayerDeclaration(0);
                SwitchTurn();
                CB.HandleBuildHeroDraft();
            }
            else
            {
                Debug.Log("I go last.");
                myTurn = true;
                player = PlayerNum.P2;
                CB.HandlePlayerDeclaration(1);
                SwitchTurn();
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

    public void SetPlayerNum(PlayerNum playerSet)
    {
        player = playerSet;
    }

    public void SetOpponentHandCount(int number)
    {
        Debug.Log($"Setting the hand text to {number}.");
        tOpponentHandCount.text = $"{number}";
    }

    public void EndTurn()
    {
        SwitchTurn();
        TurnActionIndicator.text = "0";
    }

    public void ToldSwitchTurn(bool turn)
    {
        myTurn = turn;
        StartCoroutine(TurnDeclaration(myTurn));
        PhaseAdjustment();
    }

    public void SetCardCollectAmount(int amount)
    {
        Debug.Log($"Setting card Collection to {amount}.");
        for(int i = amount; i>0; i--)
        {
            Debug.Log($"Amount to draw left: {i}");
            CB.DrawCard(CardDataBase.CardDecks.P1Deck);
        }
        StartCoroutine(PhaseDeclaration("Play Cards"));
        gCardCountCollect.SetActive(false);
    }

    public void HandCardZoom()
    {
        handZoomed = true;
        gCardZoom.SetActive(true);
        gCard.CardOverride(CB.CurrentActiveCard);
        HandleCardButtons();
    }

    public void CheckHandZoomInEffect()
    {
        if (handZoomed)
        {
            gCard.CardOverride(CB.CurrentActiveCard);
        }
    }

    #region Move Counter Methods
    public int GetTurnCounter()
    {
        return iTurnCounter;
    }

    public void TurnCounterDecrement()
    {
        iTurnCounter--;
        TurnActionIndicator.text = $"Actions Remaining: {iTurnCounter}";
    }
    #endregion

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    #endregion

    #region Private Methods
    private void HandleHeroSelected(Card card)
    {

    }

    private void HandleDeselection()
    {
        zoomed = false;
        handZoomed = false;
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
                if (!zoomed)
                {
                    Debug.Log($"Zooming Card: {card.Name}");
                    CardZoom(card);
                }
                break;
            case GamePhase.Enhance:
                if (!zoomed)
                {
                    Debug.Log($"Zooming Card: {card.Name}");
                    CardZoom(card);
                }
                break;
            case GamePhase.Recruit:
                if (!zoomed)
                {
                    Debug.Log($"Zooming Card: {card.Name}");
                    CardZoom(card);
                }
                break;
            case GamePhase.Overcome:
                if (!zoomed)
                {
                    Debug.Log($"Zooming Card: {card.Name}");
                    CardZoom(card);
                }
                break;
            case GamePhase.Feat:
                if (!zoomed)
                {
                    Debug.Log($"Zooming Card: {card.Name}");
                    CardZoom(card);
                }
                break;
            case GamePhase.TurnResponse:
                if (!zoomed)
                {
                    Debug.Log($"Zooming Card: {card.Name}");
                    CardZoom(card);
                }
                break;
            case GamePhase.Wait:
                if (!zoomed)
                {
                    Debug.Log($"Zooming Card: {card.Name}");
                    CardZoom(card);
                }
                break;
        }
    }

    private void HandlePlayCard(Card card)
    {
        CB.PlayCard(card);
    }

    private void CardZoom(CardData card)
    {
        zoomed = true;
        gCardZoom.SetActive(true);
        gCard.CardOverride(card);
        HandleCardButtons();
    }

    private void HandleCardButtons()
    {
        if (myTurn)
        {
            switch (myPhase)
            {
                case GamePhase.HEROSelect:
                    gCardSelect.SetActive(false);
                    gCardOption.SetActive(false);
                    gCardPlay.SetActive(false);
                    break;
                case GamePhase.AbilityDraft:
                    gCardSelect.SetActive(false);
                    gCardOption.SetActive(true);
                    gCardPlay.SetActive(false);
                    break;
                case GamePhase.HeroDraft:
                    gCardSelect.SetActive(false);
                    gCardOption.SetActive(true);
                    gCardPlay.SetActive(false);
                    break;
                case GamePhase.Heal:
                    gCardSelect.SetActive(true);
                    gCardOption.SetActive(false);
                    gCardPlay.SetActive(false);
                    break;
                case GamePhase.Enhance:
                    gCardSelect.SetActive(false);
                    gCardOption.SetActive(false);
                    gCardPlay.SetActive(true);
                    break;
                case GamePhase.Recruit:
                    if (!handZoomed)
                    {
                        gCardSelect.SetActive(false);
                        gCardOption.SetActive(true);
                        gCardPlay.SetActive(false);
                    }
                    else
                    {
                        gCardSelect.SetActive(false);
                        gCardOption.SetActive(false);
                        gCardPlay.SetActive(false);
                    }
                    break;
                case GamePhase.Overcome:
                    gCardSelect.SetActive(true);
                    gCardOption.SetActive(false);
                    gCardPlay.SetActive(false);
                    break;
                case GamePhase.Feat:
                    gCardSelect.SetActive(true);
                    gCardOption.SetActive(false);
                    gCardPlay.SetActive(false);
                    break;
            }
        }
        else
        {
            switch (myPhase)
            {
                case GamePhase.AbilityDraft:
                    gCardSelect.SetActive(false);
                    gCardOption.SetActive(false);
                    gCardPlay.SetActive(false);
                    break;
                case GamePhase.HeroDraft:
                    gCardSelect.SetActive(false);
                    gCardOption.SetActive(false);
                    gCardPlay.SetActive(false);
                    break;
                case GamePhase.TurnResponse:
                    gCardSelect.SetActive(false);
                    gCardOption.SetActive(false);
                    gCardPlay.SetActive(true);
                    break;
                case GamePhase.Wait:
                    gCardSelect.SetActive(false);
                    gCardOption.SetActive(false);
                    gCardPlay.SetActive(false);
                    break;
                
            }
        }
    }

    private void HandleCardCollected(Card card)
    {
        zoomed = false;
        Debug.Log("Sending Card Collected.");
        CB.HandleCardCollected(card, myPhase);
        if(myPhase != GamePhase.Recruit)
        {
            SwitchTurn();
        }
        else
        {
            if(iTurnCounter > 0)
            {
                TurnCounterDecrement();
            }
            else
            {
                SwitchTurn();
            }
        }
    }

    private void SwitchTurn()
    {
        myTurn = !myTurn;
        StartCoroutine(TurnDeclaration(myTurn));
        PhaseAdjustment();
        CB.HandleTurnDeclaration(!myTurn);
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
                //Select 1+ heros to be healed(any)
                PhaseIndicator.text = "Heal";
                gHEROSelect.SetActive(false);
                TurnActionIndicator.text = $"Actions Remaining: ~";
                StartCoroutine(PhaseDeclaration("Play Cards"));
                Debug.Log("Ready to Select Cards to play.");
                break;
            case GamePhase.Enhance:
                //Ask quantity to draw
                //Draw up to 3 cards from enhance deck
                //Play up to 3 cards from your hand
                Debug.Log("Handling the 'Enhance' Option");
                PhaseIndicator.text = "Enhance";
                gHEROSelect.SetActive(false);
                gCardCountCollect.SetActive(true);
                int i = CB.CardsRemaining(CardDataBase.CardDecks.P1Deck);
                switch (i)
                {
                    case 0:
                        gCardCountCollect.SetActive(false);
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
                iTurnCounter = 3;
                TurnActionIndicator.text = $"Actions Remaining: {iTurnCounter}";
                StartCoroutine(PhaseDeclaration("Play Cards"));
                break;
            case GamePhase.Recruit:
                PhaseIndicator.text = "Recruit";
                gHEROSelect.SetActive(false);
                iTurnCounter = 2;
                TurnActionIndicator.text = $"Actions Remaining: {iTurnCounter}";
                //pick up to 2 Heros either from Reserve or HQ
                break;
            case GamePhase.Overcome:
                PhaseIndicator.text = "Overcome";
                gHEROSelect.SetActive(false);
                TurnActionIndicator.text = $"Actions Remaining: ~";
                //Declare attacking hero(s) as a single attack, directed towards a single target
                //Calculate all attack power from total heros and abilities
                //Calculate all defensive power from total hero & abilities
                //Resolve, if defeated, place in discard, all attacking are exhausted
                //Able to repeate
                break;
            case GamePhase.Feat:
                PhaseIndicator.text = "Feat";
                TurnActionIndicator.text = $"Actions Remaining: 0";
                gHEROSelect.SetActive(false);
                //Resolve card
                break;
            case GamePhase.TurnResponse:
                PhaseIndicator.text = "Turn Response";
                gCardOption.SetActive(true);
                TurnActionIndicator.text = $"Actions Remaining: 0";
                break;
            case GamePhase.Wait:
                PhaseIndicator.text = "Wait";
                gCardOption.SetActive(false);
                TurnActionIndicator.text = $"Actions Remaining: ~";
                break;
            case GamePhase.HEROSelect:
                PhaseIndicator.text = "Hero Selection";
                HEROSelectionBegin();
                break;
            case GamePhase.AbilityDraft:
                PhaseIndicator.text = "Ability Draft";
                TurnActionIndicator.text = $"Actions Remaining: ~";
                break;
            case GamePhase.HeroDraft:
                PhaseIndicator.text = "Hero Draft";
                TurnActionIndicator.text = $"Actions Remaining: ~";
                break;
        }
    }

    private void PhaseAdjustment()
    {
        switch (myPhase)
        {
            case GamePhase.Enhance:
                PhaseChange(GamePhase.Wait);
                break;
            case GamePhase.Feat:
                PhaseChange(GamePhase.Wait);
                break;
            case GamePhase.Heal:
                PhaseChange(GamePhase.Wait);
                break;
            case GamePhase.Overcome:
                PhaseChange(GamePhase.Wait);
                break;
            case GamePhase.Recruit:
                PhaseChange(GamePhase.Wait);
                break;
            case GamePhase.TurnResponse:
                PhaseChange(GamePhase.HEROSelect);
                break;
            case GamePhase.Wait:
                PhaseChange(GamePhase.HEROSelect);
                break;
        }
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
                    if (CB.AiDraft)
                    {
                        Debug.Log("Taking auto turn in Hero Draft");
                        CB.DrawDraftCard("HeroReserve");
                    }
                    break;
                case GamePhase.AbilityDraft:
                    if (!bAbilityDraftStart)
                    {
                        StartCoroutine(PhaseDeclaration("Ability Drafting"));
                        bAbilityDraftStart = true;
                    }
                    if (CB.AiDraft)
                    {
                        Debug.Log("Taking auto turn in Ability Draft");
                        CB.DrawDraftCard("AbilityDraft");
                    }
                    if (CB.AutoDraft)
                    {

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
