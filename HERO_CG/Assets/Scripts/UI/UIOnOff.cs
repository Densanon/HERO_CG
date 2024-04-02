using UnityEngine;
using System;

public class UIOnOff : MonoBehaviour
{
    public static Action OnUpdateUI = delegate { };

    public GameObject UIToToggle;

    public void ToggleOnOff()
    {
        UIToToggle.SetActive(!UIToToggle.activeSelf);
        if (UIToToggle.activeSelf) OnUpdateUI?.Invoke();
    }
}
