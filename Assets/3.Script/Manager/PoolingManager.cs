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
                // 싱글톤 인스턴스가 없으면 새로운 인스턴스를 찾거나 생성
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
            // 총알 프리팹을 선택합니다. (여기서는 랜덤으로 선택하지만 필요에 따라 수정 가능)
            GameObject bulletPrefab = bulletList[bulletNum];
            GameObject bulletObject = Instantiate(bulletPrefab);
            bulletObject.SetActive(false); // 비활성화하여 풀에 추가

            // Bullet 컴포넌트를 가져와서 큐에 추가합니다.
            Bullet bullet = bulletObject.GetComponent<Bullet>();

            if (bullet != null)
            {
                bulletPool.Enqueue(bullet);
            }
            bulletObject.transform.SetParent(bulletBox.transform, false);
            // 생성된 Map 오브젝트를 map의 자식 목록에서 마지막으로 위치시키기
            bulletObject.transform.SetAsLastSibling();

        }
    }
    public void ReMakeBullet(int bulletIndex) 
    {
        // 기존 총알 제거
        while (bulletPool.Count > 0)
        {
            Bullet bullet = bulletPool.Dequeue(); // 큐에서 총알을 꺼냄
            if (bullet != null && bullet.gameObject != null)
            {
                Destroy(bullet.gameObject); // 게임 오브젝트 삭제
            }
        }

        // 큐 비워짐, 총알 새로 생성
        BulletMake(bulletIndex); // `currentBulletNum`은 새로 만들 총알의 번호
    }
}
