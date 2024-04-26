using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// IPunObservable : 변수 동기화
public class PlayerController : MonoBehaviourPun, IPunObservable
{
    [SerializeField]
    private PlayerInput input;

    [SerializeField]
    private GameObject model;

    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private int fireCount;
    [SerializeField]
    private List<Color> colorList = new List<Color>();

    private Vector3 moveDir;
    private Coroutine fireDelay;

    private float rotZ;
    private void Awake()
    {
        // 포톤 뷰 설정 (내 시점에서 다른 플레이어 입력무시)
        if (!photonView.IsMine)
        {
            Destroy(input);
        }

        SetPlayerColor();
    }

    private void Update()
    {
        transform.Translate(moveDir * moveSpeed * Time.deltaTime);   
    }

    private void LateUpdate()
    {
        
    }

    private void OnMove(InputValue value)
    {
        Vector2 inputDir = value.Get<Vector2>();
        moveDir.x = inputDir.x;
        moveDir.z = inputDir.y;
        if (moveDir.x == 0f) rotZ = 0;
        else if (moveDir.x < -0.1f) rotZ = 45;
        else if (moveDir.x > 0.1f) rotZ = -45;
    }

    private void OnFire(InputValue value)
    {
        if(value.isPressed)
        {
            if (fireDelay != null)
                StopCoroutine(fireDelay);

            fireDelay = StartCoroutine(FireDelayTimer());
        }
        else
        {
            StopCoroutine(fireDelay);
            fireDelay = null;
        }
    }

    private void Fire()
    {
        fireCount++;
    }

    IEnumerator FireDelayTimer()
    {
        while(true)
        {
            Fire();
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void SetPlayerColor()
    {
        int playerNumber = photonView.Owner.GetPlayerNumber();

        if (colorList == null || colorList.Count <= playerNumber) return;

        Renderer render = model.GetComponent<Renderer>();
        render.material.color = photonView.IsMine ? Color.green : colorList[playerNumber];
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting) // == photonView.IsMine 일 때
        {
            stream.SendNext(fireCount);
            stream.SendNext(rotZ);
        }
        else  // == stream.IsReading || photonView.IsMine == false 일 때
        {
            fireCount = (int)stream.ReceiveNext();
            rotZ = (float)stream.ReceiveNext();
            model.transform.rotation = Quaternion.Euler(0, 0, rotZ);
        }
    }
}
