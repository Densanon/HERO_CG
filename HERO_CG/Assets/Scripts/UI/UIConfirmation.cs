using UnityEngine;
using TMPro;
using System;

public class UIConfirmation : MonoBehaviour
{
    public GameObject confirmationUI;
    public TMP_Text confirmationText;
    public enum Confirmation { Heal, Enhance, Recruit, Overcome, Feat, Quit, Enhancing, Ability, AtDef}
    private Confirmation typeOfConfirmation = Confirmation.Heal;
    private Card myCardToUse;
    private CardData myTargetCard;

    public static Action<PhotonGameManager.GamePhase> OnHEROSelection = delegate { };
    public static Action<CardData, Card> OnTargetAccepted = delegate { };

    private void Awake()
    {
        CardDataBase.OnTargeting += HandleTargeting;
        CardData.IsTarget += HandleTarget;

    }

    private void OnDestroy()
    {
        CardDataBase.OnTargeting -= HandleTargeting;
        CardData.IsTarget -= HandleTarget;
    }

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

    public void Accept()
    {
        Debug.Log($"Accepting {typeOfConfirmation}");
        switch (typeOfConfirmation)
        {
            case Confirmation.Heal:
                OnHEROSelection?.Invoke(PhotonGameManager.GamePhase.Heal);
                break;
            case Confirmation.Enhance:
                Debug.Log("Sending Enhance Action");
                OnHEROSelection?.Invoke(PhotonGameManager.GamePhase.Enhance);
                break;
            case Confirmation.Recruit:
                OnHEROSelection?.Invoke(PhotonGameManager.GamePhase.Recruit);
                break;
            case Confirmation.Overcome:
                OnHEROSelection?.Invoke(PhotonGameManager.GamePhase.Overcome);
                break;
            case Confirmation.Feat:
                OnHEROSelection?.Invoke(PhotonGameManager.GamePhase.Feat);
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
        }
        confirmationUI.SetActive(false);
    }

    public void Decline()
    {
        confirmationUI.SetActive(false);
    }
}
