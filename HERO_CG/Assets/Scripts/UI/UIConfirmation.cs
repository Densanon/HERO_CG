//Created by Jordan Ezell
//Last Edited: 1/6/23 Jordan

using UnityEngine;
using TMPro;
using System;

public class UIConfirmation : MonoBehaviour
{
    public GameObject confirmationUI;
    public TMP_Text confirmationText;
    public enum Confirmation { Heal, Enhance, Recruit, Overcome, Feat, Quit, Enhancing, Ability, AtDef, Ayumi}
    private Confirmation typeOfConfirmation = Confirmation.Heal;
    private Card myCardToUse;
    private CardData myTargetCard;

    public static Action<Referee.GamePhase> OnHEROSelection = delegate { };
    public static Action<CardData, Card> OnTargetAccepted = delegate { };
    public static Action<int> OnNeedDrawEnhanceCards = delegate { };

    #region Unity Methods
    private void Awake()
    {
        CardDataBase.OnTargeting += HandleTargeting;
        CardData.IsTarget += HandleTarget;
        Ability.OnConfirmDrawEnhanceCard += OnConfirmationRequest;
    }
    private void OnDestroy()
    {
        CardDataBase.OnTargeting -= HandleTargeting;
        CardData.IsTarget -= HandleTarget;
    }
    #endregion

    private void HandleTargeting(Card cardToBePlayed, bool target)
    {
        if (target)
        {
            myCardToUse = cardToBePlayed;
            Card.Type type = cardToBePlayed.CardType;
            switch (type)
            {
                case Card.Type.Ability:
                    confirmationText.text = "Confirm Ability to target.";
                    typeOfConfirmation = Confirmation.Ability;
                    break;
                case Card.Type.Enhancement:
                    confirmationText.text = "Confirm Enhancement to target.";
                    typeOfConfirmation = Confirmation.Enhancing;
                    break;
            }
        }
    }
    private void HandleTarget(CardData card)
    {
        myTargetCard = card;
        confirmationUI.SetActive(true);
    }
    public void OnConfirmationRequest(string type)
    {
        confirmationUI.SetActive(true);
        switch (type)
        {
            case "Heal":
                confirmationText.text = "Confirm 'Heal'";
                typeOfConfirmation = Confirmation.Heal;
                break;
            case "Enhance":
                confirmationText.text = "Confirm 'Enhance'";
                typeOfConfirmation = Confirmation.Enhance;
                break;
            case "Recruit":
                confirmationText.text = "Confirm 'Recruit'";
                typeOfConfirmation = Confirmation.Recruit;
                break;
            case "Overcome":
                confirmationText.text = "Confirm 'Overcome'";
                typeOfConfirmation = Confirmation.Overcome;
                break;
            case "Feat":
                confirmationText.text = "Confirm 'Feat'";
                typeOfConfirmation = Confirmation.Feat;
                break;
            case "Leave":
                confirmationText.text = "Confirm 'Quit'";
                typeOfConfirmation = Confirmation.Quit;
                break;
        }
    }
    void OnConfirmationRequest(string type, int amount)
    {
        Debug.Log($"Got the {type} message.");
        confirmationUI.SetActive(true);
        switch (type)
        {
            case "Ayumi":
                confirmationText.text = "Confirm: Ayumi's Draw an Enhance Card";
                typeOfConfirmation = Confirmation.Ayumi;
                break;
            default:
                Debug.Log($"UI Confirmation was given a request of type: {type} and doesn't use it.");
                break;
        }
    }

    public void Accept()
    {
        //Debug.Log($"Accepting {typeOfConfirmation}");
        switch (typeOfConfirmation)
        {
            case Confirmation.Heal:
                OnHEROSelection?.Invoke(Referee.GamePhase.Heal);
                break;
            case Confirmation.Enhance:
                //Debug.Log("Sending Enhance Action");
                OnHEROSelection?.Invoke(Referee.GamePhase.Enhance);
                break;
            case Confirmation.Recruit:
                OnHEROSelection?.Invoke(Referee.GamePhase.Recruit);
                break;
            case Confirmation.Overcome:
                OnHEROSelection?.Invoke(Referee.GamePhase.Overcome);
                break;
            case Confirmation.Feat:
                OnHEROSelection?.Invoke(Referee.GamePhase.Feat);
                break;
            case Confirmation.Quit:
                break;
            case Confirmation.Ability:
                OnTargetAccepted?.Invoke(myTargetCard, myCardToUse);
                break;
            case Confirmation.Enhancing:
                OnTargetAccepted?.Invoke(myTargetCard, myCardToUse);
                break;
            case Confirmation.AtDef:
                OnTargetAccepted?.Invoke(myTargetCard, null);
                break;
            case Confirmation.Ayumi:
                OnNeedDrawEnhanceCards(1);
                break;
        }
        confirmationUI.SetActive(false);
    }

    public void Decline()
    {
        confirmationUI.SetActive(false);
    }
}
