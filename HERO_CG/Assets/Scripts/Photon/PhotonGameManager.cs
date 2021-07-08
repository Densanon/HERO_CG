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
    public GameObject pAbilityPrefab;
    public Transform tAbilityContainer;
    List<GameObject> gAbilities = new List<GameObject>();
    public GameObject gCardCollect;
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
    private bool bEndTurn = true;
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
    public PlayerBase PB;
    public PlayerBase MyPB;
    public static bool OpponentDestroyed = false;
    public static bool OpponentExhausted = false;

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
        Ability.OnOpponentAbilityActivation += HandleOpponentAbilityHandover;
        Ability.OnActivateable += HandleActivateAbilitySetup;
        Ability.OnHoldTurn += HandleHoldTurn;
        Ability.OnPreventAbilitiesToFieldForTurn += HandleAbilityToFieldSilence;
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
        Ability.OnOpponentAbilityActivation -= HandleOpponentAbilityHandover;
        Ability.OnActivateable -= HandleActivateAbilitySetup;
        Ability.OnHoldTurn -= HandleHoldTurn;
        Ability.OnPreventAbilitiesToFieldForTurn -= HandleAbilityToFieldSilence;

    }

    private void Start()
    {
        PhaseIndicator.text = "Hero Draft";
        tOpponentHandCount.text = "0";
        TurnActionIndicator.text = "1";
        if (PhotonNetwork.IsMasterClient)
        {
            var turnStart = UnityEngine.Random.Range(0, 2);
            if(turnStart == 1)
            {
                myTurn = false;
                player = PlayerNum.P1;
                CB.HandlePlayerDeclaration(0);
                SwitchTurn();
                CB.HandleBuildHeroDraft();
            }
            else
            {
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

    #region Ability Methods
    public void PassiveActivate(Ability.PassiveType passiveType)
    {
        if(myPhase == GamePhase.HeroDraft || myPhase == GamePhase.AbilityDraft)
        {
            return;
        }
        //Debug.Log($"PhotonGameManager calling {passiveType}");
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
        TurnActionIndicator.text = $"Actions Remaining: {iTurnCounter}";
        if(iTurnCounter == 0)
        {
            if(myPhase == GamePhase.Recruit)
            {
                CB.FillHQ();
            }
            PassiveActivate(Ability.PassiveType.ActionComplete);
                        StartCoroutine(EndturnDelay());
            CB.HandleTurnDeclaration(!myTurn);
        }
    }
    #endregion

    public void PopUpUpdater(string message)
    {
        StartCoroutine(PhaseDeclaration(message));
    }

    public void EndTurn()
    {
        bEndTurn = true;
                    StartCoroutine(EndturnDelay());
        TurnActionIndicator.text = "0";
    }

    public void ToldSwitchTurn(bool turn)
    {
        myTurn = turn;
        StartCoroutine(TurnDeclaration(myTurn));
        PhaseAdjustment();
        OnTurnResetabilities?.Invoke();
    }

    public void SetTurnGauge(int newNum)
    {
        iTurnGauge = newNum;
    }

    public void PhaseChange(GamePhase phaseToChangeTo)
    {
        if(myPhase == GamePhase.Overcome)
        {
            gOvercome.SetActive(false);
            OnOvercomeTime?.Invoke(false);
        }
        myPhase = phaseToChangeTo;
        HandlePhaseChange();
    }
    #endregion

    #region Card Methods
    public void DrawCardOption(int amount)
    {
        gCardCountCollect.SetActive(true);

        int i = CB.CardsRemaining(CardDataBase.CardDecks.P1Deck);
        if (i < amount)
        {
            amount = i;
        }

        switch (amount)
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
    }

    public void SetCardCollectAmount(int amount)
    {
        for(int i = amount; i>0; i--)
        {
            CB.DrawCard(CardDataBase.CardDecks.P1Deck);
        }
        if(myPhase == GamePhase.Enhance)
        {
            StartCoroutine(PhaseDeclaration("Play Cards"));
        }
        gCardCountCollect.SetActive(false);
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
    #endregion

    public void SetPlayerNum(PlayerNum playerSet)
    {
        player = playerSet;
    }

    public void SetOpponentHandCount(int number)
    {
        tOpponentHandCount.text = $"{number}";
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void HandleHoldTurn(bool hold)
    {
        if (myPhase == GamePhase.AbilityDraft || myPhase == GamePhase.HeroDraft)
            return;

        bEndTurn = hold ? false : true;
    }
    #endregion

    #region Private Methods
    private void HandleAbilityToFieldSilence()
    {
        if(player == PlayerNum.P1)
        {
            CB.SilenceAbilityToField(PlayerNum.P2, 1);
            return;
        }
        CB.SilenceAbilityToField(PlayerNum.P1, 1);
    }

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
                    /*else if(!zoomed)
                    {
                        Debug.Log("Card Zoomed in TurnResponse");
                        CardZoom(card);
                    }*/
                break;
            case GamePhase.Wait:
                    Debug.Log("Card Zoomed in Wait");
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

    private void HandleAbilityTargetting(CardData card)
    {
        if(activeAbility != null && card.CardType == Card.Type.Character)
        {
            activeAbility.Target(card);
            abilityTargetting = true;
        }
    }

    private void HandleOpponentAbilityHandover(Ability ability)
    {
        Debug.Log($"Handing over control of {ability.Name} to opponent");
        CB.AbilityHandover(ability);
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

    private void HandleFeatComplete()
    {
        PassiveActivate(Ability.PassiveType.ActionComplete);
                    StartCoroutine(EndturnDelay());
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
        ClearAbilityPanel();
        GetNewAbilities(card);
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
                    /*
                    gCardSelect.SetActive(false);
                    gCardCollect.SetActive(false);
                    gCardPlay.SetActive(true);*/
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
        gCardCollect.SetActive(false);
        gCardPlay.SetActive(false);
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
            if(iTurnCounter > 0)
            {
                TurnCounterDecrement();
            }
            else
            {
                PassiveActivate(Ability.PassiveType.ActionComplete);
                            StartCoroutine(EndturnDelay());
            }
        }
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
        StartCoroutine(TurnDeclaration(myTurn));
        PhaseAdjustment();
        CB.HandleTurnDeclaration(!myTurn);
        OnTurnResetabilities?.Invoke();
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
                StartCoroutine(PhaseDeclaration("Heal Heros"));
                break;
            case GamePhase.Enhance:
                //Ask quantity to draw
                //Draw up to 3 cards from enhance deck
                //Play up to 3 cards from your hand
                PhaseIndicator.text = "Enhance";
                gHEROSelect.SetActive(false);
                DrawCardOption(3);
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
                StartCoroutine(PhaseDeclaration("Pick Your Feat"));
                gHEROSelect.SetActive(false);
                //Resolve card
                break;
            case GamePhase.TurnResponse:
                PhaseIndicator.text = "Turn Response";
                gCardCollect.SetActive(true);
                TurnActionIndicator.text = $"Actions Remaining: 0";
                break;
            case GamePhase.Wait:
                PhaseIndicator.text = "Wait";
                gCardCollect.SetActive(false);
                TurnActionIndicator.text = $"Actions Remaining: ~";
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
                OnOvercomeTime?.Invoke(false);
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
