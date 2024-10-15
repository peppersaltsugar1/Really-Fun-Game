using System.Collections;
using System.Collections.Generic;
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
        White_Spider,
        Boss_Mouse
    }

    public static Dictionary<MonsterType, string> MonsterNameDict = new Dictionary<MonsterType, string>
    {
        { MonsterType.M_V1, "M_V1이름이름" },
        { MonsterType.M_V2, "M_V2이름이름" },
        { MonsterType.M_V3, "M_V3이름이름" },
        { MonsterType.M_CardPack, "M_CardPack이름이름" },
        { MonsterType.M_VE_1, "M_VE_1이름이름" },
        { MonsterType.M_VE_2, "M_VE_2이름이름" },
        { MonsterType.M_SpiderCardPack, "M_SpiderCardPack이름이름" },
        { MonsterType.Red_Spider, "Red_Spider이름이름" },
        { MonsterType.White_Spider, "White_Spider이름이름" },
        { MonsterType.Boss_Mouse, "Boss_Mouse는 보스" }
    };

    // Monster Base Info
    public MonsterType monsterType;
    public float MoveSpeed;
    public float AttackPower;
    public float HP;
    protected float BaseHP;
    public float DefenseRate; // 방어력 계수
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

        DefenseRate = 1.0f;
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
                statusManager.TakeDamage(AttackPower, monsterType);
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
            // Debug.Log("Monster Take Damage");
            this.HP -= statusManager.AttackPower * DefenseRate;
            Destroy(collision.gameObject);

            if (HP <= 0)
            {
                Die();
            }
        }
    }

    protected virtual void Die()
    {
        Destroy(this.gameObject);
    }

    public bool DetectionPlayerPosition()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(transform.position, DetectingAreaR);
        rb.bodyType = RigidbodyType2D.Dynamic;

        foreach (Collider2D obj in detectedObjects)
        {
            if (obj.CompareTag("Player"))
            {
                player = obj.transform;

                TargetPosition = player.position;
                return true;
            }
        }

        return false;
    }

    // ========== 탐색범위 표시용 ==========
    // 탐지 범위를 시각적으로 표시 (에디터 전용)
    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, DetectingAreaR);
    }
}
