using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_V3 : MonsterBase
{
    public float SearchingCoolTime;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        monsterType = MonsterType.M_V3;
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
        MAnimator.SetTrigger("Jump");
        yield return new WaitForSeconds(1.7f);

        var speed = MoveSpeed * Time.deltaTime;
        float maxMoveDuration = 1.0f;
        float elapsedTime = 0f;

        while (Vector3.Distance(transform.position, TargetPosition) > speed)
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

    public override IEnumerator RandomMoveAfterSearchFail()
    {
        TargetPosition = GetRanomPositionAround();

        // ���� ����
        SpriteFlipSetting();

        MAnimator.SetTrigger("Jump");
        yield return new WaitForSeconds(1.7f);

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

    protected override Vector3 GetRanomPositionAround()
    {
        // ������ ���� ���� (0�� ~ 360��)
        float randomAngle = Random.Range(0f, 360f);

        // �Ÿ��� �׻� DetectingAreaR (������)���� ����
        float x = transform.position.x + DetectingAreaR * Mathf.Cos(randomAngle * Mathf.Deg2Rad);
        float y = transform.position.y + DetectingAreaR * Mathf.Sin(randomAngle * Mathf.Deg2Rad);

        return new Vector3(x, y, 0);
    }
}
