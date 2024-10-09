using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class M_CardPack : MonsterBase
{
    public Transform FirePoint;      // �Ѿ� �߻� ��ġ
    public float BulletSpeed = 10f;  // �Ѿ� �ӵ�

    // ������ ����Ʈ 
    public GameObject CardPrefab1;
    public GameObject CardPrefab2;
    public GameObject CardPrefab3;
    public GameObject CardPrefab4;

    protected override void Start()
    {
        base.Start();
        monsterType = MonsterType.M_CardPack;
        StartCoroutine(MonsterRoutine());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override IEnumerator MonsterRoutine()
    {
        while (true)
        {
            Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(transform.position, DetectingAreaR);
            rb.bodyType = RigidbodyType2D.Dynamic;

            foreach (Collider2D obj in detectedObjects)
            {
                if (obj.CompareTag("Player"))
                {
                    player = obj.transform;

                    TargetPosition = player.position;
                    DetectionSuccess = true;
                    break;
                }
            }

            if (DetectionSuccess)
            {
                yield return AttackPreparation();
                DetectionSuccess = false;
            }
            else
            {
                isMoving = true;
                Debug.Log("�÷��̾� Ž������");
            }

            yield return new WaitForSeconds(SearchingCoolTime);
        }
    }

    public override IEnumerator AttackPreparation()
    {
        // ���� ����
        // SpriteFlipSetting();
        // �÷��̾� �ٶ󺸱�
        Vector3 direction = TargetPosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);

        // ���� ó��
        yield return Fire();

        yield return new WaitForSeconds(AttackCoolTime);
    }

    IEnumerator Fire()
    {
        int CardNum = Random.Range(0, 4);

        /*
         * 2�ʵ��� ���� �ִϸ��̼� ����. -> �߻� -> �ٷ� ��� �ִ�.
            4���߿� �ϳ� Ŭ�ι� ��Ʈ �����̵� ���̾�
            �����̵� ���̾ƴ� ������ �߻� �ѹ�.
         */

        Vector3 direction = TargetPosition - FirePoint.position;
        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        switch (CardNum)
        {
            case 0:
                MAnimator.SetTrigger("Hearth");
                break;
            case 1:
                MAnimator.SetTrigger("Clover");
                break;
            case 2:
                MAnimator.SetTrigger("Spade");
                break;
            case 3:
                MAnimator.SetTrigger("Dia");
                break;
            default:
                Debug.Log("CardNum Error");
                break;

        }
        // ���� �ִϸ��̼� ����ð�
        yield return new WaitForSeconds(2f);

        if (CardNum == 0 || CardNum == 1)
        {
            float[] angles = { baseAngle - 30, baseAngle, baseAngle + 30 };

            foreach (float angle in angles)
            {

                GameObject bullet = Instantiate((CardNum == 0 ? CardPrefab1 : CardPrefab2), FirePoint.position, Quaternion.Euler(0, 0, angle));
                Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());

                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                bullet.transform.Rotate(0, 0, 90);
                if (rb != null)
                {
                    Vector2 fireDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
                    rb.velocity = fireDirection * (BulletSpeed*2/5);
                }
            }
        }
        else
        {
            GameObject bullet = Instantiate((CardNum == 2 ? CardPrefab3 : CardPrefab4), FirePoint.position, Quaternion.Euler(0, 0, baseAngle));
            Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            bullet.transform.Rotate(0, 0, 90);
            if (rb != null)
            {
                Vector2 fireDirection = (TargetPosition - FirePoint.position).normalized;
                rb.velocity = fireDirection * BulletSpeed;
            }
        }
    }
}
