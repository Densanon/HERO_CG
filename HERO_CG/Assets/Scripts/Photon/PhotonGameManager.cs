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
    private int iTurnGauge = 0;

    public GameObject gOvercome;
    public Button bSwitch;
    public Button bBattle;
    private List<CardData> AttackingHeros = new List<CardData>();
    private CardData DefendingHero;
    public static bool AttDef = true;

    public static Action<Card, GamePhase> OnCardCollected = delegate { };
    public static Action<bool> OnOvercomeTime = delegate { };
    public static Action OnOvercomeSwitch = delegate { };

    #region Unity Methods
    private void Awake()
    {
        PlayerBase.OnBaseDestroyed += OnBaseDestroyed;
        CardFunction.OnCardSelected += HandleCardSelecion;
        //CardFunction.OnHeroSelected += HandleHeroSelected;
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
        //CardFunction.OnHeroSelected -= HandleHeroSelected;
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
        if(myPhase == GamePhase.Overcome)
        {
            gOvercome.SetActive(false);
            OnOvercomeTime?.Invoke(false);
        }
        myPhase = phaseToChangeTo;
        HandlePhaseChange();
    }

    public void SetPlayerNum(PlayerNum playerSet)
    {
        player = playerSet;
    }

    public void SetTurnGauge(int newNum)
    {
        iTurnGauge = newNum;
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

    #region Overcome Method/Functions
    public void CalculateBattle()
    {
        if (AttackingHeros.Count > 0 && DefendingHero != null)
        {
            int tDmg = 0;
            foreach(CardData data in AttackingHeros)
            {
                tDmg += data.Attack;
                data.Exhaust();
            }
            AttackingHeros.Clear();

            DefendingHero.DamageCheck(tDmg);
            DefendingHero = null;

            SwitchAttDef();
        }
    }

    public void SwitchAttDef()
    {
        AttDef = !AttDef;
        OnOvercomeSwitch?.Invoke();
        Debug.Log($"Switching AttDef to: {AttDef}");
    }
    #endregion

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
        gCard.CardOverride(CB.CurrentActiveCard, CardData.FieldPlacement.Zoom);
        HandleCardButtons(CardData.FieldPlacement.Hand);
    }

    public void CheckHandZoomInEffect()
    {
       gCard.CardOverride(CB.CurrentActiveCard, CardData.FieldPlacement.Hand);
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
        if(iTurnCounter == 0)
        {
            if(myPhase == GamePhase.Recruit)
            {
                CB.FillHQ();
            }
            SwitchTurn();
            CB.HandleTurnDeclaration(!myTurn);
        }
    }
    #endregion

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    #endregion

    #region Private Methods
    private void HandleHeroSelected(CardData card)
    {
        if (CB.CheckIfMyCard(card))
        {
            if (!card.Exhausted)
            {
                if (AttackingHeros.Contains(card))
                {
                    Debug.Log($"Removing {card.Name} from Attacking.");
                    //Untarget Card
                    AttackingHeros.Remove(card);
                }
                else
                {
                    Debug.Log($"Adding {card.Name} to Attacking.");
                    //Target Card
                    AttackingHeros.Add(card);
                }
            }
        }
        else
        {
            if (!AttDef)
            {
                if(DefendingHero == card)
                {
                    Debug.Log($"Removing {card.Name} from Defending.");
                    //Untarget Card
                    DefendingHero = null;
                }
                else
                {
                    Debug.Log($"Adding {card.Name} to Defending.");
                    //Target Card
                    DefendingHero = card;
                    //turn on interactible for calculate
                }
            }
        }
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
                HandleHeroSelected(card);
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
        gCard.CardOverride(card, CardData.FieldPlacement.Zoom);
        HandleCardButtons(card.myPlacement);
    }

    private void HandleCardButtons(CardData.FieldPlacement placement)
    {
        if (myTurn)
        {
            switch (myPhase)
            {
                case GamePhase.HEROSelect:
                    NullZoomButtons();
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
                    switch (placement)
                    {
                        case CardData.FieldPlacement.Hand:
                            gCardSelect.SetActive(false);
                            gCardOption.SetActive(false);
                            gCardPlay.SetActive(true);
                            break;
                        case CardData.FieldPlacement.HQ:
                            NullZoomButtons();
                            break;
                        case CardData.FieldPlacement.Mine:
                            NullZoomButtons();
                            break;
                        case CardData.FieldPlacement.Opp:
                            NullZoomButtons();
                            break;
                    }
                    break;
                case GamePhase.Recruit:
                    switch (placement)
                    {
                        case CardData.FieldPlacement.Hand:
                            NullZoomButtons();
                            break;
                        case CardData.FieldPlacement.HQ:
                            gCardSelect.SetActive(false);
                            gCardOption.SetActive(true);
                            gCardPlay.SetActive(false);
                            break;
                        case CardData.FieldPlacement.Mine:
                            NullZoomButtons();
                            break;
                        case CardData.FieldPlacement.Opp:
                            NullZoomButtons();
                            break;
                    }
                    break;
                case GamePhase.Overcome:
                    if (AttDef)
                    {
                        switch (placement)
                        {
                            case CardData.FieldPlacement.Hand:
                                NullZoomButtons();
                                break;
                            case CardData.FieldPlacement.HQ:
                                NullZoomButtons();
                                break;
                            case CardData.FieldPlacement.Mine:
                                gCardSelect.SetActive(true);
                                gCardOption.SetActive(false);
                                gCardPlay.SetActive(false);
                                break;
                            case CardData.FieldPlacement.Opp:
                                NullZoomButtons();
                                break;
                        }
                    }
                    else
                    {
                        switch (placement)
                    {
                        case CardData.FieldPlacement.Hand:
                                NullZoomButtons();
                                break;
                        case CardData.FieldPlacement.HQ:
                                NullZoomButtons();
                                break;
                        case CardData.FieldPlacement.Mine:
                                NullZoomButtons();
                                break;
                        case CardData.FieldPlacement.Opp:
                                gCardSelect.SetActive(true);
                                gCardOption.SetActive(false);
                                gCardPlay.SetActive(false);
                                break;
                    }
                    }
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
                    NullZoomButtons();
                    break;
                case GamePhase.HeroDraft:
                    NullZoomButtons();
                    break;
                case GamePhase.TurnResponse:
                    gCardSelect.SetActive(false);
                    gCardOption.SetActive(false);
                    gCardPlay.SetActive(true);
                    break;
                case GamePhase.Wait:
                    NullZoomButtons();
                    break;
                
            }
        }
    }

    private void NullZoomButtons()
    {
        gCardSelect.SetActive(false);
        gCardOption.SetActive(false);
        gCardPlay.SetActive(false);
    }

    private void HandleCardCollected(Card card)
    {
        zoomed = false;
        Debug.Log($"Sending Card Collected: {card.Name}.");
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
                break;
            case GamePhase.Enhance:
                //Ask quantity to draw
                //Draw up to 3 cards from enhance deck
                //Play up to 3 cards from your hand
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
                break;
            case GamePhase.Recruit:
                //pick up to 2 Heros either from Reserve or HQ
                PhaseIndicator.text = "Recruit";
                gHEROSelect.SetActive(false);
                iTurnCounter = 2;
                TurnActionIndicator.text = $"Actions Remaining: {iTurnCounter}";
                break;
            case GamePhase.Overcome:
                //Declare attacking hero(s) as a single attack, directed towards a single target
                //Calculate all attack power from total heros and abilities
                //Calculate all defensive power from total hero & abilities
                //Resolve, if defeated, place in discard, all attacking are exhausted
                //Able to repeate
                PhaseIndicator.text = "Overcome";
                gHEROSelect.SetActive(false);
                gOvercome.SetActive(true);
                TurnActionIndicator.text = $"Actions Remaining: ~";
                AttDef = true;
                OnOvercomeTime?.Invoke(true);
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
                gOvercome.SetActive(false);
                //OnOvercomeTime?.Invoke(false);
                PhaseChange(GamePhase.Wait);
                break;
            case GamePhase.Recruit:
                PhaseChange(GamePhase.Wait);
                break;
            case GamePhase.TurnResponse:
                PhaseChange(GamePhase.HEROSelect);
                break;
            case GamePhase.Wait:
                if(iTurnGauge >= 9)
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
        if (myTurn)
        { 
            iTurnGauge++;
        }
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
