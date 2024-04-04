using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections;

public class UICharacterAbility : MonoBehaviour
{
    public Ability myAbility;
    public TMP_Text nameText;
    public Image abilityIcon;
    public Button activateButton;

    public static Action<Ability,bool> OnAbilityDescriptionPressed = delegate { };

    public void Awake()
    {
        Referee.OnTurnResetables += ResetAbilityInteractible;
    }

    public void OnDestroy()
    {
        Referee.OnTurnResetables -= ResetAbilityInteractible;
    }

    bool Copy = false;
    public void AbilityAwake(Ability ability,bool copy)
    {
        myAbility = ability;
        nameText.text = myAbility.Name;
        AssignAbilityIcon();
        activateButton = abilityIcon.gameObject.GetComponent<Button>();
        Copy = copy;
    }

    public void PressDescription()
    {
        OnAbilityDescriptionPressed?.Invoke(myAbility,Copy);
    }

    public void PressedActivateAbility()
    {
        if(myAbility.myType == Ability.Type.Activate || myAbility.secondaryType == Ability.Type.Activate)
            myAbility.ActivateAbility();
    }

    private void ResetAbilityInteractible()
    {
        if(myAbility.myType == Ability.Type.Activate)
        {
            activateButton.interactable = true;
        }
    }

    private void AssignAbilityIcon()
    {
        Ability.Type type = myAbility.myType;
        switch (type)
        {
            case Ability.Type.Activate:
                abilityIcon.color = Color.green;
                break;
            case Ability.Type.Feat:
                abilityIcon.color = Color.red;
                break;
            case Ability.Type.Passive:
                abilityIcon.color = Color.blue;
                break;
            case Ability.Type.Character:
                abilityIcon.color = Color.black;
                break;
        }
    }

    private IEnumerator activateOnOff()
    {
        yield return new WaitForSeconds(1f);
        if(myAbility.myType == Ability.Type.Activate || myAbility.secondaryType == Ability.Type.Activate)
        {
            activateButton.interactable = true;
            ColorBlock color = activateButton.colors;
            color.disabledColor = Color.grey;
            activateButton.colors = color;
        }
    }
}
