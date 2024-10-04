using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class M_V1 : MonsterBase
{
 
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        monsterType = MonsterType.M_V1;
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
                MAnimator.SetBool("Move", false);
            }

            yield return new WaitForSeconds(SearchingCoolTime);
        }
    }

    public override IEnumerator AttackPreparation()
    {
        // ���� ����
        SpriteFlipSetting();

        // ���� �غ� ��� �ð�
        yield return new WaitForSeconds(AttackDelayTime);

        var speed = MoveSpeed * Time.deltaTime;
        float maxMoveDuration = 1.0f;
        float elapsedTime = 0f;

        while (Vector3.Distance(transform.position, TargetPosition) > speed)
        {
            elapsedTime += Time.deltaTime;

            // ������ �ð��� �ʰ��ϸ� �̵� ����
            if (elapsedTime >= maxMoveDuration)
            {
                MAnimator.SetBool("Move", false);

                yield break; // �ڷ�ƾ ����
            }

            MAnimator.SetBool("Move", true);

            // Rigidbody2D�� ����Ͽ� �̵�
            rb.MovePosition(Vector3.MoveTowards(rb.position, TargetPosition, MoveSpeed * Time.fixedDeltaTime));

            yield return new WaitForFixedUpdate(); // ���� ������Ʈ �����ӿ� ���缭 ���
        }

        MAnimator.SetBool("Move", false);
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
                MAnimator.SetBool("Move", false);
                yield break; // �ڷ�ƾ ����
            }

            MAnimator.SetBool("Move", true);

            // Rigidbody2D�� ����Ͽ� �̵�
            rb.MovePosition(Vector3.MoveTowards(rb.position, TargetPosition, MoveSpeed * Time.fixedDeltaTime));

            yield return new WaitForFixedUpdate(); // ���� ������Ʈ �����ӿ� ���缭 ���
        }

        MAnimator.SetBool("Move", false);
    }
}
