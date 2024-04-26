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

        // �÷��̾��� Ready ���¸� false�� ����
        PhotonNetwork.LocalPlayer.SetReady(false);
        PhotonNetwork.LocalPlayer.SetLoad(false);

        AllPlayerReadyCheck();

        // AutomaticallySyncScene ������ ���� ����� �� ���� �̵��ϵ���
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

    // ���� ����
    public void StartGame()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }

    // ���� ����
    public void MasterClientSwitched(Player newMasterClient)
    {
        startButton.gameObject.SetActive(newMasterClient.IsLocal);
        AllPlayerReadyCheck();
    }

    // �÷��̾� ���� ����
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

    // �÷��̾� ����
    public void PlayerEnterRoom(Player newPlayer)
    {
        PlayerEntry playerEntry = Instantiate(playerEntryPrefab, playerContent);
        playerEntry.SetPlayer(newPlayer);
        playerList.Add(playerEntry);

        AllPlayerReadyCheck();
    }

    // �÷��̾� ����
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

    // �泪���� 
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    // ��� �÷��̾��� �غ���� üũ
    public void AllPlayerReadyCheck()
    {
        // ������ �ƴ� ��� return
        if (!PhotonNetwork.IsMasterClient) return;

        int readyCount = 0;

        foreach(Player player in PhotonNetwork.PlayerList)
        {
            // Ready ��ư�� ���� �÷��̾� ī��Ʈ
            if (player.GetReady())
                readyCount++;
        }

        // ���۹�ư Ȱ��ȭ : ������ �÷��̾ ��� �غ� �� ������ ���
        startButton.interactable = readyCount == PhotonNetwork.PlayerList.Length;
    }
}
