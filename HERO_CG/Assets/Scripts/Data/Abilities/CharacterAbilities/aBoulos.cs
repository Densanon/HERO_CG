//Created by Jordan Ezell
//Last Edited: 6/29/23 Jordan

public class aBoulos : Ability
{
    protected override void Awake()
    {
        base.Awake();

        myType = Type.Character;
        secondaryType = Type.Passive;
        Name = "BOULOS";
        Description = "(P) For each card in your hand, Boulos gains +10 defense.";
    }

    public override void PassiveCheck(PassiveType passiveType)
    {
        base.PassiveCheck(passiveType);

        if( passiveType == PassiveType.HandCardAdjustment)
        {
            myHero.NewAbilityDefModifier(CardDataBase.handSize*10);
        }
    }
}
