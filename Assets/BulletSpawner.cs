using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    private static BulletSpawner inst;
    public static BulletSpawner Inst { get { return inst; } }

    [SerializeField]
    private Bullet bulletPrefab;

    [SerializeField]
    private int size;
    [SerializeField]
    private int capacity;

    private Stack<Bullet> bulletPool = new Stack<Bullet>();

    private void Awake()
    {
        inst = this;

        GameObject pooler = new GameObject("Pool_Bullet");
        for(int i =0; i<size; i++)
        {
            Bullet inst = Instantiate(bulletPrefab, pooler.transform);
            inst.pooler = this;
            inst.gameObject.SetActive(false);
            bulletPool.Push(inst);
        }
    }

    public Bullet GetPool(Vector3 pos, Quaternion rotation)
    {
        Bullet inst = null;
        if (bulletPool.Count < 1)
        {
            inst = Instantiate(bulletPrefab, pos, rotation);
            inst.pooler = this;
            inst.gameObject.SetActive(true);
            return inst;
        }
        else
        {
            inst = bulletPool.Pop();
            inst.gameObject.SetActive(true);
            inst.transform.position = pos;
            inst.transform.rotation = rotation;
            return inst;
        }
    }
    public void ReturnPool(Bullet bullet)
    {
        if (bulletPool.Count >= capacity)
            Destroy(bullet.gameObject);
        else
            bulletPool.Push(bullet);
    }
}
