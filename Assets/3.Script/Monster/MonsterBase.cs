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
        M_CardPack
    }
    // Monster Base Info
    public MonsterType monsterType;
    public float MoveSpeed;
    public float AttackPower;
    public float HP;
    public float SearchingCoolTime; // Ž�� ��Ÿ��
    public float AttackDelayTime; // Attack Ready Animation Time
    public float AttackCoolTime;
    public float DetectingAreaR;
    private bool isMoving = true;

    // Target Info
    private Transform player; // �÷��̾��� ��ġ
    private Vector3 TargetPosition; // Ž���� �÷��̾��� ��ġ ����
    private bool DetectionSuccess = false; // Ž�� ���� ����

    private GameManager GameManager;
    private SpriteRenderer SpriteRenderer;
    Animator Animator;

    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameManager.Instance;
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    protected IEnumerator MonsterRoutine()
    {
        Debug.Log("\"MonsterRoutine \"�Լ��� ���������� ����.");
        yield return null;
    }

    protected virtual IEnumerator AttackPreparation()
    {
        Debug.Log("\"AttackPreparation \"�Լ��� ���������� ����."); 
        yield return null;
    }

    protected virtual IEnumerator RandomMoveAfterSearchFail()
    {
        Debug.Log("\"RandomMoveAfterSearchFail \"�Լ��� ���������� ����.");
        yield return null;
    }

    // Return RandomPosition
    protected Vector3 GetRanomPositionAround()
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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(AttackPower);
            }
            else
            {
                Debug.Log("Monster OnTriggerEnter2D : Player Not Found");
            }
        }
    }

    // Boundery Check
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            isMoving = false;
            Animator.SetBool("Move", false);
        }
    }
}
