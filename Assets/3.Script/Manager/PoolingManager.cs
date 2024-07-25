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
        Debug.Log("�Ѿ˸����µ���");
        Debug.Log(bulletList[0].name);
        Debug.Log(bulletCount);
        for (int i = 0; i < bulletCount; i++)
        {
            Debug.Log("for������");
            // �Ѿ� �������� �����մϴ�. (���⼭�� �������� ���������� �ʿ信 ���� ���� ����)
            GameObject bulletPrefab = bulletList[bulletNum];
            // �Ѿ� �������� �ν��Ͻ�ȭ�ϰ� �ʱ�ȭ�մϴ�.
            GameObject bulletObject = Instantiate(bulletPrefab);
            bulletObject.SetActive(false); // ��Ȱ��ȭ�Ͽ� Ǯ�� �߰�

            // Bullet ������Ʈ�� �����ͼ� ť�� �߰��մϴ�.
            Bullet bullet = bulletObject.GetComponent<Bullet>();
            Debug.Log("�Ѿ˸���");

            if (bullet != null)
            {
                bulletPool.Enqueue(bullet);
            }
        }
        Debug.Log(bulletPool.Count);
    }

}
