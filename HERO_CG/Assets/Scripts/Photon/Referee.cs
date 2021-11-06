﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class Referee : MonoBehaviour
{
    public enum GamePhase { HeroDraft, AbilityDraft, PreSelection, HEROSelect, Heal, Enhance, Recruit, Overcome, Feat, PostAction, TurnResponse, Wait}
    public static GamePhase myPhase = GamePhase.HeroDraft;
    public static GamePhase prevPhase = GamePhase.Wait;
    public enum PlayerNum { P1, P2, P3, P4}
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
    //public GameObject gCardCollect;
    public TMP_Text tCardsToCollectReserve;
    public TMP_Text tCardsToDrawMyDeck;
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
    public GameObject gTurnOnHeroSelection;

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

    public static Action<Card, GamePhase> OnCardCollected = delegate { };
    public static Action<bool> OnOvercomeTime = delegate { };
    public static Action OnOvercomeSwitch = delegate { };
    public static Action<Ability.PassiveType> OnPassiveActivate = delegate { };
    public static Action OnTurnResetabilities = delegate { };

    #region Unity Methods
    private void Awake()
    {
        CardFunction.OnCardSelected += HandleCardSelecion;
        CardFunction.OnCardDeselected += HandleDeselection;
        CardFunction.OnCardCollected += HandleCardCollected;
        CardFunction.OnCardPlayed += HandlePlayCard;
        UIConfirmation.OnHEROSelection += PhaseChange;
        CardDataBase.OnAiDraftCollected += HandleCardCollected;
        Ability.OnAbilityUsed += HandleAbilityEnd;
        Ability.OnFeatComplete += HandleFeatComplete;
        Ability.OnNeedCardDraw += DrawCardOption;
        Ability.OnSetActive += SetActiveAbility;
        Ability.OnActivateable += HandleActivateAbilitySetup;
        Ability.OnHoldTurn += HandleHoldTurn;
        Ability.OnHoldTurnOffOppTurn += HandleHoldTurnOff;
    }

    private void OnDestroy()
    {
        CardFunction.OnCardSelected -= HandleCardSelecion;
        CardFunction.OnCardDeselected -= HandleDeselection;
        CardFunction.OnCardCollected -= HandleCardCollected;
        CardFunction.OnCardPlayed -= HandlePlayCard;
        UIConfirmation.OnHEROSelection -= PhaseChange;
        CardDataBase.OnAiDraftCollected -= HandleCardCollected;
        Ability.OnAbilityUsed -= HandleAbilityEnd;
        Ability.OnFeatComplete -= HandleFeatComplete;
        Ability.OnNeedCardDraw -= DrawCardOption;
        Ability.OnSetActive -= SetActiveAbility;
        Ability.OnActivateable -= HandleActivateAbilitySetup;
        Ability.OnHoldTurn -= HandleHoldTurn;
        Ability.OnHoldTurnOffOppTurn -= HandleHoldTurnOff;
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
            if(turnStart == 1)
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

    #region Base Methods
    public void OnBaseDestroyed(PlayerBase.Type pBase)
    {
        if(pBase == PlayerBase.Type.Player)
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

    public void OnBaseExhausted(PlayerBase.Type pBase)
    {
        if (pBase == PlayerBase.Type.Player)
        {
            PB.Exhaust(true);
        }
        else
        {
            MyPB.Exhaust(true);
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

    #region Ability Methods
    public void PassiveActivate(Ability.PassiveType passiveType)
    {
        if(myPhase == GamePhase.HeroDraft || myPhase == GamePhase.AbilityDraft)
        {
            return;
        }
        OnPassiveActivate?.Invoke(passiveType);
    }

    public bool canPlayAbilitiesToFieldCheck()
    {
        return canPlayAbilityToField;
    }

    public void SetActiveAbility(Ability ability)
    {
        Debug.Log($"Active ability to be set: {ability.Name}");
        activeAbility = ability;
        StartCoroutine(PhaseDeclaration($"{ability.Name} ability Activated"));
        ability.AbilityAwake();
    }

    public void SilenceAbilityToField(int turns)
    {
        abilityPlaySilenceTurnTimer = turns;
        canPlayAbilityToField = false;
    }
    #endregion

    #region Overcome Methods
    public void CalculateBattle()
    {
        if (AttackingHeros.Count > 0 && DefendingHero != null)
        {
            PreviousAttackers.Clear();
            PreviousDefender = null;
            foreach(CardData card in AttackingHeros)
            {
                PreviousAttackers.Add(card);
            }
            PreviousDefender = DefendingHero;

            int tDmg = 0;
            foreach (CardData data in AttackingHeros)
            {
                Debug.Log($"{data.Name} was an attacking hero");
                tDmg += data.Attack;
                data.Exhaust(false);
            }

            DefendingHero.DamageCheck(tDmg);
            if (DefendingHero != null)
            {
                OpponentExhausted = DefendingHero.Exhausted;
                Debug.Log($"Opponent exhaust status = {OpponentExhausted}");
            }
            else
            {
                OpponentExhausted = true;
                Debug.Log($"Opponent should have been destroyed so exhaust has been set to true");
            }

            PassiveActivate(Ability.PassiveType.BattleComplete);
            CB.SendPreviousAttackersAndDefender(AttackingHeros, DefendingHero);
            AttackingHeros.Clear();
            DefendingHero = null;
            SwitchAttDef();
        }
    }

    public void SwitchAttDef()
    {
        AttDef = !AttDef;
        if (AttDef)
        {
            OnPassiveActivate?.Invoke(Ability.PassiveType.BattleStart);
        }
        if (!AttDef && !CB.CheckFieldForOpponents() && AttackingHeros.Count > 0)
        {
            //Target base
            int tDmg = 0;
            foreach (CardData data in AttackingHeros)
            {
                tDmg += data.Attack;
                data.Exhaust(false);
            }
            AttackingHeros.Clear();

            PB.Damage(tDmg);

            SwitchAttDef();
        }
        else if(AttDef && !CB.CheckMyFieldForUsableHeros())
        {
            //check if all characters are exhausted, if they are, end turn
                        StartCoroutine(EndturnDelay());
            PassiveActivate(Ability.PassiveType.ActionComplete);
            return;
        }
        OnOvercomeSwitch?.Invoke();
    }
    #endregion
    
    #region Turn Methods

    #region Move Counter Methods
    public int GetTurnCounter()
    {
        return iTurnCounter;
    }

    public void TurnCounterDecrement()
    {
        iTurnCounter--;
        //TurnActionIndicator.text = $"Actions Remaining: {iTurnCounter}";
        if (iTurnCounter > 0)
        {
            if (myPhase == GamePhase.Recruit)
            {
                tCardsToCollectReserve.text = $"{iTurnCounter}/{CB.CardsRemaining(CardDataBase.CardDecks.Reserve)}";
            }
            if (myPhase == GamePhase.Enhance)
            {
                tCardsToDrawMyDeck.text = $"{iTurnCounter}/{CB.CardsRemaining(CardDataBase.CardDecks.P1Deck)}";
            }
            /*if(myPhase == GamePhase.Recruit)
            {
                CB.FillHQ();
            }
            PassiveActivate(Ability.PassiveType.ActionComplete);
                        StartCoroutine(EndturnDelay());
            HandleTurnDeclaration(!myTurn);*/
        }
        else
        {
            tCardsToCollectReserve.text = $"{CB.CardsRemaining(CardDataBase.CardDecks.Reserve)}";

        }
    }

    private void HandleTurnDeclaration(bool myNewTurn)
    {
        myManager.RPCRequest("DeclaredTurn", RpcTarget.Others, myNewTurn);
    }

    public void SetTurnGauge(int newNum)
    {
        iTurnGauge = newNum;
    }
    #endregion

    public void PopUpUpdater(string message)
    {
        StartCoroutine(PhaseDeclaration(message));
    }

    public void EndTurn()
    {
        bEndTurn = true;
        if (myPhase == GamePhase.Recruit)
        {
            CB.FillHQ();
        }
        StartCoroutine(EndturnDelay());
        //TurnActionIndicator.text = "0";
    }

    public void ToldSwitchTurn(bool turn)
    {
        myTurn = turn;
        StartCoroutine(TurnDeclaration(myTurn));
        NextPhase();
        OnTurnResetabilities?.Invoke();
    }

    private void SwitchTurn()
    {
        if (myTurn)
        {
            abilityPlaySilenceTurnTimer--;
            if(abilityPlaySilenceTurnTimer <= 0)
            {
                canPlayAbilityToField = true;
            }
        }
        myTurn = !myTurn;
        SwitchEndTurnButtonInteractible(myTurn);
        StartCoroutine(TurnDeclaration(myTurn));
        NextPhase();
        HandleTurnDeclaration(!myTurn);
        OnTurnResetabilities?.Invoke();
    }

    private void SwitchEndTurnButtonInteractible(bool interactible)
    {
        btEndTurn.interactable = interactible;
    }

    public void HandleHoldTurn(bool hold, bool myTurn)
    {
        if (myPhase == GamePhase.AbilityDraft || myPhase == GamePhase.HeroDraft)
            return;

        if (!myTurn)
            PhaseChange(GamePhase.Wait);

        bEndTurn = !hold;
    }

    private void HandleHoldTurnOff() => myManager.RPCRequest("HandleTurnOffTurnHold", RpcTarget.All, true);

    #region Phase State Adjustments
    private void HEROSelectionBegin()
    {
        gHEROSelect.SetActive(true);
    }

    public void ToHero()
    {
        PhaseChange(GamePhase.HEROSelect);
    }

    public void PhaseChange(GamePhase phaseToChangeTo)
    {
        Debug.Log($"{player}: Changing Phase to {phaseToChangeTo} from {myPhase}");
        if(myPhase == GamePhase.Overcome)
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
            case GamePhase.PreSelection:
                //This is for actions that happen before selecting a turn action, like looking at your hand and the board
                PhaseIndicator.text = "Pre Selection";
                SetDeckNumberAmounts();
                gTurnOnHeroSelection.SetActive(true);
                break;
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
                break;
            case GamePhase.Wait:
                StartCoroutine(PhaseDeclaration("Wait for Opponent"));
                PhaseIndicator.text = "Wait";
                break;
            case GamePhase.HEROSelect:
                PhaseIndicator.text = "Hero Selection";
                HEROSelectionBegin();
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
                if(iTurnGauge >= 9)
                PhaseChange(GamePhase.PreSelection);
                break;
        }
    }
    #endregion

    #endregion

    #region Card Methods
    public void DrawCardOption(int amount)
    {
        int i = CB.CardsRemaining(CardDataBase.CardDecks.P1Deck);
        if (i < amount)
        {
            amount = i;
        }
    }

    public void SetDeckNumberAmounts()
    {
        tCardsToCollectReserve.text = $"{CB.CardsRemaining(CardDataBase.CardDecks.Reserve)}";
        tCardsToDrawMyDeck.text = $"{CB.CardsRemaining(CardDataBase.CardDecks.P1Deck)}";
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

    public void HandCardZoom()
    {
        handZoomed = true;
        gCardZoom.SetActive(true);
        gCard.CardOverride(CB.CurrentActiveCard, CardData.FieldPlacement.Zoom);
        HandleCardButtons(CardData.FieldPlacement.Hand);
        if(CB.CurrentActiveCard.CardType == Card.Type.Feat && myPhase != GamePhase.Feat)
        {
            NullZoomButtons();
        }
        ClearAbilityPanel();
    }

    public void CheckHandZoomInEffect()
    {
       gCard.CardOverride(CB.CurrentActiveCard, CardData.FieldPlacement.Hand);
       HandleCardButtons(CardData.FieldPlacement.Hand);
        if (CB.CurrentActiveCard.CardType == Card.Type.Feat && myPhase != GamePhase.Feat)
        {
            NullZoomButtons();
        }
        ClearAbilityPanel();
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

    private void HandlePlayCard(Card card)
    {
        CB.PlayCard(card);
    }

    private void HandleCardCollected(Card card)
    {
        zoomed = false;
        CB.HandleCardCollected(card, myPhase);
        if(myPhase != GamePhase.Recruit)
        {
            PassiveActivate(Ability.PassiveType.ActionComplete);
                        StartCoroutine(EndturnDelay());
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
                case GamePhase.PreSelection:
                    if (!zoomed)
                    {
                        CardZoom(card);
                    }
                    break;
                case GamePhase.HEROSelect:
                    break;
                case GamePhase.Heal:
                    if (abilityTargetting == false && card.Exhausted)
                    {
                        card.Heal(false);
                            return;
                    }
                        HandleAbilityTargetting(card);
                    break;
                case GamePhase.Enhance:
                    if (abilityTargetting == false && !zoomed)
                    {
                        CardZoom(card);
                            return;
                    }
                        HandleAbilityTargetting(card);
                    break;
                case GamePhase.Recruit:
                    if (abilityTargetting == false && !zoomed)
                    {
                        CardZoom(card);
                            return;
                    }
                        HandleAbilityTargetting(card);
                    break;
                case GamePhase.Overcome:
                    if(abilityTargetting == false)
                        {
                            HandleHeroSelected(card);
                            return;
                        }
                        HandleAbilityTargetting(card);
                    break;
                case GamePhase.Feat:
                    HandleAbilityTargetting(card);
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
                            HandleAbilityTargetting(card);
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
                    card.OvercomeTarget(false);
                }
                else
                {
                    Debug.Log($"Adding {card.Name} to Attacking.");
                    //Target Card
                    AttackingHeros.Add(card);
                    card.OvercomeTarget(true);
                }
            }
        }
        else
        {
            if (!AttDef)
            {
                if (DefendingHero == card)
                {
                    Debug.Log($"Removing {card.Name} from Defending.");
                    //Untarget Card
                    DefendingHero = null;
                    card.OvercomeTarget(false);
                }
                else
                {
                    Debug.Log($"Adding {card.Name} to Defending.");
                    //Target Card
                    if (DefendingHero != null)
                    {
                        DefendingHero.OvercomeTarget(false);
                    }
                    DefendingHero = card;
                    //turn on interactible for calculate
                    card.OvercomeTarget(true);
                }
            }
        }
    }

    private void HandleDeselection()
    {
        zoomed = false;
        handZoomed = false;
    }
    #endregion

    #region Ability Functions + Feat
    private void HandleAbilityTargetting(CardData card)
    {
        if(activeAbility != null && card.CardType == Card.Type.Character)
        {
            activeAbility.Target(card);
            abilityTargetting = true;
        }
    }

    private void HandleActivateAbilitySetup(Ability ability)
    {
        if(myPhase == GamePhase.AbilityDraft || myPhase == GamePhase.HeroDraft)
        {
            return;
        }
        activeAbility = ability;
        StartCoroutine(PhaseDeclaration("Activate Ability?"));
        ability.AbilityAwake();
    }

    private void HandleAbilityEnd()
    {
        Debug.Log("Ability has ended.");
        if(activeAbility.myType == Ability.Type.Feat)
        {
            PassiveActivate(Ability.PassiveType.ActionComplete);
                        StartCoroutine(EndturnDelay());
        }
        activeAbility = null;
        abilityTargetting = false;
    }

    private void GetNewAbilities(CardData card)
    {
        if(card.charAbility != null)
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

    private void ClearAbilityPanel()
    {
        if (gAbilities != null)
        {
            for(int i = gAbilities.Count-1; i > -1; i--)
            {
                GameObject obj = gAbilities[i]; 
                gAbilities.Remove(obj);
                Destroy(obj);
            }
        }
    }

    private void HandleFeatComplete()
    {
        PassiveActivate(Ability.PassiveType.ActionComplete);
                    StartCoroutine(EndturnDelay());
    }
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
                    //gCardCollect.SetActive(true);
                    gCardPlay.SetActive(false);
                    break;
                case GamePhase.HeroDraft:
                    gCardSelect.SetActive(false);
                    //gCardCollect.SetActive(true);
                    gCardPlay.SetActive(false);
                    break;
                case GamePhase.PreSelection:
                    NullZoomButtons();
                    break;
                case GamePhase.Heal:
                    gCardSelect.SetActive(true);
                    //gCardCollect.SetActive(false);
                    gCardPlay.SetActive(false);
                    break;
                case GamePhase.Enhance:
                    switch (placement)
                    {
                        case CardData.FieldPlacement.Hand:
                            gCardSelect.SetActive(false);
                            //gCardCollect.SetActive(false);
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
                            //gCardCollect.SetActive(true);
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
                                //gCardCollect.SetActive(false);
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
                                //gCardCollect.SetActive(false);
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
                        //gCardCollect.SetActive(false);
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
            switch (myPhase)
            {
                case GamePhase.AbilityDraft:
                    NullZoomButtons();
                    break;
                case GamePhase.HeroDraft:
                    NullZoomButtons();
                    break;
                case GamePhase.TurnResponse:
                    NullZoomButtons();
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
        //gCardCollect.SetActive(false);
        gCardPlay.SetActive(false);
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

    private IEnumerator EndturnDelay()
    {
        yield return new WaitForSeconds(1f);
        if (bEndTurn)
        {
            SwitchTurn();
        }
        else
        {
            StartCoroutine(EndturnDelay());
        }
    }
    #endregion
}