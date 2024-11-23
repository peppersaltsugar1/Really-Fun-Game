using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : MonoBehaviour
{
    private static PoolingManager instance = null;
    public GameObject[] bulletList;
    public GameObject[] monsterList;
    public GameObject bulletBox;
    public int bulletCount;
    public int monsterCount = 50;
    public Queue<Bullet> bulletPool = new Queue<Bullet>();
    public Transform shotPoint;


    public static PoolingManager Instance
    {
        get
        {
            if (instance == null)
            {
                // �̱��� �ν��Ͻ��� ������ ���ο� �ν��Ͻ��� ã�ų� ����
                instance = FindObjectOfType<PoolingManager>();

                if (instance == null)
                {
                    GameObject singleton = new GameObject("PoolingManager");
                    instance = singleton.AddComponent<PoolingManager>();
                    DontDestroyOnLoad(singleton);
                }
            }
            return instance;
        }
    }
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        BulletMake(0);
    }

    
    public void BulletMake(int bulletNum)
    {
        for (int i = 0; i < bulletCount; i++)
        {
            // �Ѿ� �������� �����մϴ�. (���⼭�� �������� ���������� �ʿ信 ���� ���� ����)
            GameObject bulletPrefab = bulletList[bulletNum];
            GameObject bulletObject = Instantiate(bulletPrefab);
            bulletObject.SetActive(false); // ��Ȱ��ȭ�Ͽ� Ǯ�� �߰�

            // Bullet ������Ʈ�� �����ͼ� ť�� �߰��մϴ�.
            Bullet bullet = bulletObject.GetComponent<Bullet>();

            if (bullet != null)
            {
                bulletPool.Enqueue(bullet);
            }
            bulletObject.transform.SetParent(bulletBox.transform, false);
            // ������ Map ������Ʈ�� map�� �ڽ� ��Ͽ��� ���������� ��ġ��Ű��
            bulletObject.transform.SetAsLastSibling();

        }
    }
    public void ReMakeBullet(int bulletIndex) 
    {
        // ���� �Ѿ� ����
        while (bulletPool.Count > 0)
        {
            Bullet bullet = bulletPool.Dequeue(); // ť���� �Ѿ��� ����
            if (bullet != null && bullet.gameObject != null)
            {
                Destroy(bullet.gameObject); // ���� ������Ʈ ����
            }
        }

        // ť �����, �Ѿ� ���� ����
        BulletMake(bulletIndex); // `currentBulletNum`�� ���� ���� �Ѿ��� ��ȣ
    }
}
