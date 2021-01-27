using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIConfirmation : MonoBehaviour
{
    public GameObject confirmationUI;
    public TMP_Text confirmationText;
    public enum ConfirmationType { Leave, Action}
    public ConfirmationType curType;

    private void Awake()
    {
        
    }

    private void OnDestroy()
    {
        
    }

    private void OnHandleConfirmation(ConfirmationType type)
    {
        confirmationUI.SetActive(true);
        curType = type;
        switch (type)
        {
            case ConfirmationType.Action:
                confirmationText.text = "";
                break;
            case ConfirmationType.Leave:
                confirmationText.text = "";
                break;
        }
    }

    public void Accept()
    {
        //do something with curType
        confirmationUI.SetActive(false);
    }

    public void Decline()
    {
        confirmationUI.SetActive(false);
    }
}
