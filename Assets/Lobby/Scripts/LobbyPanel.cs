using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class LobbyPanel : MonoBehaviour
{
    [SerializeField] RectTransform roomContent;
    [SerializeField] RoomEntry roomEntryPrefab;

    private Dictionary<string, RoomEntry> roomDictionary;

    private void Awake()
    {
        roomDictionary = new Dictionary<string, RoomEntry>();
    }
    private void OnDisable()
    {
        for(int i =0; i< roomContent.childCount; i++)
        {
            Destroy(roomContent.GetChild(i).gameObject);
        }
        roomDictionary.Clear();
    }

    public void LeaveLobby()
    {
        PhotonNetwork.LeaveLobby();
    }

    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        Debug.Log("Update room info");

        foreach(RoomInfo roomInfo in roomList)
        {
            // 1. 방이 사라지는 경우
            if(roomInfo.RemovedFromList || !roomInfo.IsOpen || !roomInfo.IsVisible)
            {
                if (!roomDictionary.ContainsKey(roomInfo.Name))
                    return;

                RoomEntry roomEntry = roomDictionary[roomInfo.Name];
                roomDictionary.Remove(roomInfo.Name);
                Destroy(roomEntry.gameObject);
            }
            // 2. 방의 내용물이 바뀌는 경우
            else if (roomDictionary.ContainsKey(roomInfo.Name))
            {
                RoomEntry roomEntry = roomDictionary[roomInfo.Name];
                roomEntry.SetRoomInfo(roomInfo);
            }
            // 3. 방이 생기는 경우
            else
            {
                RoomEntry roomEntry = Instantiate(roomEntryPrefab, roomContent);
                roomEntry.SetRoomInfo(roomInfo);
                roomDictionary.Add(roomInfo.Name, roomEntry);
            }
        }
    }
}
