using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MonsterBase : MonoBehaviour
{
    public enum MonsterType
    {
        M_V1,
        M_V2,
        M_V3,
        M_CardPack,
        M_VE_1,
        M_VE_2,
        M_SpiderCardPack,
        Red_Spider,
        White_Spider
    }
    // Monster Base Info
    public MonsterType monsterType;
    public float MoveSpeed;
    public float AttackPower;
    public float HP;
    public float SearchingCoolTime; // 탐색 쿨타임
    public float AttackDelayTime; // Attack Ready Animation Time
    public float AttackCoolTime;
    public float DetectingAreaR;
    protected bool isMoving = true;

    // Target Info
    protected Transform player; // 플레이어의 위치
    protected Vector3 TargetPosition; // 탐지된 플레이어의 위치 저장
    protected bool DetectionSuccess = false; // 탐지 성공 여부

    protected GameManager GameManager;
    protected StatusManager statusManager;
    protected SpriteRenderer SpriteRenderer;
    protected Animator MAnimator;
    protected Rigidbody2D rb;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        GameManager = GameManager.Instance;
        statusManager = StatusManager.Instance;
        SpriteRenderer = GetComponent<SpriteRenderer>();
        MAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual IEnumerator MonsterRoutine()
    {
        Debug.Log("\"MonsterRoutine \"함수를 재정의하지 않음.");
        yield return null;
    }

    public virtual IEnumerator AttackPreparation()
    {
        Debug.Log("\"AttackPreparation \"함수를 재정의하지 않음.");
        yield return null;
    }

    public virtual IEnumerator RandomMoveAfterSearchFail()
    {
        Debug.Log("\"RandomMoveAfterSearchFail \"함수를 재정의하지 않음.");
        yield return null;
    }

    // Return RandomPosition
    protected virtual Vector3 GetRanomPositionAround()
    {
        float randomAngle = Random.Range(0f, 360f);
        float randomDistance = Random.Range(0f, DetectingAreaR);

        float x = transform.position.x + randomDistance * Mathf.Cos(randomAngle * Mathf.Deg2Rad);
        float y = transform.position.y + randomDistance * Mathf.Sin(randomAngle * Mathf.Deg2Rad);

        return new Vector3(x, y, 0);
    }

    // Sprite Flip Setting
    protected void SpriteFlipSetting()
    {
        // 방향 설정
        if (TargetPosition.x > transform.position.x)
        {
            SpriteRenderer.flipX = true;   // 왼쪽을 바라봄 (Flip X 활성화)
        }
        else if (TargetPosition.x < transform.position.x)
        {
            SpriteRenderer.flipX = false;  // 오른쪽을 바라봄 (Flip X 비활성화)
        }
    }

    /*
     * Collision Handling Section
     */
    // Player Collision
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (statusManager != null)
            {
                statusManager.TakeDamage(AttackPower);
            }
            else
            {
                Debug.Log("Monster OnTriggerEnter2D : Player Not Found");
            }
            if (monsterType != MonsterType.M_SpiderCardPack)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = 0f;
            }
        }

        if (collision.gameObject.CompareTag("Bullet"))
        {
            Debug.Log("Monster Take Damage");
            this.HP -= statusManager.AttackPower;
            Destroy(collision.gameObject);

            if (HP < 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        Destroy(this.gameObject);
    }

    // ========== 탐색범위 표시용 ==========
    // 탐지 범위를 시각적으로 표시 (에디터 전용)
    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, DetectingAreaR);
    }
}
