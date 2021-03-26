using UnityEngine;
using TMPro;

public class UIAbilityDescriptor : MonoBehaviour
{
    public TMP_Text title;
    public TMP_Text description;
    public GameObject descriptor;

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
        descriptor.SetActive(true);
        title.text = ability.Name;
        description.text = ability.Description;
    }
}
