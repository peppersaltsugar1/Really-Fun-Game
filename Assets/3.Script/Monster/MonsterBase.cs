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
        { MonsterType.M_V1, "M_V1�̸��̸�" },
        { MonsterType.M_V2, "M_V2�̸��̸�" },
        { MonsterType.M_V3, "M_V3�̸��̸�" },
        { MonsterType.M_CardPack, "M_CardPack�̸��̸�" },
        { MonsterType.M_VE_1, "M_VE_1�̸��̸�" },
        { MonsterType.M_VE_2, "M_VE_2�̸��̸�" },
        { MonsterType.M_SpiderCardPack, "M_SpiderCardPack�̸��̸�" },
        { MonsterType.Red_Spider, "Red_Spider�̸��̸�" },
        { MonsterType.White_Spider, "White_Spider�̸��̸�" },
        { MonsterType.Boss_Mouse, "Boss_Mouse�� ����" }
    };

    // Monster Base Info
    public MonsterType monsterType;
    public float MoveSpeed;
    public float AttackPower;
    public float HP;
    protected float BaseHP;
    public float DefenseRate; // ���� ���
    public float DetectingAreaR;
    protected bool isMoving = true;

    // Target Info
    protected Transform player; // �÷��̾��� ��ġ
    protected Vector3 TargetPosition; // Ž���� �÷��̾��� ��ġ ����
    protected bool DetectionSuccess = false; // Ž�� ���� ����

    protected GameManager GameManager;
    protected StatusManager statusManager;
    protected SpriteRenderer SpriteRenderer;
    protected Animator MAnimator;
    protected Rigidbody2D rb;

    FolderManager folderManager;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        GameManager = GameManager.Instance;
        statusManager = StatusManager.Instance;
        SpriteRenderer = GetComponent<SpriteRenderer>();
        MAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        folderManager = FolderManager.Instance;
        DefenseRate = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual IEnumerator MonsterRoutine()
    {
        Debug.Log("\"MonsterRoutine \"�Լ��� ���������� ����.");
        yield return null;
    }

    public virtual IEnumerator AttackPreparation()
    {
        Debug.Log("\"AttackPreparation \"�Լ��� ���������� ����.");
        yield return null;
    }

    public virtual IEnumerator RandomMoveAfterSearchFail()
    {
        Debug.Log("\"RandomMoveAfterSearchFail \"�Լ��� ���������� ����.");
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
        // ���� ����
        if (TargetPosition.x > transform.position.x)
        {
            SpriteRenderer.flipX = true;   // ������ �ٶ� (Flip X Ȱ��ȭ)
        }
        else if (TargetPosition.x < transform.position.x)
        {
            SpriteRenderer.flipX = false;  // �������� �ٶ� (Flip X ��Ȱ��ȭ)
        }
    }

    /*
     * Collision Handling Section
     */
    // Player Collision
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger)
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
                Destroy(collision.gameObject);
                // Debug.Log("Attack Damage : " + statusManager.AttackPower * DefenseRate);
                this.HP -= statusManager.AttackPower * DefenseRate;
                
                if (HP <= 0)
                {
                    Die();
                }
            }
        }
    }

    protected virtual void Die()
    {
        Destroy(this.gameObject);
        folderManager.UpdateMonsterCount(-1);
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

    // ========== Ž������ ǥ�ÿ� ==========
    // Ž�� ������ �ð������� ǥ�� (������ ����)
    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, DetectingAreaR);
    }
}
