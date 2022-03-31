using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class CardData : MonoBehaviour
{
    public enum FieldPlacement { Mine, Opp, Draft, HQ, Zoom, Hand}
    public FieldPlacement myPlacement;

    public enum CardState { Normal, Exhausted, PotentialNeutralTarget, PotentialDefenderTarget, Attacking, Defending}
    public CardState myState = CardState.Normal;
    public CardState prevState = CardState.Normal;

    [SerializeField] Image Icon;
    [SerializeField] TMP_Text tAttack;
    [SerializeField] TMP_Text tDefense;
    [SerializeField] TMP_Text tbAttack;
    [SerializeField] TMP_Text tbDefense;
    [SerializeField] Image[] gAbilityCounters;
    [SerializeField] Button Target;
    public List<Ability> myAbilities = new List<Ability>();
    public List<Enhancement> myEnhancements = new List<Enhancement>();
    public Ability charAbility;
    int abilityAttModifier = 0;
    int abilityDefModifier = 0;
    int enhancementAttModifier = 0;
    int enhancementDefModifier = 0;
    bool charAbSet = false;
    ParticleSystem myParticleSystem;

    public static Action<CardData> IsTarget = delegate { };
    public static Action<CardData, string, int> OnNumericAdjustment = delegate { };
    public static Action<CardData, bool> OnExhausted = delegate { };
    public static Action<CardData> OnDestroyed = delegate { };
    public static Action<CardData> OnAbilitiesStripped = delegate { };
    public static Action<CardData> OnEnhancementsStripped = delegate { };
    public static Action<List<Ability>, CardData> OnGivenAbilities = delegate { };
    public static Action<List<Enhancement>, CardData> OnGivenEnhancements = delegate { };

    public bool Exhausted { get; private set; }

    public  Card myCard { get; private set; }

    public Sprite CardImage { get; private set; }

    public Card.Type CardType { get; private set; }

    public string Name { get; private set; }

    public int Attack { get; private set; }

    public int Defense { get; private set; }

    public int AbilityCounter { get; private set; }

    public CardData(Card card, FieldPlacement placement)
    {
        myPlacement = placement;
        Exhausted = false;
        CardType = card.CardType;
        Name = card.Name;
        Attack = card.Attack;
        Defense = card.Defense;
        CardImage = card.image;
    }

    #region Unity Methods
    private void Awake()
    {
        CardDataBase.OnTargeting += HandleTargetting;
        Referee.OnOvercomeTime += HandleActivateOvercome;
        Referee.OnOvercomeSwitch += HandleSwitchOvercome;
        myParticleSystem = gameObject.GetComponentInChildren<ParticleSystem>();

        UISetup();
    }

    private void OnDestroy()
    {
        CardDataBase.OnTargeting -= HandleTargetting;
        Referee.OnOvercomeTime -= HandleActivateOvercome;
        Referee.OnOvercomeSwitch -= HandleSwitchOvercome;
    }
    #endregion

    #region Public Methods
    public void DamageCheck(int dmg)
    {
        if(dmg >= Defense)
        {
            //Destroy
            Debug.Log($"{Name} has been destroyed.");
            Exhausted = true;
            OnDestroyed?.Invoke(this);
        }else if (dmg >= Defense/2)
        {
            //Exhaust
            Exhaust(false);
        }
        else
        {
            //Block
            Debug.Log($"{Name} has blocked.");
        }
    }

    public void Exhaust(bool told)
    {
        Exhausted = true;
        StateChange(CardState.Exhausted);
        SetDefense(Defense / 2);
        tDefense.color = Color.red;

        if(!told)
            OnExhausted?.Invoke(this, true);
        Debug.Log($"{Name} has been exhausted.");
    }

    public void Heal(bool told)
    {
        Exhausted = false;
        StateChange(CardState.Normal);
        SetDefense(Defense * 2);
        tDefense.color = Color.white;

        if (!told)
            OnExhausted?.Invoke(this, false);
    }

    public void SetAttack(int amount)
    {
        Attack = amount;
        tAttack.text = Attack.ToString();
    }

    public void AdjustAttack(int amount)
    {
        bool sendit = (Attack != (Attack + amount));
        HandleEnhancementAddition(amount, 'a');
        ValuesSetup();
        if (sendit)
        {
            OnNumericAdjustment?.Invoke(this, "Attack", Attack);
        }
    }

    public void NewAbilityAttModifier(int amountAdjustment)
    {
        int i = abilityAttModifier - amountAdjustment;

        if(i != 0)
        {
            abilityAttModifier = amountAdjustment;
            ValuesSetup();
            OnNumericAdjustment?.Invoke(this, "Attack", Attack);
        }
    }

    public void SetDefense(int amount)
    {
        Defense = amount;
        tDefense.text = Defense.ToString();
    }

    public void AdjustDefense(int amount)
    {
        bool sendit = (Defense != (Defense + amount));
        HandleEnhancementAddition(amount, 'd');
        ValuesSetup();
        if (sendit)
        {
            OnNumericAdjustment?.Invoke(this, "Defense", Defense);
        }
    }

    public void NewAbilityDefModifier(int amountAdjustment)
    {

        int i = abilityDefModifier - amountAdjustment;
        if(abilityDefModifier == 0)
        {
            i = amountAdjustment;
        }
        //Debug.Log($"NewAbilityDefModifier: Current modifier{abilityDefModifier}/ Current Adjustment{amountAdjustment}");

        if(i != 0)
        {
            //Debug.Log($"NewAbilityDefModifier: amount adjusted {i}");
            abilityDefModifier = amountAdjustment;
            ValuesSetup();
            OnNumericAdjustment?.Invoke(this, "Defense", Defense);
        }
    }

    public void AdjustCounter(int amount, Ability ability)
    {
        AbilityCounter += amount;
            for(int i = 0; i < gAbilityCounters.Length-1; i++)
            {
                if(AbilityCounter-1 >= i)
                {
                    gAbilityCounters[i].color = Color.yellow;
                }
                else
                {
                    gAbilityCounters[i].color = Color.clear;
                }
            }
        if(myPlacement != FieldPlacement.Opp)
        {
            if (amount > 0)
            {
                myAbilities.Add(ability);
            }
            else
            {
                myAbilities.Remove(ability);
            }
        }
    }

    public List<Ability> GetCharacterAbilities()
    {
        return myAbilities;
    }

    public List<Enhancement> GetCharacterEnhancements()
    {
        Debug.Log($"Giving {myEnhancements.Count} enhancements");
        return myEnhancements;
    }

    public void StripAbilities(bool told)
    {
        foreach(Image im in gAbilityCounters)
        {
            im.color = Color.clear;
        }

        if(myAbilities != null)
            myAbilities.Clear();

        if (!told)
        {
            OnAbilitiesStripped?.Invoke(this);
        }
        GetCharacterAbilities();
    }

    public void StripEnhancements(bool told)
    {
        if(myEnhancements != null)
            myEnhancements.Clear();
        
        CardOverride(myCard, myPlacement);
        if (!told)
        {
            OnEnhancementsStripped?.Invoke(this);
        }
    }

    public void GainAbilities(List<Ability> abilities, bool told)
    {
        //need to add all the abilities to the card and update its info
        Debug.Log($"GainingAbilities: {abilities.Count}");
        foreach(Ability a in abilities)
        {
            Debug.Log($"Ability to be gained: {a.Name}");
            AdjustCounter(1, a);
        }

        if(!told)
        OnGivenAbilities?.Invoke(abilities, this);
        Debug.Log("GainAbilities complete.");
    }

    public void GainEnhancements(List<Enhancement> enhancements, bool told)
    {
        //need to add all the enhancements to the card and update its info
        Debug.Log("GainingEnhancements");
        foreach (Enhancement e in enhancements)
        {
            Debug.Log($"Adding {e.attack}:{e.attack}");
            if (myEnhancements == null)
            {
                Debug.Log("myEnhancements were null");
                myEnhancements = new List<Enhancement>();
            }


            if (e.attack > 0)
            {
                Debug.Log("Attack addition.");
                AdjustAttack(e.attack);
                continue;
            }

            Debug.Log("Defense Addition.");
            AdjustDefense(e.defense);
        }

        if (!told)
            OnGivenEnhancements?.Invoke(enhancements, this);
        Debug.Log("Enhancement Gain complete.");
    }

    public void CardOverride(Card card, FieldPlacement placement)
    {
        myPlacement = placement;
        myCard = card;
        CardType = card.CardType;
        Name = card.Name;
        Attack = card.Attack;
        Defense = card.Defense;
        if(myPlacement == FieldPlacement.Mine || myPlacement == FieldPlacement.Opp || myPlacement == FieldPlacement.HQ)
        {
            CardImage = card.alphaImage;
        }
        else
        {
            CardImage = card.image;
        }

        if(CardType == Card.Type.Character)
        {
            SetCharacterAbility();
        }

        UISetup();
    }

    public void CardOverride(CardData card, FieldPlacement placement)
    {
        myPlacement = placement;
        myCard = card.myCard;
        CardType = card.CardType;
        Name = card.Name;
        Attack = card.Attack;
        Defense = card.Defense;
        if (myPlacement == FieldPlacement.Mine || myPlacement == FieldPlacement.Opp || myPlacement == FieldPlacement.HQ)
        {
            CardImage = card.myCard.alphaImage;
        }
        else
        {
            CardImage = card.myCard.image;
        }


        if (CardType == Card.Type.Character)
        {
            SetCharacterAbility();
        }

        UISetup();
    }

    public void Targeted()
    {
        IsTarget?.Invoke(this);
    }

    public void OvercomeTarget(bool target)
    {
        if (target)
        {
            StateChange(CardState.Attacking);
            return;
        }

        StateChange(prevState);
        
    }

    public void ParticleSetAndStart(Color color)
    {
        if(myParticleSystem != null)
        {
            var main = myParticleSystem.main;
            main.startColor = color;
            myParticleSystem.Play();
        }
    }

    public void ParticleStop()
    {
        myParticleSystem.Stop();
    }
    #endregion

    #region Private Methods
    private void UISetup()
    {

        Icon.sprite = CardImage;
        if(tbAttack != null && CardType == Card.Type.Character)
        {
            tAttack.text = Attack.ToString();
            tbAttack.text = Attack.ToString();
            tDefense.text = Defense.ToString();
            tbDefense.text = Defense.ToString();
        }else if(myPlacement == FieldPlacement.HQ || myPlacement == FieldPlacement.Mine || myPlacement == FieldPlacement.Opp)
        {
            tAttack.text = Attack.ToString();
            tDefense.text = Defense.ToString();
        }
        else
        {
            tAttack.text = "";
            tDefense.text = "";
            if (tbAttack != null)
            {
                tbAttack.text = "";
                tbDefense.text = "";
            }
        }

        //Feat not Feat card coloring for use
        if(Referee.myPhase == Referee.GamePhase.Feat && myPlacement == FieldPlacement.Hand)
        {
           if(CardType == Card.Type.Feat)
            {
                StateChange(CardState.Normal);
            }
            else
            {
                StateChange(CardState.Exhausted);
            }
        }
        else if (myPlacement == FieldPlacement.Hand)
        {
            if (CardType == Card.Type.Feat)
            {
                StateChange(CardState.Exhausted);
            }
            else
            {
                StateChange(CardState.Normal);
            }
        }
    }

    private void ValuesSetup()
    {
        int a = myCard.Attack;
        int d = myCard.Defense;

        a += enhancementAttModifier;
        d += enhancementDefModifier;

        a += abilityAttModifier;
        d += abilityDefModifier;

        Attack = a;
        Defense = d;
        if (Exhausted)
            Defense = Defense / 2;

        tDefense.text = Defense.ToString();
        tAttack.text = Attack.ToString();
    }

    private void StateChange(CardState StateToTransitionTo)
    {
        prevState = myState;
        myState = StateToTransitionTo;

        switch (StateToTransitionTo)
        {
            case CardState.Attacking:
                Icon.color = Color.blue;
                break;
            case CardState.Defending:
                Icon.color = Color.cyan;
                break;
            case CardState.Exhausted:
                Icon.color = Color.grey;
                break;
            case CardState.Normal:
                Icon.color = Color.white;
                break;
            case CardState.PotentialDefenderTarget:
                Icon.color = Color.red;
                break;
            case CardState.PotentialNeutralTarget:
                Icon.color = Color.green;
                break;
        }
    }

    private void HandleTargetting(Card card, bool target)
    {
        if(Target != null)
        {
            Target.gameObject.SetActive(target);
            if (target)
            {
                StateChange(CardState.PotentialNeutralTarget);
                return;
            }

            StateChange(prevState);
        }
    }

    private void HandleActivateOvercome(bool onOff)
    {
        if (onOff)
        {
            switch (myPlacement)
            {
                case FieldPlacement.Mine:
                    if (!Exhausted)
                    {
                        StateChange(CardState.PotentialNeutralTarget);
                    }
                    break;
                case FieldPlacement.Opp:
                    if (!Exhausted)
                    {
                        StateChange(CardState.Normal);
                    }
                    break;
            }
        }
        else
        {
            switch (myPlacement)
            {
                case FieldPlacement.Mine:
                    if (!Exhausted)
                    {
                        StateChange(CardState.Normal);
                    }
                    break;
                case FieldPlacement.Opp:
                    if (!Exhausted)
                    {
                        StateChange(CardState.Normal);
                    }
                    break;
            }
        }
    }

    private void HandleSwitchOvercome()
    {
        if (Referee.AttDef)
        {
            switch (myPlacement)
            {
                case FieldPlacement.Mine:
                    if (!Exhausted)
                    {
                        StateChange(CardState.PotentialNeutralTarget);
                    }
                    break;
                case FieldPlacement.Opp:
                    if (!Exhausted)
                    {
                        StateChange(CardState.Normal);
                    }
                    break;
            }
        }
        else
        {
            switch (myPlacement)
            {
                case FieldPlacement.Mine:
                    if (!Exhausted)
                    {
                        StateChange(CardState.Normal);
                    }
                    break;
                case FieldPlacement.Opp:
                    StateChange(CardState.PotentialDefenderTarget);
                    break;
            }
        }
    }

    private void HandleEnhancementAddition(int amount, char type) {

        Enhancement e = new Enhancement();
        if(type == 'a')
        {
            e.attack = amount;
            enhancementAttModifier += amount;
        }
        else
        {
            e.defense = amount;
            enhancementDefModifier += amount;
        }
        if(myEnhancements == null)
        {
            myEnhancements = new List<Enhancement>();
        }
        myEnhancements.Add(e);

    }

    private void SetCharacterAbility()
    {
        if (charAbSet)
        {
            return;
        }
        switch (Name)
        {
            case "AKIO":
                Ability a = this.gameObject.AddComponent<aAkio>();
                charAbility = a;
                break;
            case "AYUMI":
                Ability b = this.gameObject.AddComponent<aAyumi>();
                charAbility = b;
                break;
            case "BOULOS":
                Ability c = this.gameObject.AddComponent<aBoulos>();
                charAbility = c;
                break;
            case "CHRISTOPH":
                Ability d = this.gameObject.AddComponent<aChristoph>();
                charAbility = d;
                break;
            case "ENG":
                Ability e = this.gameObject.AddComponent<aEng>();
                charAbility = e;
                break;
            case "GAMBITO":
                Ability f = this.gameObject.AddComponent<aGambito>();
                charAbility = f;
                break;
            case "GRIT":
                Ability g = this.gameObject.AddComponent<aGrit>();
                charAbility = g;
                break;
            case "HINDRA":
                Ability h = this.gameObject.AddComponent<aHindra>();
                charAbility = h;
                break;
            case "IGNACIA":
                Ability i = this.gameObject.AddComponent<aIgnacia>();
                charAbility = i;
                break;
            case "ISAAC":
                Ability j = this.gameObject.AddComponent<aIsaac>();
                charAbility = j;
                break;
            case "IZUMI":
                Ability k = this.gameObject.AddComponent<aIzumi>();
                charAbility = k;
                break;
            case "KAY":
                Ability l = this.gameObject.AddComponent<aKay>();
                charAbility = l;
                break;
            case "KYAUTA":
                Ability m = this.gameObject.AddComponent<aKyauta>();
                charAbility = m;
                break;
            case "MACE":
                Ability n = this.gameObject.AddComponent<aMace>();
                charAbility = n;
                break;
            case "MICHAEL":
                Ability o = this.gameObject.AddComponent<aMichael>();
                charAbility = o;
                break;
            case "ORIGIN":
                Ability p = this.gameObject.AddComponent<aOrigin>();
                charAbility = p;
                break;
            case "ROHAN":
                Ability q = this.gameObject.AddComponent<aRohan>();
                charAbility = q;
                break;
            case "YASMINE":
                Ability r = this.gameObject.AddComponent<aYasmine>();
                charAbility = r;
                break;
            case "ZHAO":
                Ability s = this.gameObject.AddComponent<aZhao>();
                charAbility = s;
                break;
            case "ZOE":
                Ability t = this.gameObject.AddComponent<aZoe>();
                charAbility = t;
                break;
        }
        charAbSet = true;
    }
    #endregion
}

public class Enhancement{
    public int attack;
    public int defense;
    }
