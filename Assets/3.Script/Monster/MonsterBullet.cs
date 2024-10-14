using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBullet : MonoBehaviour
{
    public float lifeTime = 5f;  // �Ѿ��� �ı��Ǳ������ �ð�
    public int damage = 10;      // �Ѿ��� ���ݷ�
    public MonsterBase.MonsterType monstertype;
    // Start is called before the first frame update
    void Start()
    {
        // ���� �ð��� ������ �Ѿ� �ı�
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // �浹 ó��
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
