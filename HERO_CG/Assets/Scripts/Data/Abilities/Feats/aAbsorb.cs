using UnityEngine;
using System.Collections.Generic;

public class aAbsorb : Ability
{
    //"ABSORB", "(H) Discard the Enhancements, if any, from any one hero. Then, replace with those from another hero."

    CardData Target1;
    CardData Target2;

    List<Ability> abilities = new List<Ability>();
    List<Enhancement> enhancements = new List<Enhancement>();

    #region Unity Methods
    private void Awake()
    {
        myType = Ability.Type.Feat;
        Name = "ABSORB";
        Description = "Discard the Enhancements, if any, from any one hero. Then, replace with those from another hero.";
    }
    #endregion

    #region Ability Methods
    public override void Target(CardData card)
    {
        if (Target1 == null)
        {
            Debug.Log("Getting first target for Absorb");
            Target1 = card;
            RemoveEnhancements(Target1);
            return;
        }
        else if (Target2 == null)
        {
            Debug.Log("Getting second target for Absorb");

            Target2 = card;
            abilities = Target2.GetCharacterAbilities();
            enhancements = Target2.GetCharacterEnhancements();
            RemoveEnhancements(Target2);

            if(abilities != null)
                Target1.GainAbilities(abilities, false);
            if(enhancements != null)
                Target1.GainEnhancements(enhancements, false);

            OnAbilityUsed?.Invoke();
        }
    }
    #endregion

    private void RemoveEnhancements(CardData card)
    {
        card.StripAbilities(false);
        card.StripEnhancements(false);
        Debug.Log($"Stripped abilities and enhancements from {card.Name}");
    }
}
