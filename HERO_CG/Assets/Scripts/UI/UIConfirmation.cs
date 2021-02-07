using UnityEngine;
using TMPro;
using System;

public class UIConfirmation : MonoBehaviour
{
    public GameObject confirmationUI;
    public TMP_Text confirmationText;
    public enum Confirmation { Heal, Enhance, Recruit, Overcome, Feat, Quit}
    private Confirmation typeOfConfirmation = Confirmation.Heal;

    public static Action<PhotonGameManager.GamePhase> OnHEROSelection = delegate { };

    public void OnConfirmationRequest(string type)
    {
        confirmationUI.SetActive(true);
        switch (type)
        {
            case "Heal":
                confirmationText.text = "Confirm you selected 'Heal'";
                typeOfConfirmation = Confirmation.Heal;
                break;
            case "Enhance":
                confirmationText.text = "Confirm you selected 'Enhance'";
                typeOfConfirmation = Confirmation.Enhance;
                break;
            case "Recruit":
                confirmationText.text = "Confirm you selected 'Recruit'";
                typeOfConfirmation = Confirmation.Recruit;
                break;
            case "Overcome":
                confirmationText.text = "Confirm you selected 'Overcome'";
                typeOfConfirmation = Confirmation.Overcome;
                break;
            case "Feat":
                confirmationText.text = "Confirm you selected 'Feat'";
                typeOfConfirmation = Confirmation.Feat;
                break;
            case "Leave":
                confirmationText.text = "Confirm you selected 'Quit'";
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
        }
        confirmationUI.SetActive(false);
    }

    public void Decline()
    {
        confirmationUI.SetActive(false);
    }
}
