using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Bullet : MonoBehaviourPun
{
    [SerializeField]
    private Rigidbody rigid;

    [SerializeField]
    private float movePower;
    public BulletSpawner pooler;

    private Coroutine releaseRoutine;

    [SerializeField]
    public Vector3 Velocity { get { return rigid.velocity; } } 

    public void OnShoot()
    {
        rigid.velocity = transform.forward * movePower;
        releaseRoutine = StartCoroutine(ReleaseRoutine());
    }

    private void Release()
    {
        if (releaseRoutine != null)
        {
            StopCoroutine(releaseRoutine);
            releaseRoutine = null;
        }
        
        // ฟ๘บน
        rigid.velocity = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;
        transform.parent = pooler.transform;

        pooler.ReturnPool(this);
    }

    IEnumerator ReleaseRoutine()
    {
        yield return new WaitForSeconds(2.0f);
        Release();
        releaseRoutine = null;
    }
}
