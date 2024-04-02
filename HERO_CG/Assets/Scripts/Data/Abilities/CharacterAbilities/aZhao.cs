using UnityEngine;

public class aZhao : Ability
{
    public static bool zhaoBoost = false;

    private void Awake()
    {
        base.Awake();

        myType = Type.Character;
        secondaryType = Type.Passive;
        Name = "ZHAO";
        Description = "(P) Allied heroes gain +10 attack while Zhao is in play. When Zhao is defeated or removed play a card to the field.";
        if (myHero.myPlacement == CardData.FieldPlacement.Mine) { zhaoBoost = true; OnModifyValues?.Invoke(); }
        CardDataBase.OnZhaoAbilityRequest += HandleAbilityTriggered;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        CardDataBase.OnZhaoAbilityRequest -= HandleAbilityTriggered;
    }

    private void HandleAbilityTriggered(string obj)
    {
        OnActivateable?.Invoke(this);
    }

    public override void ActivateAbility()
    {
        Debug.Log("Zhao death ability activated. Should update everyone's values.");
        base.ActivateAbility();
        zhaoBoost = false;
        OnModifyValues?.Invoke();
    }
}
