using UnityEngine;
using TMPro;

public class UIError : MonoBehaviour
{
    [SerializeField] private TMP_Text errorTitle;
    [SerializeField] private TMP_Text errorBody;
    [SerializeField] private GameObject ErrorUI;

    private void Awake()
    {
        PhotonChatController.OnError += HandleErrors;
        PlayfabFriendController.OnError += HandleErrors;
    }

    private void OnDestroy()
    {
        PhotonChatController.OnError -= HandleErrors;
        PlayfabFriendController.OnError -= HandleErrors;
    }

    private void HandleErrors(string title, string body)
    {
        errorTitle.text = title;
        errorBody.text = body;
        ErrorUI.SetActive(true);
    }
}
