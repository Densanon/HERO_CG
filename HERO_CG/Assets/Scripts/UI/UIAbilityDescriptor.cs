//Created by Jordan Ezell
//Last Edited: 6/30/23 Jordan

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class UIAbilityDescriptor : MonoBehaviour
{
    public TMP_Text title;
    public TMP_Text description;
    public GameObject descriptor;
    public Button activateAbility;
    Ability myAbility;

    public static Action<Ability> OnActivateAbility = delegate { };

    private void Awake()
    {
        UICharacterAbility.OnAbilityDescriptionPressed += HandleDescriptionInquery;
    }

    private void OnDestroy()
    {
        UICharacterAbility.OnAbilityDescriptionPressed -= HandleDescriptionInquery;
    }

    private void HandleDescriptionInquery(Ability ability)
    {
        myAbility = ability;
        descriptor.SetActive(true);
        title.text = ability.Name;
        description.text = ability.Description;
        if (Referee.myTurn && ability.secondaryType == Ability.Type.Activate  && ability.canActivate && Referee.myPhase != Referee.GamePhase.AbilityDraft && Referee.myPhase != Referee.GamePhase.HeroDraft && ability.GetPlacement() == CardData.FieldPlacement.Mine)
        {
            activateAbility.gameObject.SetActive(true);
            activateAbility.interactable = (ability.Name == "ZOE")? (!ability.oncePerTurnUsed && Referee.myPhase == Referee.GamePhase.HEROSelect) : !ability.oncePerTurnUsed;
            return;
        }
        activateAbility.gameObject.SetActive(false);
    }

    public void Activate()
    {
        Debug.Log("Activating ability with button in IUAbilityDescriptor.");
        activateAbility.interactable = false;
        OnActivateAbility?.Invoke(myAbility);
    }
}
