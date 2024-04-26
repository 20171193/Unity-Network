using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomEntry : MonoBehaviour
{
    [SerializeField] TMP_Text roomName;
    [SerializeField] TMP_Text currentPlayer;
    [SerializeField] Button joinRoomButton;

    private RoomInfo roomInfo;

    private void OnEnable()
    {
        
    }
    public void SetRoomInfo(RoomInfo roomInfo)
    {
        // RoomEntry�� RoomInfo�� �Ҵ�
        this.roomInfo = roomInfo;
        // �� �̸�
        roomName.text = roomInfo.Name;
        // �濡 ������ �÷��̾� ��
        currentPlayer.text = $"{roomInfo.PlayerCount} / {roomInfo.MaxPlayers}";
        // Join ��ư Ȱ��ȭ ����
        joinRoomButton.interactable = roomInfo.PlayerCount < roomInfo.MaxPlayers;
    }

    public void JoinRoom()
    {
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.JoinRoom(roomInfo.Name);
    }
}
