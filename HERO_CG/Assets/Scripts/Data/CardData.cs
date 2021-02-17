using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class CardData : MonoBehaviour
{
    [SerializeField] TMP_Text Title;
    [SerializeField] TMP_Text tFlavor;
    [SerializeField] Image Icon;
    [SerializeField] Image Highlight;
    [SerializeField] TMP_Text tAttack;
    [SerializeField] TMP_Text tDefense;
    [SerializeField] TMP_Text tAbility;
    [SerializeField] Image[] gAbilityCounters;
    [SerializeField] Button Target;

    public static Action<CardData> IsTarget = delegate { };
    public static Action<CardData, string, int> OnNumericAdjustment = delegate { };

    public bool Exhausted { get; private set; }

    public  Card myCard { get; private set; }

    public Sprite CardImage { get; private set; }

    public Card.Type CardType { get; private set; }

    public string Name { get; private set; }

    public int Attack { get; private set; }

    public int Defense { get; private set; }

    public string Flavor { get; private set; }

    public string Ability { get; private set; }

    public int AbilityCounter { get; private set; }

    public CardData(Card card)
    {
        Exhausted = false;
        CardType = card.CardType;
        Name = card.Name;
        Attack = card.Attack;
        Defense = card.Defense;
        Flavor = card.Flavor;
        Ability = card.Ability;
        CardImage = card.image;
    }

    #region Unity Methods
    private void Awake()
    {
        CardDataBase.OnTargeting += HandleTargetting;

        UISetup();
    }

    private void OnDestroy()
    {
        CardDataBase.OnTargeting -= HandleTargetting;
    }
    #endregion

    #region Public Methods
    public void Exhaust()
    {
        Exhausted = true;
    }

    public void Heal()
    {
        Exhausted = false;
    }

    public void SetAttack(int amount)
    {
        Attack = amount;
        tAttack.text = Attack.ToString();
    }

    public void AdjustAttack(int amount)
    {
        bool sendit = (Attack != (Attack + amount));
        Debug.Log($"Adjusting Attack from {Attack} to {Attack + amount}");
        Attack += amount;
        tAttack.text = Attack.ToString();
        if (sendit)
        {
            Debug.Log($"Sending the new Attack Value. {Attack}");
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
        Debug.Log($"Amount to adjust defense {amount}");
        Defense += amount;
        tDefense.text = Defense.ToString();
        if (sendit)
        {
            Debug.Log($"Sending the new Defense Value. {Defense}");
            OnNumericAdjustment?.Invoke(this, "Defense", Defense);
        }
    }

    public void AdjustCounter(int amount)
    {
        AbilityCounter += amount;
        for(int i = 0; i > gAbilityCounters.Length; i++)
        {
            if(AbilityCounter >= i)
            {
                gAbilityCounters[i].color = Color.yellow;
            }
            else
            {
                gAbilityCounters[i].color = Color.clear;
            }
        }
    }

    public void CardOverride(Card card)
    {
        myCard = card;
        CardType = card.CardType;
        Name = card.Name;
        Attack = card.Attack;
        Defense = card.Defense;
        Flavor = card.Flavor;
        Ability = card.Ability;
        CardImage = card.image;

        UISetup();
    }

    public void CardOverride(CardData card)
    {
        myCard = card.myCard;
        CardType = card.CardType;
        Name = card.Name;
        Attack = card.Attack;
        Defense = card.Defense;
        Flavor = card.Flavor;
        Ability = card.Ability;
        CardImage = card.CardImage;

        UISetup();
    }

    public void Targeted()
    {
        IsTarget?.Invoke(this);
    }
    #endregion

    #region Private Methods
    private void UISetup()
    {
        if (Title != null)
        {
            Title.text = Name;
        }
        if (tFlavor != null)
        {
            tFlavor.text = Flavor;
        }
        Icon.sprite = CardImage;
        tAttack.text = Attack.ToString();
        tDefense.text = Defense.ToString();
        if (tAbility != null)
        {
            tAbility.text = Ability;
        }
    }

    private void HandleTargetting(Card card)
    {
        if(Target != null)
        {
            bool targetting = !Target.interactable;
            Target.gameObject.SetActive(targetting);
            Target.interactable = targetting;
            Highlight.color = targetting ? Color.green : Color.clear;
        }
    }
    #endregion
}
