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
        // RoomEntry의 RoomInfo를 할당
        this.roomInfo = roomInfo;
        // 방 이름
        roomName.text = roomInfo.Name;
        // 방에 참여한 플레이어 수
        currentPlayer.text = $"{roomInfo.PlayerCount} / {roomInfo.MaxPlayers}";
        // Join 버튼 활성화 여부
        joinRoomButton.interactable = roomInfo.PlayerCount < roomInfo.MaxPlayers;
    }

    public void JoinRoom()
    {
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.JoinRoom(roomInfo.Name);
    }
}
