using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBullet : MonoBehaviour
{
    public float lifeTime = 5f;  // �Ѿ��� �ı��Ǳ������ �ð�
    public int damage = 10;      // �Ѿ��� ���ݷ�

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
        Player player = hitInfo.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
