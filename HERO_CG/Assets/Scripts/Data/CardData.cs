using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardData : MonoBehaviour
{
    [SerializeField] TMP_Text Title;
    [SerializeField] TMP_Text tFlavor;
    [SerializeField] Image Icon;
    [SerializeField] TMP_Text tAttack;
    [SerializeField] TMP_Text tDefense;
    [SerializeField] TMP_Text tAbility;

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

    public bool inHand = false;

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

    public void Exhaust()
    {
        Exhausted = true;
    }

    public void Heal()
    {
        Exhausted = false;
    }

    public void AdjustAttack(int amount)
    {
        Attack += amount;
    }

    public void AdjustDefense(int amount)
    {
        Defense += amount;
    }

    public void AdjustCounter(int amount)
    {
        AbilityCounter += amount;
    }

    private void Awake()
    {
        UISetup();
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

    private void UISetup()
    {
        Title.text = Name;
        tFlavor.text = Flavor;
        Icon.sprite = CardImage;
        tAttack.text = Attack.ToString();
        tDefense.text = Defense.ToString();
        tAbility.text = Ability;
    }
}
