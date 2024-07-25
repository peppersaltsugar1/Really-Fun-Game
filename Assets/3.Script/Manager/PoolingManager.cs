using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : MonoBehaviour
{
    public GameObject[] bulletList;
    public GameObject[] monsterList;
    public int bulletCount;
    public int monsterCount = 50;
    public Queue<Bullet> bulletPool = new Queue<Bullet>();
    public Transform shotPoint;

    void Start()
    {
        BulletMake(0);
    }

    void Update()
    {

    }

    public void BulletMake(int bulletNum)
    {
        Debug.Log("총알만들기는들어옴");
        Debug.Log(bulletList[0].name);
        Debug.Log(bulletCount);
        for (int i = 0; i < bulletCount; i++)
        {
            Debug.Log("for문들어옴");
            // 총알 프리팹을 선택합니다. (여기서는 랜덤으로 선택하지만 필요에 따라 수정 가능)
            GameObject bulletPrefab = bulletList[bulletNum];
            // 총알 프리팹을 인스턴스화하고 초기화합니다.
            GameObject bulletObject = Instantiate(bulletPrefab);
            bulletObject.SetActive(false); // 비활성화하여 풀에 추가

            // Bullet 컴포넌트를 가져와서 큐에 추가합니다.
            Bullet bullet = bulletObject.GetComponent<Bullet>();
            Debug.Log("총알만듬");

            if (bullet != null)
            {
                bulletPool.Enqueue(bullet);
            }
        }
        Debug.Log(bulletPool.Count);
    }

}
