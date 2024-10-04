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
                    // Debug.Log("플레이어 탐색 성공");

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
        // 방향 설정
        SpriteFlipSetting();

        // 공격 준비 대기 시간
        yield return new WaitForSeconds(AttackDelayTime);

        var speed = MoveSpeed * Time.deltaTime;
        float maxMoveDuration = 1.0f;
        float elapsedTime = 0f;

        while (Vector3.Distance(transform.position, TargetPosition) > speed)
        {
            elapsedTime += Time.deltaTime;

            // 지정된 시간을 초과하면 이동 중지
            if (elapsedTime >= maxMoveDuration)
            {
                MAnimator.SetBool("Move", false);

                yield break; // 코루틴 종료
            }

            MAnimator.SetBool("Move", true);

            // Rigidbody2D를 사용하여 이동
            rb.MovePosition(Vector3.MoveTowards(rb.position, TargetPosition, MoveSpeed * Time.fixedDeltaTime));

            yield return new WaitForFixedUpdate(); // 물리 업데이트 프레임에 맞춰서 대기
        }

        MAnimator.SetBool("Move", false);
    }

    public override IEnumerator RandomMoveAfterSearchFail()
    {
        TargetPosition = GetRanomPositionAround();

        // 방향 설정
        SpriteFlipSetting();

        var speed = MoveSpeed * Time.deltaTime;
        float maxMoveDuration = 1.0f;  // 이동을 허용할 최대 시간 (예: 3초)
        float elapsedTime = 0f;        // 경과 시간

        while (Vector3.Distance(transform.position, TargetPosition) > speed && isMoving)
        {
            elapsedTime += Time.deltaTime;

            // 지정된 시간을 초과하면 이동 중지
            if (elapsedTime >= maxMoveDuration)
            {
                MAnimator.SetBool("Move", false);
                yield break; // 코루틴 종료
            }

            MAnimator.SetBool("Move", true);

            // Rigidbody2D를 사용하여 이동
            rb.MovePosition(Vector3.MoveTowards(rb.position, TargetPosition, MoveSpeed * Time.fixedDeltaTime));

            yield return new WaitForFixedUpdate(); // 물리 업데이트 프레임에 맞춰서 대기
        }

        MAnimator.SetBool("Move", false);
    }
}
