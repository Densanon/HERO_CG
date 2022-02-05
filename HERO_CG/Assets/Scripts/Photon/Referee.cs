﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class Referee : MonoBehaviour
{
    public enum GamePhase { HeroDraft, AbilityDraft, HEROSelect, Heal, Enhance, Recruit, Overcome, Feat, PostAction, TurnResponse, Wait }
    public static GamePhase myPhase = GamePhase.HeroDraft;
    public static GamePhase prevPhase = GamePhase.Wait;
    public enum PlayerNum { P1, P2, P3, P4 }
    public static PlayerNum player;

    public CardDataBase CB;
    public PhotonInGameManager myManager;

    public TMP_Text TurnIndicator;
    public TMP_Text PhaseIndicator;

    public GameObject gCardZoom;
    public CardData gCard;
    public GameObject pAbilityPrefab;
    public Transform tAbilityContainer;
    List<GameObject> gAbilities = new List<GameObject>();
    public GameObject gCardCollect;
    public TMP_Text tCardsToCollectReserve;
    public TMP_Text tCardsToDrawMyDeck;
    public GameObject DrawDeckButton;
    public int iEnhanceCardsToCollect;
    public GameObject gCardSelect;
    public GameObject gCardPlay;

    public GameObject PhaseDeclarationUI;
    public TMP_Text PhaseText;

    public TMP_Text tOpponentHandCount;

    public GameObject gHEROSelect;
    public Button bHEROSelectHeal;
    public Button bHEROSelectEnhance;
    public Button bHEROSelectRectruit;
    public Button bHEROSelectOvercome;
    public Button bHEROSelectFeat;
    public Button bDrawEnhancementCards;

    public GameObject EndUI;
    public TMP_Text EndText;

    private bool bAI = false;

    public static bool myTurn { get; private set; }
    bool bAwaitingResponse = false;

    private bool bAbilityDraftStart = false;
    private bool zoomed = false;
    private bool handZoomed = false;

    private bool heroDrafted = false;

    private int iTurnCounter = 0;
    private int iTurnGauge = 0;
    private bool bEndTurn = true;
    public Button btEndTurn;

    private bool canPlayAbilityToField = true;
    private int abilityPlaySilenceTurnTimer = 0;

    public GameObject gOvercome;
    public Button bSwitch;
    public Button bBattle;
    public static List<CardData> AttackingHeros = new List<CardData>();
    public static List<CardData> PreviousAttackers = new List<CardData>();
    public static CardData DefendingHero;
    public static CardData PreviousDefender;
    public static bool AttDef = true;
    public static bool OpponentDestroyed = false;
    public static bool OpponentExhausted = false;

    public PlayerBase PB;
    public PlayerBase MyPB;

    private Ability activeAbility;
    private bool abilityTargetting = false;

    //public static Action<Card, GamePhase> OnCardCollected = delegate { };
    public static Action<bool> OnOvercomeTime = delegate { };
    public static Action OnOvercomeSwitch = delegate { };
    public static Action<Ability.PassiveType> OnPassiveActivate = delegate { };
    public static Action OnTurnResetabilities = delegate { };
    public static Action OnWaitTimer = delegate { };

    #region Unity Methods
    private void Awake()
    {
        CardFunction.OnCardSelected += HandleCardSelecion;
        CardFunction.OnCardDeselected += HandleDeselection;
        CardFunction.OnCardCollected += HandleCardCollected;
        CardFunction.OnCardPlayed += HandlePlayCard;
        UIConfirmation.OnHEROSelection += PhaseChange;
        CardDataBase.OnAiDraftCollected += HandleCardCollected;
        /*Ability.OnAbilityUsed += HandleAbilityEnd;
        Ability.OnFeatComplete += HandleFeatComplete;
        Ability.OnNeedCardDraw += DrawCardOption;
        Ability.OnSetActive += SetActiveAbility;
        Ability.OnActivateable += HandleActivateAbilitySetup;
        Ability.OnHoldTurn += HandleHoldTurn;
        Ability.OnHoldTurnOffOppTurn += HandleHoldTurnOff;*/
    }

    private void OnDestroy()
    {
        CardFunction.OnCardSelected -= HandleCardSelecion;
        CardFunction.OnCardDeselected -= HandleDeselection;
        CardFunction.OnCardCollected -= HandleCardCollected;
        CardFunction.OnCardPlayed -= HandlePlayCard;
        UIConfirmation.OnHEROSelection -= PhaseChange;
        CardDataBase.OnAiDraftCollected -= HandleCardCollected;
        /*Ability.OnAbilityUsed -= HandleAbilityEnd;
        Ability.OnFeatComplete -= HandleFeatComplete;
        Ability.OnNeedCardDraw -= DrawCardOption;
        Ability.OnSetActive -= SetActiveAbility;
        Ability.OnActivateable -= HandleActivateAbilitySetup;
        Ability.OnHoldTurn -= HandleHoldTurn;
        Ability.OnHoldTurnOffOppTurn -= HandleHoldTurnOff;*/
    }

    private void Start()
    {
        PhaseIndicator.text = "Hero Draft";
        tOpponentHandCount.text = "0";
        tCardsToCollectReserve.text = "";
        tCardsToDrawMyDeck.text = "";
        bDrawEnhancementCards.interactable = false;
        if (myManager.IsMasterClient())
        {
            var turnStart = UnityEngine.Random.Range(0, 2);
            if (turnStart == 1)
            {
                myTurn = false;
                player = PlayerNum.P1;
                myManager.RPCRequest("DeclarePlayer", RpcTarget.OthersBuffered, 0);
                SwitchTurn();
                CB.HandleBuildHeroDraft();
            }
            else
            {
                myTurn = true;
                player = PlayerNum.P2;
                myManager.RPCRequest("DeclarePlayer", RpcTarget.OthersBuffered, 1);
                SwitchTurn();
                CB.HandleBuildHeroDraft();
            }
        }
    }
    #endregion

    #region Player Declarations
    public void SetPlayerNum(PlayerNum playerSet)
    {
        player = playerSet;
    }

    public void SetOpponentHandCount(int number)
    {
        tOpponentHandCount.text = $"{number}";
    }
    #endregion

    #region Turn Methods

    #region Switching Turn Methods
    private void SwitchTurn()
    {
        if (myTurn)
        {
            abilityPlaySilenceTurnTimer--;
            if (abilityPlaySilenceTurnTimer <= 0)
            {
                canPlayAbilityToField = true;
            }
        }
        myTurn = !myTurn;
        GenericTurnChangables();
        NextPhase();
        HandleTurnDeclaration(!myTurn);

    }

    public void ToldSwitchTurn(bool turn)
    {
        myTurn = turn;
        GenericTurnChangables();
        NextPhase();
        
    }

    public void EndTurn()
    {
        GenericTurnChangables();
        bEndTurn = true;
        if (myPhase == GamePhase.Recruit)
        {
            CB.FillHQ();
        }
        //StartCoroutine(EndturnDelay());
        //TurnActionIndicator.text = "0";
        PhaseChange(GamePhase.Wait);
    }

    private void GenericTurnChangables()
    {
        SwitchEndTurnButtonInteractible(myTurn);
        StartCoroutine(TurnDeclaration(myTurn));
        OnTurnResetabilities?.Invoke();
    }

    private void SwitchEndTurnButtonInteractible(bool interactible)
    {
        btEndTurn.gameObject.SetActive(interactible);
    }
    #endregion

    #region Turn Declarations Methods
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

    private IEnumerator PhaseDeclaration(string phase)
    {
        PhaseText.text = phase;
        PhaseDeclarationUI.SetActive(true);
        yield return new WaitForSeconds(2f);
        PhaseDeclarationUI.SetActive(false);
    }

    private void HandleTurnDeclaration(bool myNewTurn)
    {
        myManager.RPCRequest("DeclaredTurn", RpcTarget.Others, myNewTurn);
    }
    #endregion

    #region Phase Methods
    private void NextPhase()
    {
        switch (myPhase)
        {
            case GamePhase.Enhance:
                PhaseChange(GamePhase.PostAction);
                break;
            case GamePhase.Feat:
                PhaseChange(GamePhase.PostAction);
                break;
            case GamePhase.Heal:
                PhaseChange(GamePhase.PostAction);
                break;
            case GamePhase.Overcome:
                gOvercome.SetActive(false);
                OnOvercomeTime?.Invoke(false);
                PhaseChange(GamePhase.PostAction);
                break;
            case GamePhase.Recruit:
                PhaseChange(GamePhase.PostAction);
                break;
            case GamePhase.PostAction:
                PhaseChange(GamePhase.Wait);
                break;
            case GamePhase.TurnResponse:
                PhaseChange(GamePhase.Wait);
                break;
            case GamePhase.Wait:
                if (iTurnGauge >= 9)
                    PhaseChange(GamePhase.HEROSelect);
                break;
        }
    }

    public void PhaseChange(GamePhase phaseToChangeTo)
    {
        Debug.Log($"{player}: Changing Phase to {phaseToChangeTo} from {myPhase}");
        if (myPhase == GamePhase.Overcome)
        {
            gOvercome.SetActive(false);
            OnOvercomeTime?.Invoke(false);
        }
        prevPhase = myPhase;
        myPhase = phaseToChangeTo;
        HandlePhaseChange();
    }

    private void HandlePhaseChange()
    {
        switch (myPhase)
        {
            case GamePhase.Heal:
                //Select 1+ heros to be healed(any)
                PhaseIndicator.text = "Heal";
                gHEROSelect.SetActive(false);
                StartCoroutine(PhaseDeclaration("Heal Heros"));
                break;
            case GamePhase.Enhance:
                //Ask quantity to draw
                //Draw up to 3 cards from enhance deck
                //Play up to 3 cards from your hand
                StartCoroutine(PhaseDeclaration("Draw and Play Cards"));
                PhaseIndicator.text = "Enhance";
                gHEROSelect.SetActive(false);
                bDrawEnhancementCards.interactable = true;
                //DrawCardOption(3);
                iTurnCounter = 3;
                iEnhanceCardsToCollect = 3;
                int temp = CB.CardsRemaining(CardDataBase.CardDecks.P1Deck);
                switch (temp)
                {
                    case 0:
                        tCardsToDrawMyDeck.text = $"0/0";
                        break;
                    case 1:
                        tCardsToDrawMyDeck.text = $"1/1";
                        break;
                    case 2:
                        tCardsToDrawMyDeck.text = $"2/2";
                        break;
                    default:
                        tCardsToDrawMyDeck.text = $"3/{temp}";
                        break;
                }
                //TurnActionIndicator.text = $"Actions Remaining: {iTurnCounter}";
                break;
            case GamePhase.Recruit:
                //pick up to 2 Heros either from Reserve or HQ
                StartCoroutine(PhaseDeclaration("Recruit Heros"));
                PhaseIndicator.text = "Recruit";
                gHEROSelect.SetActive(false);
                int temp1 = CB.CardsRemaining(CardDataBase.CardDecks.Reserve);
                switch (temp1)
                {
                    case 0:
                        tCardsToCollectReserve.text = $"0/0";
                        break;
                    case 1:
                        tCardsToCollectReserve.text = $"1/1";
                        break;
                    default:
                        tCardsToCollectReserve.text = $"2/{temp1}";
                        break;
                }
                iTurnCounter = 2;
                break;
            case GamePhase.Overcome:
                //Declare attacking hero(s) as a single attack, directed towards a single target
                //Calculate all attack power from total heros and abilities
                //Calculate all defensive power from total hero & abilities
                //Resolve, if defeated, place in discard, all attacking are exhausted
                //Able to repeate
                StartCoroutine(PhaseDeclaration("Select Heros to Battle"));
                PhaseIndicator.text = "Overcome";
                gHEROSelect.SetActive(false);
                gOvercome.SetActive(true);
                AttDef = true;
                OnOvercomeTime?.Invoke(true);
                break;
            case GamePhase.Feat:
                PhaseIndicator.text = "Feat";
                StartCoroutine(PhaseDeclaration("Pick Your Feat"));
                gHEROSelect.SetActive(false);
                //Resolve card
                break;
            case GamePhase.PostAction:
                StartCoroutine(PhaseDeclaration("Ending Phase"));
                PhaseIndicator.text = "Post Action";
                break;
            case GamePhase.TurnResponse:
                StartCoroutine(PhaseDeclaration("Player Response"));
                PhaseIndicator.text = "Turn Response";
                OnWaitTimer?.Invoke();
                break;
            case GamePhase.Wait:
                StartCoroutine(PhaseDeclaration("Wait for Opponent"));
                PhaseIndicator.text = "Wait";
                break;
            case GamePhase.HEROSelect:
                PhaseIndicator.text = "Hero Selection";
                SetDeckNumberAmounts();
                gHEROSelect.SetActive(true);
                //check for heros that can be healed
                bHEROSelectHeal.interactable = CB.CheckForHealableHeros();
                //check for cards in enhancement deck or hand
                bHEROSelectEnhance.interactable = (CB.CardsRemaining(CardDataBase.CardDecks.P1Deck) + CB.CardsRemaining(CardDataBase.CardDecks.P1Hand) > 0);
                //check for heros that are recruitable
                bHEROSelectRectruit.interactable = (CB.CardsRemaining(CardDataBase.CardDecks.HQ) + CB.CardsRemaining(CardDataBase.CardDecks.Reserve) > 0);
                //Check for usable hero cards
                bHEROSelectOvercome.interactable = CB.CheckMyFieldForUsableHeros();
                //Check for feat cards
                bHEROSelectFeat.interactable = CB.CheckIfFeatCards();
                break;
            case GamePhase.AbilityDraft:
                PhaseIndicator.text = "Ability Draft";
                break;
            case GamePhase.HeroDraft:
                PhaseIndicator.text = "Hero Draft";
                break;
        }
    }

    #endregion

    #endregion

    #region Card Methods

    public void SetDeckNumberAmounts()
    {
        tCardsToCollectReserve.text = $"{CB.CardsRemaining(CardDataBase.CardDecks.Reserve)}";
        tCardsToDrawMyDeck.text = $"{CB.CardsRemaining(CardDataBase.CardDecks.P1Deck)}";
    }

    private void CardZoom(CardData card)
    {
        zoomed = true;
        gCardZoom.SetActive(true);
        gCard.CardOverride(card, CardData.FieldPlacement.Zoom);
        HandleCardButtons(card.myPlacement);
        ClearAbilityPanel();
        GetNewAbilities(card);
    }

    private void HandleCardCollected(Card card)
    {
        zoomed = false;
        CB.HandleCardCollected(card, myPhase);
        if (myPhase != GamePhase.Recruit)
        {
            //PassiveActivate(Ability.PassiveType.ActionComplete);
            //StartCoroutine(EndturnDelay());
        }
        else
        {
            /*if(iTurnCounter > 0)
            {
                TurnCounterDecrement();
            }
            else
            {
                PassiveActivate(Ability.PassiveType.ActionComplete);
                            StartCoroutine(EndturnDelay());
            }*/
        }
    }

    public void SetCardCollectAmount(int amount)
    {
        for (int i = amount; i > 0; i--)
        {
            iEnhanceCardsToCollect--;
            CB.DrawCard(CardDataBase.CardDecks.P1Deck);
        }
        Debug.Log($"Enhance cards to be collected: {iEnhanceCardsToCollect}");

        if (iEnhanceCardsToCollect > 0)
        {
            tCardsToDrawMyDeck.text = $"{iEnhanceCardsToCollect}/{CB.CardsRemaining(CardDataBase.CardDecks.P1Deck)}";
        }
        else
        {
            Debug.Log("Ran out of cards to draw for the turn.");
            tCardsToDrawMyDeck.text = $"{CB.CardsRemaining(CardDataBase.CardDecks.P1Deck)}";
            bDrawEnhancementCards.interactable = false;
        }
    }

    #region Ability Methods

    private void ClearAbilityPanel()
    {
        if (gAbilities != null)
        {
            for (int i = gAbilities.Count - 1; i > -1; i--)
            {
                GameObject obj = gAbilities[i];
                gAbilities.Remove(obj);
                Destroy(obj);
            }
        }
    }

    private void GetNewAbilities(CardData card)
    {
        if (card.charAbility != null)
        {
            GameObject obj = Instantiate(pAbilityPrefab, tAbilityContainer);
            UICharacterAbility ca = obj.GetComponent<UICharacterAbility>();
            ca.AbilityAwake(card.charAbility);

            gAbilities.Add(obj);
        }

        if (card.myAbilities != null)
        {
            foreach (Ability a in card.myAbilities)
            {
                GameObject o = Instantiate(pAbilityPrefab, tAbilityContainer);
                UICharacterAbility uica = o.GetComponent<UICharacterAbility>();
                uica.AbilityAwake(a);

                gAbilities.Add(o);
            }
        }
    }

    #endregion

    #endregion

    #region Button Controls
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
                    gCardCollect.SetActive(true);
                    gCardPlay.SetActive(false);
                    break;
                case GamePhase.HeroDraft:
                    gCardSelect.SetActive(false);
                    gCardCollect.SetActive(true);
                    gCardPlay.SetActive(false);
                    break;
                case GamePhase.Heal:
                    gCardSelect.SetActive(true);
                    gCardCollect.SetActive(false);
                    gCardPlay.SetActive(false);
                    break;
                case GamePhase.Enhance:
                    switch (placement)
                    {
                        case CardData.FieldPlacement.Hand:
                            gCardSelect.SetActive(false);
                            gCardCollect.SetActive(false);
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
                            gCardCollect.SetActive(true);
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
                                gCardCollect.SetActive(false);
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
                                gCardCollect.SetActive(false);
                                gCardPlay.SetActive(false);
                                break;
                        }
                    }
                    break;
                case GamePhase.Feat:
                    if (CB.CurrentActiveCard.CardType != Card.Type.Feat)
                    {
                        NullZoomButtons();
                    }
                    else
                    {
                        gCardSelect.SetActive(false);
                        gCardCollect.SetActive(false);
                        gCardPlay.SetActive(true);
                    }
                    break;
                case GamePhase.PostAction:
                    NullZoomButtons();
                    break;
            }
        }
        else
        {
            NullZoomButtons();
        }
    }

    private void NullZoomButtons()
    {
        gCardSelect.SetActive(false);
        gCardCollect.SetActive(false);
        gCardPlay.SetActive(false);
    }

    public void TurnOnPersonalDeckVisual()
    {
        DrawDeckButton.SetActive(true);
    }
    #endregion

    #region Selections
    private void HandleCardSelecion(CardData card)
    {
        if (!CB.SpecificDraw)
        {
            switch (myPhase)
            {
                case GamePhase.HeroDraft:
                    if (!zoomed)
                    {
                        CardZoom(card);
                    }
                    break;
                case GamePhase.AbilityDraft:
                    if (!zoomed)
                    {
                        CardZoom(card);
                    }
                    break;
                case GamePhase.HEROSelect:
                    if (!zoomed)
                    {
                        CardZoom(card);
                    }
                    break;
                case GamePhase.Heal:
                    if (abilityTargetting == false && card.Exhausted)
                    {
                        card.Heal(false);
                        return;
                    }
                    //HandleAbilityTargetting(card);
                    break;
                case GamePhase.Enhance:
                    if (abilityTargetting == false && !zoomed)
                    {
                        CardZoom(card);
                        return;
                    }
                    //HandleAbilityTargetting(card);
                    break;
                case GamePhase.Recruit:
                    if (abilityTargetting == false && !zoomed)
                    {
                        CardZoom(card);
                        return;
                    }
                    //HandleAbilityTargetting(card);
                    break;
                case GamePhase.Overcome:
                    if (abilityTargetting == false)
                    {
                        //HandleHeroSelected(card);
                        return;
                    }
                    //HandleAbilityTargetting(card);
                    break;
                case GamePhase.Feat:
                    //HandleAbilityTargetting(card);
                    break;
                case GamePhase.PostAction:
                    if (!zoomed)
                    {
                        CardZoom(card);
                    }
                    break;
                case GamePhase.TurnResponse:
                    if (activeAbility != null)
                    {
                        //HandleAbilityTargetting(card);
                        abilityTargetting = false;
                    }
                    else
                    {
                        Debug.Log($"{activeAbility}");
                        Debug.Log("No active Ability");
                    }
                    break;
                case GamePhase.Wait:
                    if (!zoomed)
                    {
                        CardZoom(card);
                    }
                    break;
            }
        }
        else
        {
            CB.DrawSpecificCard(card.myCard);
            CB.SpecificDraw = false;
        }
    }

    private void HandleDeselection()
    {
        zoomed = false;
        handZoomed = false;
    }

    private void HandlePlayCard(Card card)
    {
        if (myPhase == GamePhase.Enhance)
        {
            iEnhanceCardsToCollect = 0;
            tCardsToDrawMyDeck.text = $"{CB.CardsRemaining(CardDataBase.CardDecks.P1Deck)}";
            bDrawEnhancementCards.interactable = false;
            //TurnCounterDecrement();
        }
        zoomed = false;
        CB.PlayCard(card);
    }
    #endregion
}