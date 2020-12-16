using UnityEngine;

public class UIOnOff : MonoBehaviour
{
    public GameObject UIToToggle;

    public void ToggleOnOff()
    {
        UIToToggle.SetActive(!UIToToggle.activeSelf);
    }
}
