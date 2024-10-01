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
    public float SearchingCoolTime; // 탐색 쿨타임
    public float AttackDelayTime; // Attack Ready Animation Time
    public float AttackCoolTime;
    public float DetectingAreaR;
    private bool isMoving = true;

    // Target Info
    private Transform player; // 플레이어의 위치
    private Vector3 TargetPosition; // 탐지된 플레이어의 위치 저장
    private bool DetectionSuccess = false; // 탐지 성공 여부

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
        Debug.Log("\"MonsterRoutine \"함수를 재정의하지 않음.");
        yield return null;
    }

    protected virtual IEnumerator AttackPreparation()
    {
        Debug.Log("\"AttackPreparation \"함수를 재정의하지 않음."); 
        yield return null;
    }

    protected virtual IEnumerator RandomMoveAfterSearchFail()
    {
        Debug.Log("\"RandomMoveAfterSearchFail \"함수를 재정의하지 않음.");
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
