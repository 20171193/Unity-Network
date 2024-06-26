using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Stone : MonoBehaviourPun
{
    [SerializeField]
    private Rigidbody rigid;

    private void Awake()
    {
        if(photonView.InstantiationData != null)
        {
            Vector3 force = (Vector3)photonView.InstantiationData[0];
            Vector3 torque = (Vector3)photonView.InstantiationData[1];

            rigid.AddForce(force, ForceMode.Impulse);
            rigid.AddForce(torque, ForceMode.Impulse);
        }
    }

    private void Update()
    {
        if (photonView.IsMine == false)
            return;

        if(transform.position.sqrMagnitude > 40000)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }


    // Room Object의 처리는 사실 상 서버처리로 볼 수 있음. (모든 클라이언트에 동기화)

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            //if (photonView.IsMine == false)
            //    return;

            other.GetComponent<PlayerController>().TakeDamage();
        }
    }
}
