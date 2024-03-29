﻿//Created by Jordan Ezell
//Last Edited: 6/30/23 Jordan

using UnityEngine;
using System;

public class Ability : MonoBehaviour
{
    public enum Type { Feat, Activate, Passive, Character }
    public Type myType;
    public Type secondaryType;

    public enum PassiveType { CharacterSpawn, CharacterDestroyed, BattleComplete, BattleStart, HeroRecruited, HandCardAdjustment, ActionComplete, HeroFatigued, HeroHealed }

    public string Name;
    public string Description;
    protected CardData myHero;
    public bool oncePerTurnUsed { get; private set; }
    public bool canActivate { get; private set; }
    protected bool passiveCheckable = false;

    public static Action<Ability> OnAddAbilityToMasterList = delegate { };
    public static Action OnFeatComplete = delegate { };
    public static Action<string> OnConfirmAyumiDrawEnhanceCard = delegate { };
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

    public static Action<string> OnNeedDrawFromDiscard = delegate { };
    public static Action OnToggleIzumi = delegate { };
    //public static Action<string> OnToggleIzumiConfirmationRequest = delegate { };

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
    }

    private void ResetOncePerTurn()
    {
        oncePerTurnUsed = false;
    }

    protected void ChangeOncePerTurn(bool change)
    {
        oncePerTurnUsed = change;
    }
    protected void ChangeCanActivate(bool change)
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

    }
}
