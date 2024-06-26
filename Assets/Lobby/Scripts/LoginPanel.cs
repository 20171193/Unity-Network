using TMPro;
using UnityEngine;
using Photon.Pun;

public class LoginPanel : MonoBehaviour
{
    [SerializeField] TMP_InputField idInputField;

    private void Start()
    {
        // 기본값 할당
        idInputField.text = $"Player {Random.Range(1000, 10000)}";
    }

    public void Login()
    {
        if (idInputField.text == "")
        {
            Debug.LogError("Empty nickname : Please input name");
            return;
        }

        PhotonNetwork.LocalPlayer.NickName = idInputField.text;
        PhotonNetwork.ConnectUsingSettings();
    }
}
