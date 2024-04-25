using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class MainPanel : MonoBehaviour
{
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject createRoomPanel;
    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_InputField maxPlayerInputField;

    private void OnEnable()
    {
        createRoomPanel.SetActive(false);
    }

    public void CreateRoomMenu()
    {
        createRoomPanel.SetActive(true);
    }

    public void CreateRoomConfirm()
    {
        string roomName = roomNameInputField.text.Length < 1 ?
            $"Room {Random.Range(100, 1000)}" :
            roomNameInputField.text;

        int maxPlayer = maxPlayerInputField.text.Length < 1 ? 1 : int.Parse(maxPlayerInputField.text);
        maxPlayer = Mathf.Clamp(maxPlayer, 1, 8);

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = maxPlayer;

        PhotonNetwork.CreateRoom(roomName, options);
    }

    public void CreateRoomCancel()
    {

    }

    public void RandomMatching()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void JoinLobby()
    {

    }

    public void Logout()
    {
        PhotonNetwork.Disconnect();
    }
}
