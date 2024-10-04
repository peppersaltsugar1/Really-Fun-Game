using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_VE_2 : MonsterBase
{
    public GameObject BulletPrefab;  // �߻��� �Ѿ� ������
    public Transform FirePoint;      // �Ѿ� �߻� ��ġ
    public float BulletSpeed = 10f;  // �Ѿ� �ӵ�

    protected override void Start()
    {
        base.Start();
        monsterType = MonsterType.M_VE_2;
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
        SpriteFlipSetting();

        if (!MAnimator.GetBool("Detected"))
        {
            MAnimator.SetBool("Detected", true);
            yield return new WaitForSeconds(1.30f);

            DetectingAreaR = 10;

            SearchingCoolTime = 1;
            AttackCoolTime = 1;
        }


        MAnimator.SetTrigger("Shot");
        yield return new WaitForSeconds(1.05f);
        Fire();

        yield return new WaitForSeconds(AttackCoolTime);
    }

    private void Fire()
    {
        GameObject bullet = Instantiate(BulletPrefab, FirePoint.position, FirePoint.rotation);
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        Vector2 fireDirection = (TargetPosition - FirePoint.position).normalized;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = fireDirection * BulletSpeed;
    }
}
