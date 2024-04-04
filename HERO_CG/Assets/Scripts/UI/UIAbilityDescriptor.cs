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
    public bool Copy;

    public static Action<Ability> OnActivateAbility = delegate { };

    private void Awake()
    {
        UICharacterAbility.OnAbilityDescriptionPressed += HandleDescriptionInquery;
    }

    private void OnDestroy()
    {
        UICharacterAbility.OnAbilityDescriptionPressed -= HandleDescriptionInquery;
    }

    private void HandleDescriptionInquery(Ability ability,bool copy)
    {
        if (!Copy && !copy)
        {
            TurnOnUniversal(ability);
            //Debug.Log("Getting inquery.");
            if (Referee.myTurn && (ability.myType == Ability.Type.Activate || ability.secondaryType == Ability.Type.Activate) && ability.canActivate && Referee.myPhase != Referee.GamePhase.AbilityDraft && Referee.myPhase != Referee.GamePhase.HeroDraft && ability.GetPlacement() == CardData.FieldPlacement.Mine)
            {
                //Debug.Log("Should be activated.");
                activateAbility.gameObject.SetActive(true);
                activateAbility.interactable = (ability.Name == "ZOE") ? (!ability.oncePerTurnUsed && Referee.myPhase == Referee.GamePhase.HEROSelect) : !ability.oncePerTurnUsed;
                return;
            }
            activateAbility.gameObject.SetActive(false);
        }
        else if(Copy && copy)
        {
            TurnOnUniversal(ability);
            activateAbility.gameObject.SetActive(true);
        }
    }

    private void TurnOnUniversal(Ability ability)
    {
        myAbility = ability;
        descriptor.SetActive(true);
        title.text = ability.Name;
        description.text = ability.Description;
    }

    public void Activate()
    {
        //Debug.Log("Activating ability with button in IUAbilityDescriptor.");
        activateAbility.interactable = false;
        OnActivateAbility?.Invoke(myAbility);
    }
}
