using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class M_VE_1 : MonsterBase
{
    protected override void Start()
    {
        base.Start();
        monsterType = MonsterType.M_VE_1;
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
            }

            yield return new WaitForSeconds(SearchingCoolTime);
        }
    }

    public override IEnumerator AttackPreparation()
    {
        // 방향 설정
        SpriteFlipSetting();

        if (!MAnimator.GetBool("Detected"))
        {
            MAnimator.SetBool("Detected", true);
            yield return new WaitForSeconds(1.30f);

            DetectingAreaR = 30;

            SearchingCoolTime = 0;
            AttackDelayTime = 0;
            AttackCoolTime = 0;
        }

        // 실시간 추적
        // Rigidbody2D를 사용하여 이동
        rb.MovePosition(Vector3.MoveTowards(rb.position, TargetPosition, MoveSpeed * Time.fixedDeltaTime));
        yield return null;
    }
}
