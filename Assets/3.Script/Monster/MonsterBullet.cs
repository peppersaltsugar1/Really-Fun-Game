using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBullet : MonoBehaviour
{
    public float lifeTime = 5f;  // �Ѿ��� �ı��Ǳ������ �ð�
    public int damage = 10;      // �Ѿ��� ���ݷ�
    public MonsterBase.MonsterType monstertype;

    //�ѹ��� �浹�� �ι� ȿ�� �߻��ϴ°� ������
    private bool hasHit = false;



    // Start is called before the first frame update
    void Start()
    {
        // ���� �ð��� ������ �Ѿ� �ı�
        Destroy(gameObject, lifeTime);
    }



    // �浹 ó��
    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        Player player = hitInfo.GetComponent<Player>();

        if (hitInfo.gameObject.CompareTag("Player"))
        {
            if (hasHit) return; // �̹� �浹������ �� �̻� ó������ ����
            hasHit = true;
            StatusManager statusManager = StatusManager.Instance;
            if (statusManager != null)
            {
                statusManager.TakeDamage(damage, monstertype);
            }

            Destroy(gameObject);
        }
    }
}
