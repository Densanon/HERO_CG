using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aHardened : Ability
{
    private void Awake()
    {
        base.Awake();
        myType = Type.Passive;
        Name = "HARDENED";
        Description = "(P) For every opposing card on the field, this hero may gain +10 defense.";
        CardDataBase.HardenedPresent = true;
    }

    private void OnDestroy()
    {
        CardDataBase.HardenedPresent = false;
        base.OnDestroy();
    }

    public override void PassiveCheck(PassiveType passiveType)
    {
        if(Referee.myTurn && (passiveType == PassiveType.CardPlayed || passiveType == PassiveType.CharacterDestroyed || passiveType == PassiveType.CharacterSpawn))
        {
            base.PassiveCheck(passiveType);
            Debug.Log("I am going to adjust the card now.");
            myHero.NewAbilityDefModifier(CardDataBase.oppFieldCardCount * 10);
        }else if(!Referee.myTurn && (passiveType == PassiveType.CardPlayed || passiveType == PassiveType.CharacterDestroyed || passiveType == PassiveType.CharacterSpawn))
        {
            OnHardenedUpdate.Invoke();
        }
    }

    public override void AbilityCompleteCleanup(string abilityName)
    {
        if(abilityName == Name)
        {
            base.AbilityCompleteCleanup(abilityName);
            myHero.NewAbilityDefModifier(CardDataBase.oppFieldCardCount * 10);
        }
    }
}
