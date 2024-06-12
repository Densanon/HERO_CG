//Created by Jordan Ezell
//Last Edited: 6/12/24 Jordan

using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIConfirmation : MonoBehaviour
{
    private Dictionary<ConfirmationAction.ConfirmationType, Action> confirmationActions =
        new Dictionary<ConfirmationAction.ConfirmationType, Action>();

    private Dictionary<string, Action> confirmationHandlers;

    public GameObject confirmationPanelPrefab;
    private Queue<ConfirmationAction> confirmationQueue = new Queue<ConfirmationAction>();

    private Card myCardToUse;
    private CardData myTargetCard;
    private Ability activeAbility;
    private bool needSendResponse = false;

    public event Action<ConfirmationAction.ConfirmationType> OnActionConfirmation = delegate { };
    public static Action<Referee.GamePhase> OnHEROSelection = delegate { };
    public static Action<CardData, Card> OnTargetAccepted = delegate { };
    public static Action<int> OnNeedDrawEnhanceCards = delegate { };
    public static Action OnNeedDrawFromDiscard = delegate { };
    public static Action OnConfirmIzumiToggle = delegate { };
    public static Action<string> OnAbilityComplete = delegate { };
    public static Action<bool> OnSendAbilityResponse = delegate { };
    public static Action OnRohanRecruitment = delegate { };
    public static Action OnPlayCardFromHand = delegate { };
    public static Action OnActivateTempHealState = delegate { };

    public static Action OnAccelerate = delegate { };
    public static Action OnBackfire = delegate { };
    public static Action OnBoost = delegate { };

    #region Unity Methods
    private void Awake()
    {
        // Initialize the dictionaries with mappings
        initializeConfirmationActions();
        initializeConfirmationHandlers();

        CardDataBase.OnTargeting += HandleTargeting;
        CardDataBase.OnSendYasmineAbilityRequest += onConfirmationRequest;
        CardDataBase.OnZhaoAbilityRequest += onConfirmationRequest;
        CardData.IsTarget += HandleTarget;
        Ability.OnTargetedFrom += HandleActiveHero;
        Ability.OnCharacterAbilityRequest += onConfirmationRequest;
        OnActionConfirmation += Accept;
        PhotonInGameManager.OnOriginRequest += onConfirmationRequest;
    }
    private void OnDestroy()
    {
        CardDataBase.OnTargeting -= HandleTargeting;
        CardDataBase.OnSendYasmineAbilityRequest -= onConfirmationRequest;
        CardDataBase.OnZhaoAbilityRequest -= onConfirmationRequest;
        CardData.IsTarget -= HandleTarget;
        Ability.OnTargetedFrom -= HandleActiveHero;
        Ability.OnCharacterAbilityRequest -= onConfirmationRequest;
        OnActionConfirmation -= Accept;
        PhotonInGameManager.OnOriginRequest -= onConfirmationRequest;
    }
    #endregion

    #region ConfirmationQueue
    private void initializeConfirmationActions()
    {
        // Mapping for each ConfirmationType
        confirmationActions[ConfirmationAction.ConfirmationType.Heal] = () => OnHEROSelection?.Invoke(Referee.GamePhase.Heal);
        confirmationActions[ConfirmationAction.ConfirmationType.Enhance] = () => OnHEROSelection?.Invoke(Referee.GamePhase.Enhance);
        confirmationActions[ConfirmationAction.ConfirmationType.Recruit] = () => OnHEROSelection?.Invoke(Referee.GamePhase.Recruit);
        confirmationActions[ConfirmationAction.ConfirmationType.Overcome] = () => OnHEROSelection?.Invoke(Referee.GamePhase.Overcome);
        confirmationActions[ConfirmationAction.ConfirmationType.Feat] = () => OnHEROSelection?.Invoke(Referee.GamePhase.Feat);
        confirmationActions[ConfirmationAction.ConfirmationType.Quit] = () => { /* Handle Quit action */ };

        confirmationActions[ConfirmationAction.ConfirmationType.Ability] = handleAbilityConfirmation;
        confirmationActions[ConfirmationAction.ConfirmationType.Enhancing] = handleAbilityConfirmation;

        confirmationActions[ConfirmationAction.ConfirmationType.Ayumi] = () => OnNeedDrawEnhanceCards?.Invoke(1);
        confirmationActions[ConfirmationAction.ConfirmationType.Isaac] = () => { aIsaac.IsaacDraw = true; OnNeedDrawFromDiscard?.Invoke(); };
        confirmationActions[ConfirmationAction.ConfirmationType.Izumi] = () => OnConfirmIzumiToggle?.Invoke();
        confirmationActions[ConfirmationAction.ConfirmationType.Mace] = () => aMace.maceDoubleActive = true;
        confirmationActions[ConfirmationAction.ConfirmationType.Michael] = () => { OnNeedDrawEnhanceCards?.Invoke(1); OnAbilityComplete?.Invoke("MICHAEL"); };
        confirmationActions[ConfirmationAction.ConfirmationType.Origin] = () => OnSendAbilityResponse?.Invoke(true);
        confirmationActions[ConfirmationAction.ConfirmationType.Rohan] = () => { OnRohanRecruitment?.Invoke(); OnAbilityComplete?.Invoke("ROHAN"); };
        confirmationActions[ConfirmationAction.ConfirmationType.Yasmine] = () => OnPlayCardFromHand?.Invoke();
        confirmationActions[ConfirmationAction.ConfirmationType.Zhao] = () => OnPlayCardFromHand?.Invoke();
        confirmationActions[ConfirmationAction.ConfirmationType.Zoe] = () => OnActivateTempHealState?.Invoke();

        confirmationActions[ConfirmationAction.ConfirmationType.Accelerate] = () => { OnAccelerate?.Invoke(); OnAbilityComplete?.Invoke("ACCELERATE"); };
        confirmationActions[ConfirmationAction.ConfirmationType.Backfire] = () => { OnBackfire?.Invoke(); OnAbilityComplete?.Invoke("BACKFIRE"); };
        confirmationActions[ConfirmationAction.ConfirmationType.Boost] = () => { OnBoost?.Invoke(); OnAbilityComplete?.Invoke("BOOST"); };
        confirmationActions[ConfirmationAction.ConfirmationType.CollateralDamage] = () => OnAbilityComplete?.Invoke("COLLATERAL DAMAGE");
    }
    private void initializeConfirmationHandlers()
    {
        confirmationHandlers = new Dictionary<string, Action>()
        {
            { "Heal", onHealConfirmationRequest },
            { "Enhance", onEnhanceConfirmationRequest },
            {"Recruit", onRecruitConfirmationRequest },
            {"Overcome", onOvercomeConfirmationRequest },
            {"Feat", onFeatConfirmationRequest },
            {"Leave", onLeaveConfirmationRequest },
            {"Discard", onDiscardConfirmationRequest },
            {"Ayumi", onAyumiConfirmationRequest },
            {"Izumi", onIzumiConfirmationRequest },
            {"Mace", onMaceConfirmationRequest },
            {"Michael", onMichaelConfirmationRequest },
            {"Origin", onOriginConfirmationRequest },
            {"Rohan", onRohanConfirmationRequest },
            {"Yasmine", onYasmineConfirmationRequest },
            {"Zhao", onZhaoConfirmationRequest },
            {"Zoe", onZoeConfirmationRequest },
            {"Accelerate", onAccelerateConfirmationRequest },
            {"Backfire", onBackfireConfirmationRequest },
            {"Boost", onBoostConfirmationRequest },
            {"Collateral Damage", onCollateralDamageConfirmationRequest }
        };
    }

    private void onHealConfirmationRequest()
    {
        queueConfirmation(new ConfirmationAction("Confirm 'Heal'", ConfirmationAction.ConfirmationType.Heal));
    }
    private void onEnhanceConfirmationRequest()
    {
        queueConfirmation(new ConfirmationAction("Confirm 'Enhance'", ConfirmationAction.ConfirmationType.Enhance));
    }
    private void onRecruitConfirmationRequest()
    {
        queueConfirmation(new ConfirmationAction("Confirm 'Recruit'", ConfirmationAction.ConfirmationType.Recruit));
    }
    private void onOvercomeConfirmationRequest()
    {
        queueConfirmation(new ConfirmationAction("Confirm 'Overcome'", ConfirmationAction.ConfirmationType.Overcome));
    }
    private void onFeatConfirmationRequest()
    {
        queueConfirmation(new ConfirmationAction("Confirm 'Feat'", ConfirmationAction.ConfirmationType.Feat));
    }
    private void onLeaveConfirmationRequest()
    {
        queueConfirmation(new ConfirmationAction("Confirm 'Quit'", ConfirmationAction.ConfirmationType.Quit));
    }
    private void onDiscardConfirmationRequest()
    {
        queueConfirmation(new ConfirmationAction("Confirm: Isaac's Draw from Discard", ConfirmationAction.ConfirmationType.Isaac));
    }
    private void onAyumiConfirmationRequest()
    {
        queueConfirmation(new ConfirmationAction("Confirm: Ayumi's Draw an Enhance Card", ConfirmationAction.ConfirmationType.Ayumi));
    }
    private void onIzumiConfirmationRequest()
    {
        queueConfirmation(new ConfirmationAction("Toggle: Izumi's +20 Defense Buff", ConfirmationAction.ConfirmationType.Izumi));
    }
    private void onMaceConfirmationRequest()
    {
        queueConfirmation(new ConfirmationAction("Double the Total Attack for the next attack that occurs this turn?", ConfirmationAction.ConfirmationType.Mace));
    }
    private void onMichaelConfirmationRequest()
    {
        queueConfirmation(new ConfirmationAction("Confirm: Michael's Draw an Enhance Card", ConfirmationAction.ConfirmationType.Michael));
    }
    private void onOriginConfirmationRequest()
    {
        needSendResponse = true;
        queueConfirmation(new ConfirmationAction("Confirm: Block incoming damage towards Origin", ConfirmationAction.ConfirmationType.Origin));
    }
    private void onRohanConfirmationRequest()
    {
        queueConfirmation(new ConfirmationAction("Confirm: Rohan's recruitment", ConfirmationAction.ConfirmationType.Rohan));
    }
    private void onYasmineConfirmationRequest()
    {
        queueConfirmation(new ConfirmationAction("Confirm: Yasmine's play card", ConfirmationAction.ConfirmationType.Yasmine));
    }
    private void onZhaoConfirmationRequest()
    {
        queueConfirmation(new ConfirmationAction("Confirm: Zhao's play card", ConfirmationAction.ConfirmationType.Yasmine));
    }
    private void onZoeConfirmationRequest()
    {
        queueConfirmation(new ConfirmationAction("Confirm: Zoe's heal ability", ConfirmationAction.ConfirmationType.Zoe));
    }

    private void onAccelerateConfirmationRequest()
    {
        queueConfirmation(new ConfirmationAction("Confirm: Accelerate Draw and Discard", ConfirmationAction.ConfirmationType.Accelerate));
    }
    private void onBackfireConfirmationRequest()
    {
        queueConfirmation(new ConfirmationAction("Activate Backfire", ConfirmationAction.ConfirmationType.Backfire));
    }
    private void onBoostConfirmationRequest()
    {
        queueConfirmation(new ConfirmationAction("Activate Boost?", ConfirmationAction.ConfirmationType.Boost));
    }
    private void onCollateralDamageConfirmationRequest()
    {
        queueConfirmation(new ConfirmationAction("Activate Collateral Damage?", ConfirmationAction.ConfirmationType.CollateralDamage));
    }

    private void displayNextConfirmation()
    {
        ConfirmationAction action = confirmationQueue.Peek();
        GameObject confirmationPanel = Instantiate(confirmationPanelPrefab, transform);
        TMP_Text descriptionText = confirmationPanel.GetComponentInChildren<TMP_Text>();
        descriptionText.text = action.MyText;

        Button confirmButton = confirmationPanel.transform.Find("ConfirmButton").GetComponent<Button>();
        Button declineButton = confirmationPanel.transform.Find("DeclineButton").GetComponent<Button>();

        confirmButton.onClick.AddListener(() =>
        {
            confirmationQueue.Dequeue();
            OnActionConfirmation?.Invoke(action.MyType);
            Destroy(confirmationPanel);
        });

        declineButton.onClick.AddListener(() =>
        {
            confirmationQueue.Dequeue();
            OnActionConfirmation?.Invoke(action.MyType);
            Destroy(confirmationPanel);
            this.decline();
            if (confirmationQueue.Count > 0)
            {
                displayNextConfirmation();
            }
        });
    }
    private void queueConfirmation(ConfirmationAction action)
    {
        confirmationQueue.Enqueue(action);
        if (confirmationQueue.Count == 1) displayNextConfirmation();
    }
    public void onConfirmationRequest(string type)
    {
        // Check if the dictionary contains the specified confirmation type
        if (confirmationHandlers.ContainsKey(type))
        {
            // Invoke the corresponding handler method
            confirmationHandlers[type]?.Invoke();
        }
        else
        {
            Debug.Log($"Unhandled confirmation request: {type}");
        }
    }
    public void Accept(ConfirmationAction.ConfirmationType type)
    {
        // Check if the dictionary contains the specified ConfirmationType
        if (confirmationActions.ContainsKey(type))
        {
            // Invoke the corresponding action from the dictionary
            confirmationActions[type]?.Invoke();
        }

        // Handle other logic after confirmation action (if needed)
        if (confirmationQueue.Count > 0)
        {
            displayNextConfirmation();
        }
    }
    public void decline()
    {
        activeAbility = null;
        if (needSendResponse) OnSendAbilityResponse?.Invoke(false);
        needSendResponse = false;
        //confirmationUI.SetActive(false);
    }
    private void handleAbilityConfirmation()
    {
        if (activeAbility != null)
        {
            activeAbility.Target(myTargetCard);
            activeAbility = null;
        }
        else
        {
            OnTargetAccepted?.Invoke(myTargetCard, myCardToUse);
        }
    }
    #endregion

    #region Targetting
    private void HandleTargeting(Card cardToBePlayed, bool target)
    {
        if (target)
        {
            myCardToUse = cardToBePlayed;          
        }
    }
    private void HandleActiveHero(Ability ability)
    {
        activeAbility = ability;
    }
    private void HandleTarget(CardData card)
    {
        myTargetCard = card;
        if (myCardToUse != null)
        {
            switch (myCardToUse.CardType)
            {
                case Card.Type.Ability:
                    queueConfirmation(new ConfirmationAction("Confirm Ability to target.", ConfirmationAction.ConfirmationType.Ability));
                    break;
                case Card.Type.Enhancement:
                    queueConfirmation(new ConfirmationAction("Confirm Enhancement to target.", ConfirmationAction.ConfirmationType.Enhancing));
                    break;
            }
        }
        else
        {
            queueConfirmation(new ConfirmationAction($"Confirm target for {activeAbility.Name}'s Ability", ConfirmationAction.ConfirmationType.Ability));
        }
    }
    #endregion
}

public class ConfirmationAction
{
    public enum ConfirmationType { Heal, Enhance, Recruit, Overcome, Feat, Quit, Enhancing, Ability, AtDef, Ayumi, Isaac, Izumi, Mace, Michael, Origin, Rohan, Yasmine, Zhao, Zoe
    , Accelerate, Backfire, Boost, CollateralDamage
    }
    public ConfirmationType MyType = ConfirmationType.Heal;
    public string MyText;

    public ConfirmationAction(string description, ConfirmationType type)
    {
        MyText = description;
        MyType = type;
    }
}
