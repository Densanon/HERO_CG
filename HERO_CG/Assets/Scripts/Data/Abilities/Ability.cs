using UnityEngine;
using System;

public class Ability : MonoBehaviour
{
    public enum Type { Feat, Activate, Passive, Character}
    public Type myType;
    public Type secondaryType;

    public enum PassiveType { CharacterSpawn, CharacterDestroyed, BattleComplete, BattleStart, HeroRecruited, HandCardAdjustment, ActionComplete, HeroFatigued}

    public string Name;
    public string Description;
    protected CardData myHero;
    protected bool oncePerTurnUsed = false;
    protected bool canActivate = false;

    public static Action<Ability> OnAddAbilityToMasterList = delegate { };
    public static Action OnAbilityUsed = delegate { };
    public static Action OnFeatComplete = delegate { };
    public static Action<int> OnNeedCardDraw = delegate { };
    public static Action<string, string, int> OnDiscardCard = delegate { };
    public static Action<Ability> OnSetActive = delegate { };
    public static Action<Ability> OnOpponentAbilityActivation = delegate { };
    public static Action<Ability> OnActivateable = delegate { };
    public static Action OnHoldTurn = delegate { };
    public static Action OnPreventAbilitiesToFieldForTurn = delegate { };

    protected virtual void Awake()
    {
        PhotonGameManager.OnPassiveActivate += PassiveCheck;
        PhotonGameManager.OnTurnResetabilities += ResetOncePerTurn;
        myHero = this.gameObject.GetComponent<CardData>();
        OnAddAbilityToMasterList?.Invoke(this);
        Debug.Log("Generic Ability Awake.");
    }

    protected virtual void OnDestroy()
    {
        PhotonGameManager.OnPassiveActivate -= PassiveCheck;
        PhotonGameManager.OnTurnResetabilities -= ResetOncePerTurn;
    }

    private void ResetOncePerTurn()
    {
        oncePerTurnUsed = false;
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
        if(myType == Type.Passive || secondaryType == Type.Passive)
        Debug.Log($"{Name} PassiveCheck {passiveType} Activated");
    }
}
