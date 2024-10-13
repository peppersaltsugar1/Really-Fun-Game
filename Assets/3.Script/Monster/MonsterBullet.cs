using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBullet : MonoBehaviour
{
    public float lifeTime = 5f;  // 총알이 파괴되기까지의 시간
    public int damage = 10;      // 총알의 공격력
    public MonsterBase.MonsterType monstertype;
    // Start is called before the first frame update
    void Start()
    {
        // 일정 시간이 지나면 총알 파괴
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 충돌 처리
    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.gameObject.CompareTag("Player"))
        {
            StatusManager statusManager = StatusManager.Instance;
            if (statusManager != null)
            {
                statusManager.TakeDamage(damage, monstertype);
            }
            Destroy(gameObject);
        }
    }
}
