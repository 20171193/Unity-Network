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
        string _roomName = roomNameInputField.text.Length < 1 ?
            $"Room {Random.Range(100, 1000)}" :
            roomNameInputField.text;

        int maxPlayer = maxPlayerInputField.text.Length < 1 ? 1 : int.Parse(maxPlayerInputField.text);
        maxPlayer = Mathf.Clamp(maxPlayer, 1, 8);

        RoomOptions options = new RoomOptions() { MaxPlayers = maxPlayer };

        PhotonNetwork.CreateRoom(roomName : _roomName, roomOptions : options);
    }

    public void CreateRoomCancel()
    {

    }

    public void RandomMatching()
    {
        // PhotonNetwork.JoinRandomRoom(); ----> 비어있는 방 찾기, 없으면 OnJoinRandomRoomFailed

        string _roomName = $"Room {Random.Range(100, 1000)}";
        RoomOptions options = new RoomOptions() { MaxPlayers = 8 };
        PhotonNetwork.JoinRandomOrCreateRoom(roomName : _roomName, roomOptions : options);
    }

    public void JoinLobby()
    {
        PhotonNetwork.JoinLobby();
    }

    public void Logout()
    {
        PhotonNetwork.Disconnect();
    }
}
