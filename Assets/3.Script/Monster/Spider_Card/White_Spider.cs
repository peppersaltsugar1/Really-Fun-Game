using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class White_Spider : MonsterBase
{
    bool First = true;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        TargetPosition = this.transform.position + new Vector3(0, -2, 0);
        monsterType = MonsterType.White_Spider;
        StartCoroutine(MonsterRoutine());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator MonsterRespawnMove()
    {
        var speed = MoveSpeed * Time.deltaTime;

        while (Vector3.Distance(transform.position, TargetPosition) > speed)
        {
            rb.MovePosition(Vector3.MoveTowards(rb.position, TargetPosition, MoveSpeed * Time.fixedDeltaTime));

            yield return new WaitForFixedUpdate(); // 물리 업데이트 프레임에 맞춰서 대기
        }
    }

    public override IEnumerator MonsterRoutine()
    {
        if (First)
        {
            StartCoroutine(MonsterRespawnMove());
            First = false;
            rb.velocity = Vector2.zero;
            yield return new WaitForSeconds(3.5f);
        }
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
                yield return RandomMoveAfterSearchFail();
            }

            yield return new WaitForSeconds(SearchingCoolTime);
        }
    }

    public override IEnumerator AttackPreparation()
    {
        // 방향 설정
        SpriteFlipSetting();

        MAnimator.SetTrigger("Jump");
        while (Vector3.Distance(transform.position, TargetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, TargetPosition, MoveSpeed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(AttackCoolTime);
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
                yield break; // 코루틴 종료
            }

            MAnimator.SetTrigger("Jump");

            // Rigidbody2D를 사용하여 이동
            rb.MovePosition(Vector3.MoveTowards(rb.position, TargetPosition, MoveSpeed * Time.fixedDeltaTime));

            yield return new WaitForFixedUpdate(); // 물리 업데이트 프레임에 맞춰서 대기
        }
    }
}
