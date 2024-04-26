using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;

public class RoomPanel : MonoBehaviour
{
    [SerializeField] RectTransform playerContent;
    [SerializeField] PlayerEntry playerEntryPrefab;
    [SerializeField] Button startButton;

    private List<PlayerEntry> playerList;

    private void Awake()
    {
        playerList = new List<PlayerEntry>();
    }

    private void OnEnable()
    {
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            PlayerEntry playerEntry = Instantiate(playerEntryPrefab, playerContent);
            playerEntry.SetPlayer(player);
            playerList.Add(playerEntry);
        }

        startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);

        // 플레이어의 Ready 상태를 false로 시작
        PhotonNetwork.LocalPlayer.SetReady(false);
        PhotonNetwork.LocalPlayer.SetLoad(false);

        AllPlayerReadyCheck();

        // AutomaticallySyncScene 방장의 씬이 변경될 때 같이 이동하도록
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void OnDisable()
    {
        for (int i = 0; i < playerContent.childCount; i++)
        {
            Destroy(playerContent.GetChild(i).gameObject);
        }

        playerList.Clear();

        PhotonNetwork.AutomaticallySyncScene = false;
    }

    // 게임 시작
    public void StartGame()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }

    // 방장 변경
    public void MasterClientSwitched(Player newMasterClient)
    {
        startButton.gameObject.SetActive(newMasterClient.IsLocal);
        AllPlayerReadyCheck();
    }

    // 플레이어 상태 변경
    public void PlayerPropertyUpdate(Player targetPlayer, PhotonHashTable changedProps)
    {
        PlayerEntry playerEntry = null;
        foreach(PlayerEntry entry in playerList)
        {
            if(entry.Player.ActorNumber == targetPlayer.ActorNumber)
            {
                playerEntry = entry;
            }
        }
        playerEntry.ChangeCustomProperty(changedProps);
    }

    // 플레이어 입장
    public void PlayerEnterRoom(Player newPlayer)
    {
        PlayerEntry playerEntry = Instantiate(playerEntryPrefab, playerContent);
        playerEntry.SetPlayer(newPlayer);
        playerList.Add(playerEntry);

        AllPlayerReadyCheck();
    }

    // 플레이어 퇴장
    public void PlayerLeftRoom(Player otherPlayer)
    {
        PlayerEntry playerEntry = null;
        foreach(PlayerEntry entry in playerList)
        {
            if(entry.Player.ActorNumber == otherPlayer.ActorNumber)
            {
                playerEntry = entry;
            }
        }
        Destroy(playerEntry.gameObject);
        playerList.Remove(playerEntry);

        AllPlayerReadyCheck();
    }

    // 방나가기 
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    // 모든 플레이어의 준비상태 체크
    public void AllPlayerReadyCheck()
    {
        // 방장이 아닌 경우 return
        if (!PhotonNetwork.IsMasterClient) return;

        int readyCount = 0;

        foreach(Player player in PhotonNetwork.PlayerList)
        {
            // Ready 버튼을 누른 플레이어 카운트
            if (player.GetReady())
                readyCount++;
        }

        // 시작버튼 활성화 : 입장한 플레이어가 모두 준비가 된 상태일 경우
        startButton.interactable = readyCount == PhotonNetwork.PlayerList.Length;
    }
}
