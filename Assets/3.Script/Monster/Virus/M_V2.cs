using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class M_V2 : MonsterBase
{
    public GameObject BulletPrefab;  // �߻��� �Ѿ� ������
    public Transform FirePoint;      // �Ѿ� �߻� ��ġ
    public float BulletSpeed = 10f;  // �Ѿ� �ӵ�

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        monsterType = MonsterType.M_V2;
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
                    // Debug.Log("�÷��̾� Ž�� ����");

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

                yield return RandomMoveAfterSearchFail();
            }

            yield return new WaitForSeconds(SearchingCoolTime);
        }
    }

    public override IEnumerator AttackPreparation()
    {
        // ���� ����
        SpriteFlipSetting();

        // Waitting during Attack Ready Animation
        MAnimator.SetTrigger("Shot");
        yield return new WaitForSeconds(AttackDelayTime);

        // Attack
        Fire();

        yield return new WaitForSeconds(AttackCoolTime);
    }

    public override IEnumerator RandomMoveAfterSearchFail()
    {
        TargetPosition = GetRanomPositionAround();

        // ���� ����
        SpriteFlipSetting();

        var speed = MoveSpeed * Time.deltaTime;
        float maxMoveDuration = 1.0f;  // �̵��� ����� �ִ� �ð� (��: 3��)
        float elapsedTime = 0f;        // ��� �ð�

        while (Vector3.Distance(transform.position, TargetPosition) > speed && isMoving)
        {
            elapsedTime += Time.deltaTime;

            // ������ �ð��� �ʰ��ϸ� �̵� ����
            if (elapsedTime >= maxMoveDuration)
            {
                yield break; // �ڷ�ƾ ����
            }

            // Rigidbody2D�� ����Ͽ� �̵�
            rb.MovePosition(Vector3.MoveTowards(rb.position, TargetPosition, MoveSpeed * Time.fixedDeltaTime));

            yield return new WaitForFixedUpdate(); // ���� ������Ʈ �����ӿ� ���缭 ���
        }
    }

    // ���� ����
    private void Fire()
    {
        GameObject bullet = Instantiate(BulletPrefab, FirePoint.position, FirePoint.rotation);
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        Vector2 fireDirection = (TargetPosition - FirePoint.position).normalized;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = fireDirection * BulletSpeed;
    }
}
