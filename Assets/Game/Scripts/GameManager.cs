using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;
using TMPro;
using static DebugGameManager;
using System;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]
    private GameObject loadInfoCanvas;

    [SerializeField]
    private TextMeshProUGUI infoText;

    [SerializeField]
    private TextMeshProUGUI loadInfoText;

    [SerializeField]
    private TextMeshProUGUI timerText;
    [SerializeField]
    private Animator timerAnim;

    [SerializeField]
    private float countDownTime;

    [SerializeField]
    private float stoneSpawnTime;

    public int spawnIndex;

    [SerializeField]
    private List<Vector2> spawnInfos = new List<Vector2>();

    private void Awake()
    {
        // 스폰 위치 셋업
        int startXpos = -80;
        for (int i = 0; i < 10; i++)
        {
            spawnInfos.Add(new Vector2(startXpos + i * 20, -50));
        }
    }

    private void Start()
    {
        PhotonNetwork.LocalPlayer.SetLoad(true);

        spawnIndex = 0;
    }

    public override void OnDisable()
    {
        if (spawnStoneRoutine != null)
        {
            StopCoroutine(spawnStoneRoutine);
            spawnStoneRoutine = null;
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashTable changedProps)
    {
        if (changedProps.ContainsKey(CustomProperty.LOAD))
        {
            int loadCount = PlayerLoadCount();
            // 로딩 완료
            if (loadCount == PhotonNetwork.PlayerList.Length)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.CurrentRoom.SetGameStart(true);
                    PhotonNetwork.CurrentRoom.SetGameStartTime(PhotonNetwork.Time);
                }
            }
            // 로딩 대기
            else
                loadInfoText.text = $"Wait {loadCount} / {PhotonNetwork.PlayerList.Length}";

        }
    }

    // 방의 프로퍼티 설정이 갱신된 경우
    public override void OnRoomPropertiesUpdate(PhotonHashTable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(CustomProperty.GAMESTARTTIME))
        {
            StartCoroutine(StartTimer());
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if(newMasterClient.IsLocal)
            spawnStoneRoutine = StartCoroutine(SpawnStoneRoutine());
    }

    IEnumerator StartTimer()
    {
        double loadTime = PhotonNetwork.CurrentRoom.GetGameStartTime();
        int prevTime = (int)(countDownTime - (PhotonNetwork.Time - loadTime));

        while (PhotonNetwork.Time - loadTime < countDownTime)
        {
            int remainTime = (int)(countDownTime - (PhotonNetwork.Time - loadTime));
            if (prevTime != remainTime)
            {
                prevTime = remainTime;
                timerAnim.SetTrigger("OnEvent");
            }
            infoText.text = (remainTime + 1).ToString();
            yield return null;
        }

        infoText.text = "Game Start!";
        yield return new WaitForSeconds(2.0f);
        loadInfoCanvas.SetActive(false);
        GameStart();
    }

    public void GameStart()
    {
        loadInfoCanvas.SetActive(false);

        Vector3 spawnPos = new Vector3(spawnInfos[spawnIndex].x, 0, spawnInfos[spawnIndex].y);
        GameObject playerInst = PhotonNetwork.Instantiate("Player", spawnPos, Quaternion.identity);

        if(PhotonNetwork.IsMasterClient)
            spawnStoneRoutine = StartCoroutine(SpawnStoneRoutine());
    }

    private int PlayerLoadCount()
    {
        int loadCount = 0;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.GetLoad())
                loadCount++;
        }
        return loadCount;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // == photonView.IsMine 일 때
        {
            stream.SendNext(spawnIndex);
        }
        else  // == stream.IsReading || photonView.IsMine == false 일 때
        {
            spawnIndex = (int)stream.ReceiveNext() + 1;
        }
    }


    // 돌 스폰
    Coroutine spawnStoneRoutine;
    IEnumerator SpawnStoneRoutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(stoneSpawnTime);

            Vector2 dir = UnityEngine.Random.insideUnitCircle.normalized;
            Vector3 pos = new Vector3(dir.x, 0, dir.y) * 200f;

            Vector3 force = -pos.normalized * 30f + new Vector3(UnityEngine.Random.Range(-10f, 10f), 0, UnityEngine.Random.Range(-10f, 10f));
            Vector3 torque = UnityEngine.Random.insideUnitSphere * UnityEngine.Random.Range(1f, 3f);

            object[] instantiateData = { force, torque };

            if(UnityEngine.Random.Range(0,2) < 1)
            {
                PhotonNetwork.InstantiateRoomObject("LargeStone", pos, UnityEngine.Random.rotation, 0, instantiateData);
            }
            else
            {
                PhotonNetwork.InstantiateRoomObject("SmallStone", pos, UnityEngine.Random.rotation, 0, instantiateData);
            }
        }
    }
}
