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
    protected override void Awake()
    {
        myType = Ability.Type.Feat;
        Name = "ABSORB";
        Description = "Discard the Enhancements, if any, from any one hero. Then, replace with those from another hero.";
    }
    #endregion

    #region Ability Methods
    public override void Target(CardData card)
    {
        base.Target(card);
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
            Target1.GainAbilities(abilities, false);

            enhancements = Target2.GetCharacterEnhancements();
            Debug.Log($"enhancement grab count: {enhancements.Count}");

            foreach (Enhancement e in enhancements)
            {
                Debug.Log($"Grabbed {e.attack}:{e.defense} to add.");
            }

            if(enhancements != null)
            {
                Target1.GainEnhancements(enhancements, false);
                Debug.Log("Gaining Enhancements.");
            }

            RemoveEnhancements(Target2);
            OnFeatComplete?.Invoke();
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
