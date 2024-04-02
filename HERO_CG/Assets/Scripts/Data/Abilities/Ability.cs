//Created by Jordan Ezell
//Last Edited: 6/30/23 Jordan

using UnityEngine;
using System;

public class Ability : MonoBehaviour
{
    public enum Type { Feat, Activate, Passive, Character }
    public Type myType;
    public Type secondaryType;

    public enum PassiveType { TurnStart, CharacterSpawn, CharacterDestroyed, BattleCalculation, BattleComplete, BattleStart, HeroRecruited, HandCardAdjustment, ActionComplete, HeroFatigued, HeroHealed }

    public string Name;
    public string Description;
    protected CardData myHero;
    public bool oncePerTurnUsed { get; private set; }
    public bool canActivate { get; private set; }
    protected bool passiveCheckable = false;

    public static Action<Ability> OnAddAbilityToMasterList = delegate { };
    public static Action OnFeatComplete = delegate { };
    public static Action<string> OnCharacterAbilityRequest = delegate { };
    public static Action<string, string, int> OnDiscardCard = delegate { };
    public static Action<Ability> OnSetActive = delegate { };
    public static Action<Ability> OnOpponentAbilityActivation = delegate { };
    public static Action<Ability> OnActivateable = delegate { };
    public static Action OnPreventAbilitiesToFieldForTurn = delegate { };

    public static Action OnAbilityUsed = delegate { };
    public static Action<Referee.TargetType> OnRequestTargeting = delegate { };
    public static Action<Ability> OnTargetedFrom = delegate { };
    public static Action<bool> OnHoldTurn = delegate { };
    public static Action OnHandOverControl = delegate { };

    public static Action OnModifyValues = delegate { };
    public static Action OnActivateKayAbility = delegate {};
    public static Action OnNeedPlayFromReserve = delegate { };
    public static Action<string> OnCheckNeedResponse = delegate { };
    public static Action OnRohanAbility = delegate { };

    protected virtual void Awake()
    {
        canActivate = true;
        oncePerTurnUsed = false;
        myHero = this.gameObject.GetComponent<CardData>();
        passiveCheckable = myHero.myPlacement == CardData.FieldPlacement.Mine || myHero.myPlacement == CardData.FieldPlacement.Opp;
        if (passiveCheckable)
        {
            Referee.OnPassiveActivate += PassiveCheck;
            Referee.OnTurnResetables += ResetOncePerTurn;
            Referee.OnAbilityComplete += AbilityCompleteCleanup;
            UIConfirmation.OnAbilityComplete += AbilityCompleteCleanup;
        }
    }

    private void Start()
    {
        if(passiveCheckable)
            OnAddAbilityToMasterList?.Invoke(this);
    }

    protected virtual void OnDestroy()
    {
        Referee.OnPassiveActivate -= PassiveCheck;
        Referee.OnTurnResetables -= ResetOncePerTurn;
        Referee.OnAbilityComplete -= AbilityCompleteCleanup;
        UIConfirmation.OnAbilityComplete -= AbilityCompleteCleanup;
    }

    protected virtual void ResetOncePerTurn()
    {
        oncePerTurnUsed = false;
    }

    protected void ChangeOncePerTurn(bool change)
    {
        oncePerTurnUsed = change;
    }
    public void ChangeCanActivate(bool change)
    {
        canActivate = change;
    }

    public virtual void AbilityAwake()
    {
        Debug.Log("AbilityAwake Activated");
    }

    public virtual void Target(CardData card)
    {
        Debug.Log("AbilityTarget Activated");
    }

    public virtual void PassiveCheck(PassiveType passiveType)
    {

    }

    public virtual void ActivateAbility()
    {
        ChangeOncePerTurn(true);
    }

    public virtual void AbilityCompleteCleanup(string abilityName)
    {

    }
}
