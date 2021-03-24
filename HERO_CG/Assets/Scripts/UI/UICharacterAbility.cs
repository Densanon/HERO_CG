using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class UICharacterAbility : MonoBehaviour
{
    public Ability myAbility;
    public TMP_Text nameText;
    public Image abilityIcon;

    public static Action<Ability> OnAbilityDescriptionPressed = delegate { };

    private void Awake()
    {
        nameText.text = myAbility.Name;
        AssignAbilityIcon();
    }

    public void PressDescription()
    {
        OnAbilityDescriptionPressed?.Invoke(myAbility);
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
        }
    }
}
