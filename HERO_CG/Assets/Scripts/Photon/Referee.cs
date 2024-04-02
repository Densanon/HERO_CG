//Created by Jordan Ezell
//Last Edited: 6/30/23 Jordan

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class Referee : MonoBehaviour
{
    public enum GamePhase { HeroDraft, AbilityDraft, HEROSelect, Heal, Enhance, Recruit, Overcome, Feat, PostAction, TurnResponse, Wait, Targeting, CombatAbility }
    public static GamePhase myPhase = GamePhase.HeroDraft;
    public static GamePhase prevPhase = GamePhase.Wait;

    public enum GameState { PreAction, Action, PostAction, OpponentTurn}
    public static GameState myState = GameState.PreAction;

    public enum PlayerNum { P1, P2, P3, P4 }
    public static PlayerNum player;

    public enum TargetType { Player, OppHero, MyHero, Hero, Attackers, Defender, Cancel, MyHurt}

    Stack<GameAction> gameActions;

    public CardDataBase CB;
    public PhotonInGameManager myManager;

    public Image gameStateIndicator;
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

    public GameObject ResponsePanel;
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

    public static Action<bool> OnOvercomeTime = delegate { };
    public static Action OnOvercomeSwitch = delegate { };
    public static Action<Ability.PassiveType> OnPassiveActivate = delegate { };
    public static Action<bool> OnWaitTimer = delegate { };
    public static Action OnPlayerResponded = delegate { };
    public static Action<TargetType> OnDsiplayTargets = delegate { };
    public static Action OnTurnResetables = delegate { };
    public static Action OnRemoveTargeting = delegate { };
    public static Action<string> OnAbilityComplete = delegate { };
    public static Action<bool> OnTurnWaitResponse = delegate { };

    #region Unity Methods
    private void Awake()
    {
        CardFunction.OnCardSelected += HandleCardSelecion;
        CardFunction.OnCardDeselected += HandleDeselection;
        CardFunction.OnCardCollected += HandleCardCollected;
        CardFunction.OnCardPlayed += HandlePlayCard;
        UIConfirmation.OnHEROSelection += PhaseChange;
        CardDataBase.OnAiDraftCollected += HandleCardCollected;
        UIResponseTimer.OnTimerRunOut += HandleResponsePanelTurnOff;
        Ability.OnHoldTurn += HandleHoldTurn;
        Ability.OnRequestTargeting += HandleTargeting;
        Ability.OnActivateable += SetActiveAbility;
        Ability.OnAbilityUsed += HandleAbilityEnd;
        UIAbilityDescriptor.OnActivateAbility += SetActiveAbility;
        /*Ability.OnFeatComplete += HandleFeatComplete;
        Ability.OnHoldTurnOffOppTurn += HandleHoldTurnOff;*/
        Ability.OnActivateKayAbility += HandleKayPlayCardAbility;
        Ability.OnCheckNeedResponse += HandleCheckNeedResponse;
        UIConfirmation.OnRohanRecruitment += HandleRohanRecruitment;
    }
    private void OnDestroy()
    {
        CardFunction.OnCardSelected -= HandleCardSelecion;
        CardFunction.OnCardDeselected -= HandleDeselection;
        CardFunction.OnCardCollected -= HandleCardCollected;
        CardFunction.OnCardPlayed -= HandlePlayCard;
        UIConfirmation.OnHEROSelection -= PhaseChange;
        CardDataBase.OnAiDraftCollected -= HandleCardCollected;
        UIResponseTimer.OnTimerRunOut -= HandleResponsePanelTurnOff;
        Ability.OnHoldTurn -= HandleHoldTurn;
        Ability.OnRequestTargeting -= HandleTargeting;
        Ability.OnActivateable -= SetActiveAbility;
        Ability.OnAbilityUsed -= HandleAbilityEnd;
        UIAbilityDescriptor.OnActivateAbility -= SetActiveAbility;
        /*Ability.OnFeatComplete -= HandleFeatComplete;
        Ability.OnHoldTurnOffOppTurn -= HandleHoldTurnOff;*/
        Ability.OnActivateKayAbility -= HandleKayPlayCardAbility;
        Ability.OnCheckNeedResponse -= HandleCheckNeedResponse;
        UIConfirmation.OnRohanRecruitment -= HandleRohanRecruitment;
    }
    private void Start()
    {
        gameActions = new Stack<GameAction>();
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

    #region Game Actions
    void ProgressGameState()
    {
        Debug.Log($"Change in game state from: {myState}");
        switch (myState)
        {
            case GameState.PreAction:
                gameStateIndicator.color = Color.red;
                myState = GameState.Action;
                break;
            case GameState.Action:
                gameStateIndicator.color = Color.black;
                myState = GameState.PostAction;
                ActivatePassive(Ability.PassiveType.ActionComplete);
                break;
            case GameState.PostAction:
                gameStateIndicator.color = Color.grey;
                myState = GameState.OpponentTurn;
                break;
            case GameState.OpponentTurn:
                gameStateIndicator.color = Color.white;
                myState = GameState.PreAction;
                break;
        }
        Debug.Log($"To: {myState}");
    }
    public void AddActionToStack(GameAction act)
    {
        gameActions.Push(act);
    }
    public void StartStack()
    {
        int i = gameActions.Count;
        for(int j = 0; j<i; j++)
        {
            ExecuteAction(gameActions.Pop());
        }
    }
    void ExecuteAction(GameAction act)
    {
        switch (act.myType)
        {
            case GameAction.Type.ActivatedAbility:
                break;
            case GameAction.Type.Battle:
                break;
            case GameAction.Type.PassiveAbility:
                break;
            case GameAction.Type.Target:
                break;
        }
    }
    #endregion

    #region Ability Methods
    public void ActivatePassive(Ability.PassiveType type)
    {
        Debug.Log($"Starting a Passive: {type}");
        OnPassiveActivate?.Invoke(type);
    }
    public void SetActiveAbility(Ability ability)
    {
        if (canPlayAbilityToField)
        {
            activeAbility = ability;
            ability.AbilityAwake();
        }
    }
    private void HandleAbilityEnd()
    {
        activeAbility = null;
        PhaseChange(prevPhase);
        OnRemoveTargeting?.Invoke();
    }
    public void SilenceAbilityToField(int turns)
    {
        Debug.Log($"Abilities have been silenced for {turns} turns.");
        abilityPlaySilenceTurnTimer = turns;
        canPlayAbilityToField = false;
    }
    public void ToggleAbilityActivateInCombat()
    {
        if (myPhase == GamePhase.Overcome) PhaseChange(GamePhase.CombatAbility);
        else if (myPhase == GamePhase.CombatAbility) PhaseChange(GamePhase.Overcome);
    }

    public bool Rohan = false;
    private void HandleRohanRecruitment()
    {
        iTurnCounter = CardDataBase.herosFatigued/2;
        Rohan = true;
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
    public void HandleHoldTurn(bool hold)
    {
        btEndTurn.gameObject.SetActive(!hold);
        bEndTurn = !hold;
    }
    #region Switching Turn Methods
    private void SwitchTurn()
    {
        if (bEndTurn)
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
            HandleTurnDeclaration(!myTurn);
        }
    }
    public void ToldSwitchTurn(bool turn)
    {
        if (bEndTurn)
        {
            myTurn = turn;
            if(myPhase != GamePhase.AbilityDraft && myPhase != GamePhase.HeroDraft)GenericTurnChangables();
            NextPhase();
        }      
    }
    /*public void EndTurnFromButton()
    {
        Debug.Log("Are we seeing this?");
        if(myState != GameState.PostAction)
        {
            StartCoroutine(TurnEndCycle());
            return;
        }
        bEndTurn = true;

        if (CB.GetHQCount() < 3)
        {
            CB.FillHQ();
        }

        abilityPlaySilenceTurnTimer--;
        if (abilityPlaySilenceTurnTimer <= 0)
        {
            canPlayAbilityToField = true;
        }

        myTurn = false;
        GenericTurnChangables();
        PhaseChange(GamePhase.Wait);
        HandleTurnDeclaration(true);
    }
    private IEnumerator TurnEndCycle()
    {
        Debug.Log("Are we doing this?");
        yield return new WaitForSeconds(1f);
        ProgressGameState();
        EndTurnFromButton();
    }*/
    private void GenericTurnChangables()
    {
        btEndTurn.gameObject.SetActive(myTurn);
        StartCoroutine(TurnDeclaration(myTurn));
        OnTurnResetables?.Invoke();
        if (CB.GetHQCount() < 3) CB.FillHQ();
    }

    private string ResponseType;
    private void HandleCheckNeedResponse(string obj)
    {
        ResponseType = obj;
        if (obj == "Origin" && DefendingHero.Name == "ORIGIN")
        {
            PopUpUpdater("Waiting on a response.");
            gOvercome.SetActive(false);
            OnTurnWaitResponse?.Invoke(true);
            myManager.RPCRequest("HandleOriginRequest", RpcTarget.Others, "Origin");
        }
    }
    public void RecieveResponse(bool decide)
    {
        if (ResponseType == "Origin")
        {
            gOvercome.SetActive(true);
            aOrigin.blockActive = decide;
            OnAbilityComplete("ORIGIN");
        }
    }
    #endregion

    public void Respond(bool response)//Activated by UI
    {
        if (!response)
        {
            OnPlayerResponded?.Invoke();
            myManager.RPCRequest("HandlePlayerResponded", RpcTarget.Others, response);
            NextPhase();
        }
    }
    public void AwatePlayerResponse(bool wait)
    {
        if (wait)
        {
            Debug.Log("Freezing Zoom so player can't interact with anything while waiting.");
            zoomed = true;
            return;
        }
        Debug.Log("Unfreezing zoom so the player can interact again.");
        zoomed = false;
        
    }
    private void HandleResponsePanelTurnOff()
    {
        ResponsePanel.SetActive(false);
        NextPhase();
    }
    public int GetTurnCounter()
    {
        return iTurnCounter;
    }
    public void TurnCounterDecrement()
    {
        iTurnCounter--;
        PopUpUpdater($"{iTurnCounter} moves left.");
        if (iTurnCounter > 0)
        {
            if (myPhase == GamePhase.Recruit)
            {
                tCardsToCollectReserve.text = $"{iTurnCounter}/{CB.CardsRemaining(CardDataBase.CardDecks.Reserve)}";
            }
            if (myPhase == GamePhase.Enhance)
            {
                tCardsToDrawMyDeck.text = $"{iEnhanceCardsToCollect}/{CB.CardsRemaining(CardDataBase.CardDecks.MyDeck)}";
            }

            //ActivatePassive(Ability.PassiveType.ActionComplete);
        }
        else
        {
            if (Rohan) Rohan = false;
            tCardsToCollectReserve.text = $"{CB.CardsRemaining(CardDataBase.CardDecks.Reserve)}";
            PopUpUpdater("No More Actions");
            NullZoomButtons();
        }
    }
    public void PopUpUpdater(string message)
    {
        StartCoroutine(PhaseDeclaration(message));
    }
    public void PopUpUpdater(string message, float time)
    {
        StartCoroutine(PhaseDeclaration(message, time));
    }

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

    private IEnumerator PhaseDeclaration(string phase, float time)
    {
        PhaseText.text = phase;
        PhaseDeclarationUI.SetActive(true);
        yield return new WaitForSeconds(time);
        PhaseDeclarationUI.SetActive(false);
    }

    private void HandleTurnDeclaration(bool myNewTurn)
    {
        myManager.RPCRequest("DeclaredTurn", RpcTarget.Others, myNewTurn);
    }
    #endregion

    #region Phase Methods

    public void NextPhase()
    {
        switch (myPhase)
        {
            case GamePhase.HEROSelect:
                PhaseChange(GamePhase.PostAction);
                break;
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
                SwitchTurn();
                PhaseChange(GamePhase.Wait);
                break;
            case GamePhase.TurnResponse:
                PhaseChange(GamePhase.Wait);
                break;
            case GamePhase.Wait:
                PhaseChange(GamePhase.HEROSelect);
                break;
        }
    }

    public void PhaseChange(GamePhase phaseToChangeTo)
    {
        if (phaseToChangeTo == myPhase) return;
        Debug.Log($"{player}: Changing Phase to {phaseToChangeTo} from {myPhase}");
        if (myPhase == GamePhase.Overcome && phaseToChangeTo != GamePhase.CombatAbility)
        {
            CB.ResetDiscardRecord();
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
                if(prevPhase == GamePhase.HEROSelect) ProgressGameState();
                //Select 1+ heros to be healed(any)
                PhaseIndicator.text = "Heal";
                gHEROSelect.SetActive(false);
                StartCoroutine(PhaseDeclaration("Heal Heros"));
                break;
            case GamePhase.Enhance:
                if (prevPhase == GamePhase.HEROSelect) ProgressGameState();
                //Ask quantity to draw
                //Draw up to 3 cards from enhance deck
                //Play up to 3 cards from your hand
                StartCoroutine(PhaseDeclaration("Draw and Play Cards"));
                PhaseIndicator.text = "Enhance";
                gHEROSelect.SetActive(false);
                bDrawEnhancementCards.interactable = true;
                iTurnCounter = 3;
                iEnhanceCardsToCollect = 3;
                int temp = CB.CardsRemaining(CardDataBase.CardDecks.MyDeck);
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
                break;
            case GamePhase.Recruit:
                if (prevPhase == GamePhase.HEROSelect) ProgressGameState();
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
                if (prevPhase == GamePhase.HEROSelect) ProgressGameState();
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
                if (prevPhase == GamePhase.HEROSelect) ProgressGameState();
                PhaseIndicator.text = "Feat";
                StartCoroutine(PhaseDeclaration("Pick Your Feat"));
                gHEROSelect.SetActive(false);
                //Resolve card
                break;
            case GamePhase.PostAction:
                if (myState == GameState.Action) ProgressGameState(); 
                StartCoroutine(PhaseDeclaration("Ending Phase"));
                PhaseIndicator.text = "Post Action";
                break;
            case GamePhase.TurnResponse:
                StartCoroutine(PhaseDeclaration("Player Response"));
                PhaseIndicator.text = "Turn Response";
                //OnWaitTimer?.Invoke(true);
                ResponsePanel.SetActive(true);
                break;
            case GamePhase.Wait:
                if (prevPhase == GamePhase.PostAction) ProgressGameState();
                StartCoroutine(PhaseDeclaration("Wait for Opponent"));
                PhaseIndicator.text = "Wait";
                break;
            case GamePhase.HEROSelect:
                if (prevPhase == GamePhase.Wait) ProgressGameState();
                PhaseIndicator.text = "Hero Selection";
                ActivatePassive(Ability.PassiveType.TurnStart);
                SetDeckNumberAmounts();
                gHEROSelect.SetActive(true);
                //check for heros that can be healed
                bHEROSelectHeal.interactable = CB.CheckForHealableHeros();
                //check for cards in enhancement deck or hand
                bHEROSelectEnhance.interactable = (CB.CardsRemaining(CardDataBase.CardDecks.MyDeck) + CB.CardsRemaining(CardDataBase.CardDecks.MyHand) > 0);
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

    #region Overcome Methods
    public void SwitchAttDef()
    {
        AttDef = !AttDef;
        if (!AttDef && !CB.CheckFieldForOpponents() && AttackingHeros.Count > 0)
        {
            //Target the player base
            int tDmg = 0;
            foreach (CardData data in AttackingHeros)
            {
                tDmg += data.Attack;
                data.Exhaust(false);
            }
            AttackingHeros.Clear();

            if (aMace.maceDoubleActive && !aMace.used)
            {
                Debug.Log("Mace damage was ddoubled here.");
                OnAbilityComplete("MACE");
                PB.Damage(tDmg * 2);
            }
            else PB.Damage(tDmg);

            SwitchAttDef();
        }
        OnOvercomeSwitch?.Invoke();
    }

    public void CalculateBattle()
    {
        //bAwaitingResponse = true;
        //StartCoroutine(WaitResponse(GamePhase.Overcome));
        ActualCalculateBattle();
    }

    private void ActualCalculateBattle()
    {
        Debug.Log("Battle initiated.");
        if (AttackingHeros.Count > 0 && DefendingHero != null)
        {
            PreviousAttackers.Clear();
            PreviousDefender = null;

            int tDmg = 0;
            foreach (CardData card in AttackingHeros)
            {
                PreviousAttackers.Add(card);
                Debug.Log($"{card.Name} was an attacking hero");
                tDmg += card.Attack;
                card.Exhaust(false);
                card.ParticleStop();
            }
            PreviousDefender = DefendingHero;
            CB.SendPreviousAttackersAndDefender(AttackingHeros, DefendingHero);

            Debug.Log($"{tDmg} was the given damage to take.");
            DefendingHero.ParticleStop();
            if (aMace.maceDoubleActive && !aMace.used)
            {
                Debug.Log("Mace damage was doubled.");
                OnAbilityComplete?.Invoke("MACE");
                DefendingHero.DamageCheck(tDmg * 2);
            }else if (aOrigin.blockActive)
            {
                Debug.Log("Origin ability block");
                DefendingHero.DamageCheck(0);
                gOvercome.SetActive(true);
            }
            else DefendingHero.DamageCheck(tDmg);

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
            ActivatePassive(Ability.PassiveType.BattleComplete);
            AttackingHeros.Clear();
            DefendingHero = null;
            SwitchAttDef();
        }
    }

    private void HandleHeroSelected(CardData card)
    {
        ///WE NEED TO SEND OVER WHO IS GETTING SELECTED AND SHOW IT ON THE OTHER PLAYERS SCREEN SO THEY KNOW HOW THEY WANT TO RESPOND, STILL NEED TO ALLOW RESPONSE BEFORE WE CONCLUDE
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
                    card.ParticleStop();
                }
                else
                {
                    Debug.Log($"Adding {card.Name} to Attacking.");
                    //Target Card
                    AttackingHeros.Add(card);
                    card.OvercomeTarget(true);
                    card.ParticleSetAndStart(Color.blue);
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
                    card.ParticleStop();
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
                    ActivatePassive(Ability.PassiveType.BattleCalculation);
                    //turn on interactible for calculate
                    card.OvercomeTarget(true);
                    card.ParticleSetAndStart(Color.red);
                }
            }
        }
    }
    #endregion

    #endregion

    #region Card Methods
    private void HandlePlayCard(Card card)
    {
        if (iTurnCounter <= 0 && !AbilityPlayable())//!(aKay.canPlayCard && aKay.abilityActivated))
        {
            zoomed = false;
            Debug.Log("You are trying to play a card while having no moves.");
            return;
        }
        if (myPhase == GamePhase.Enhance)
        {
            iEnhanceCardsToCollect = 0;
            tCardsToDrawMyDeck.text = $"{CB.CardsRemaining(CardDataBase.CardDecks.MyDeck)}";
            bDrawEnhancementCards.interactable = false;
            TurnCounterDecrement();         
        }
        else
        {
            Debug.Log("Referee: HandlePlayCard: Playing from some phase that isn't Enhance.");
        }
        zoomed = false;
        CB.PlayCard(card);
        if (AbilityPlayable()) activeAbility.ActivateAbility();
    }

    private bool AbilityPlayable()
    {
        if(activeAbility != null && !activeAbility.oncePerTurnUsed)
        {
            if (activeAbility.Name == "KAY")
            {
                if (myPhase == GamePhase.Enhance) iTurnCounter++;
                return true;
            }else if(activeAbility.Name == "YASMINE") return true;
            else if(activeAbility.Name == "ZHAO") return true;         
        }
        return false;
    }

    public void SetDeckNumberAmounts()
    {
        tCardsToCollectReserve.text = $"{CB.CardsRemaining(CardDataBase.CardDecks.Reserve)}";
        tCardsToDrawMyDeck.text = $"{CB.CardsRemaining(CardDataBase.CardDecks.MyDeck)}";
    }

    private void CardZoom(CardData card)
    {
        zoomed = true;
        gCardZoom.SetActive(true);
        gCard.CardOverride(card, CardData.FieldPlacement.Zoom);
        HandleCardButtons(card, card.myPlacement);
        ClearAbilityPanel();
        GetNewAbilities(card);
    }
    private void CardZoom(Card card)
    {
        zoomed = true;
        gCardZoom.SetActive(true);
        gCard.CardOverride(card, CardData.FieldPlacement.Zoom);
        NullZoomButtons();
        ClearAbilityPanel();
    }
    public IEnumerator ShowOpponentPlayedCard(CardData card)
    {
        Debug.Log($"ShowOpponentplayedCard(CardData): I received {card} and will zoom into it.");
        CardZoom(card);
        yield return new WaitForSeconds(2f);
        HandleDeselection();
        gCardZoom.SetActive(false);
    }
    public IEnumerator ShowOpponentPlayedCard(Card card)
    {
        Debug.Log($"ShowOpponentplayedCard(Card): I received {card} and will zoom into it.");
        CardZoom(card);
        yield return new WaitForSeconds(2f);
        HandleDeselection();
        gCardZoom.SetActive(false);
    }
    private void HandleCardCollected(Card card)
    {
        zoomed = false;
        CB.HandleCardCollected(card, myPhase);
        if (myPhase == GamePhase.Recruit || Rohan)
        {
            TurnCounterDecrement();
            //PassiveActivate(Ability.PassiveType.ActionComplete);
        }
    }
    public void SetCardCollectAmount(int amount)
    {
        for (int i = amount; i > 0; i--)
        {
            iEnhanceCardsToCollect--;
            CB.DrawCard(CardDataBase.CardDecks.MyDeck);
        }
        //Debug.Log($"Enhance cards to be collected: {iEnhanceCardsToCollect}");

        if (iEnhanceCardsToCollect > 0)
        {
            tCardsToDrawMyDeck.text = $"{iEnhanceCardsToCollect}/{CB.CardsRemaining(CardDataBase.CardDecks.MyDeck)}";
        }
        else
        {
            //Debug.Log("Ran out of cards to draw for the turn.");
            tCardsToDrawMyDeck.text = $"{CB.CardsRemaining(CardDataBase.CardDecks.MyDeck)}";
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
    private void SetCollectButton()
    {
        gCardCollect.SetActive(true);
        gCardPlay.SetActive(false);
        gCardSelect.SetActive(false);
    }
    private void SetPlayButton()
    {
        gCardCollect.SetActive(false);
        gCardPlay.SetActive(true);
        gCardSelect.SetActive(false);
    }
    private void HandleCardButtons(CardData data, CardData.FieldPlacement placement)
    {
        if (myTurn)
        {
            switch (myPhase)
            {
                case GamePhase.HEROSelect:
                    if (AbilityPlayable() && placement == CardData.FieldPlacement.Hand)
                    {
                        SetPlayButton();
                        return;
                    }else if(Rohan && placement == CardData.FieldPlacement.HQ)
                    {
                        SetCollectButton();
                        return;
                    }
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
                    if (AbilityPlayable() && placement == CardData.FieldPlacement.Hand)
                    {
                        SetPlayButton();
                        return;
                    }
                    else if (Rohan && placement == CardData.FieldPlacement.HQ)
                    {
                        SetCollectButton();
                        return;
                    }
                    gCardSelect.SetActive(true);
                    gCardCollect.SetActive(false);
                    gCardPlay.SetActive(false);
                    break;
                case GamePhase.Enhance:
                    if (AbilityPlayable() && placement == CardData.FieldPlacement.Hand)
                    {
                        SetPlayButton();
                        return;
                    }
                    else if (Rohan && placement == CardData.FieldPlacement.HQ)
                    {
                        SetCollectButton();
                        return;
                    }
                    switch (placement)
                    {
                        case CardData.FieldPlacement.Hand:
                            gCardSelect.SetActive(false);
                            gCardCollect.SetActive(false);
                            if((data.CardType == Card.Type.Ability || data.CardType == Card.Type.Enhancement) && CB.CheckForHerosOnField())
                            {
                                gCardPlay.SetActive(true);
                            }
                            else
                            {
                                gCardPlay.SetActive(false);
                            }
                            if(data.CardType == Card.Type.Character)
                            {
                                gCardPlay.SetActive(true);
                            }
                            if(iTurnCounter == 0)
                            {
                                gCardPlay.SetActive(false);
                            }
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
                    if (AbilityPlayable() && placement == CardData.FieldPlacement.Hand)
                    {
                        SetPlayButton();
                        return;
                    }
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
                    if (AbilityPlayable() && placement == CardData.FieldPlacement.Hand)
                    {
                        SetPlayButton();
                        return;
                    }
                    else if (Rohan && placement == CardData.FieldPlacement.HQ)
                    {
                        SetCollectButton();
                        return;
                    }
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
                        SetPlayButton();
                    }
                    break;
                case GamePhase.PostAction:
                    if (AbilityPlayable() && placement == CardData.FieldPlacement.Hand)
                    {
                        SetPlayButton();
                        return;
                    }
                    else if (Rohan && placement == CardData.FieldPlacement.HQ)
                    {
                        SetCollectButton();
                        return;
                    }
                    NullZoomButtons();
                    break;
                case GamePhase.CombatAbility:
                    if (AbilityPlayable() && placement == CardData.FieldPlacement.Hand)
                    {
                        SetPlayButton();
                        return;
                    }
                    NullZoomButtons();
                    break;
            }
        }
        else
        {
            if (AbilityPlayable() && placement == CardData.FieldPlacement.Hand)
            {
                SetPlayButton();
                return;
            }
            NullZoomButtons();
        }
    }

    private void HandleKayPlayCardAbility()
    {
        gCardPlay.SetActive(true); 
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

    public void UpdateDeckCounts()
    {
        tCardsToCollectReserve.text = $"{CB.CardsRemaining(CardDataBase.CardDecks.Reserve)}";
        tCardsToDrawMyDeck.text = $"{CB.CardsRemaining(CardDataBase.CardDecks.MyDeck)}";
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
                    if (aIsaac.IsaacDraw)
                    {
                        CB.DrawSpecificCard(card.myCard, CB.MyDiscard);
                        aIsaac.IsaacDraw = false;
                    }
                    else if (abilityTargetting == false)
                    {
                        HandleHeroSelected(card);
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
                case GamePhase.Wait:
                    if (!zoomed)
                    {
                        CardZoom(card);
                    }
                    else if (aIsaac.IsaacDraw)
                    {
                        CB.DrawSpecificCard(card.myCard, CB.MyDiscard);
                        aIsaac.IsaacDraw = false;
                    }
                    break;
                case GamePhase.Targeting:
                    card.Targeted();
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
    private void HandleTargeting(TargetType obj)
    {
        PhaseChange(GamePhase.Targeting);
    }
    #endregion
}

public class GameAction
{
    public enum Type { Target, Battle, PassiveAbility, ActivatedAbility}
    public Type myType;
    public Card myCard;
}