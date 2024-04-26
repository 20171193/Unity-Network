using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using static DebugGameManager;
using System;
using UnityEditor;

// 테스트용 게임매니저
public class DebugGameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] string debugRoomName = "DebugRoom";
    [SerializeField]
    private PlayerController playerPrefab;

    [SerializeField]
    private GameObject loadInfoCanvas;

    [Serializable]
    public struct SpawnInfo
    {
        public Vector2 spawnPos;
        public bool isAssign;
        public SpawnInfo(Vector2 spawnPos, bool isAssign)
        {
            this.spawnPos = spawnPos;
            this.isAssign = isAssign;
        }
    }

    [SerializeField]
    private List<SpawnInfo> spawnInfos = new List<SpawnInfo>();

    private void Awake()
    {
        // 스폰 위치 셋업
        int startXpos = -80;
        for(int i =0; i<10; i++)
        {
            SpawnInfo info = new SpawnInfo(new Vector2(startXpos + i*20, -50), false);
            spawnInfos.Add(info);
        }
    }

    private void Start()
    {
        PhotonNetwork.LocalPlayer.NickName = "DebugPlayer";
        PhotonNetwork.ConnectUsingSettings();
    }

    // 마스터서버로 접속
    public override void OnConnectedToMaster()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 8;
        options.IsVisible = false;
        TypedLobby typedLobby = new TypedLobby("DebugLobby", LobbyType.Default);

        PhotonNetwork.JoinOrCreateRoom(debugRoomName, options, typedLobby);
    }

    public override void OnJoinedRoom()
    {
        // 룸에 입장한 경우 바로 시작
        StartCoroutine(GameStartDelay());
    }

    public void GameStart()
    {
        Debug.Log("Debug GameStart");
        loadInfoCanvas.SetActive(false);

        for(int i =0; i< spawnInfos.Count; i++)
        {
            if (spawnInfos[i].isAssign) 
                continue;

            // 할당 체크
            spawnInfos[i] = new SpawnInfo(spawnInfos[i].spawnPos, true);

            Vector3 spawnPos = new Vector3(spawnInfos[i].spawnPos.x, 0, spawnInfos[i].spawnPos.y);
            GameObject playerInst = PhotonNetwork.Instantiate("Player", spawnPos, Quaternion.identity);
            break;
        }
    }

    IEnumerator GameStartDelay()
    {
        // 네트워크 셋업
        yield return new WaitForSeconds(1f);
        GameStart();
    }
}
