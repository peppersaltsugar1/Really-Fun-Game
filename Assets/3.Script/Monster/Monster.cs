using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public enum MonsterType
    {
        M_V1,
        M_V2,
        M_V3,
        M_CardPack
    }
    // ���� �⺻����
    public MonsterType monsterType; // ���� Ÿ��
    public float MoveSpeed;
    public float AttackPower;
    public float HP;
    public float WaittingTime; // Ž�� ��Ÿ��
    public float AttackDelayTime; // ���� �غ� �ִϸ��̼� �ð�
    public float AttackCoolTime; // ���� ��Ÿ��
    public float DetectingAreaR; // Ž�� ���� ������
    private bool isMoving = true; // �̵� ���θ� ����

    private Transform player; // �÷��̾��� ��ġ
    private Vector3 targetPosition; // Ž���� �÷��̾��� ��ġ ����
    private bool isAttacking = false; // ���� �غ� �������� Ȯ��
    private bool DetectionSuccess = false; // Ž�� ���� ����

    // 3�� ���� �����̵� ������ ����
    private Vector3 controlPoint;        // ��� �׸��� ���� ������
    public float travelTime = 0.45f;    // �̵��ϴ� �� �ɸ��� �� �ð� (0.45��)

    // ���� ���� ������
    public GameObject bulletPrefab;  // �߻��� �Ѿ� ������
    public Transform firePoint;      // �Ѿ� �߻� ��ġ
    public float bulletSpeed = 10f;  // �Ѿ� �ӵ�

    public GameObject CardPrefab1;  // �߻��� �Ѿ� ������
    public GameObject CardPrefab2;  // �߻��� �Ѿ� ������
    public GameObject CardPrefab3;  // �߻��� �Ѿ� ������
    public GameObject CardPrefab4;  // �߻��� �Ѿ� ������

    // ��Ÿ ����
    Animator animator; //�ִϸ�����
    private GameManager gameManager;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        gameManager = GameManager.Instance;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        Debug.Log("���� Ȯ��, ��ƾ ����");
        StartCoroutine(MonsterRoutine());

    }

    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                Debug.Log("Monster OnTriggerEnter2D : Take Damage");
                player.TakeDamage(AttackPower);
            }
            else
            {
                Debug.Log("Monster OnTriggerEnter2D : Player Not Found");
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("There Is Wall");
            isMoving = false;  // �浹 �� �̵� ����
            animator.SetBool("Move", false);
        }
    }

    private IEnumerator MonsterRoutine()
    {
        while (true)
        {
            Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(transform.position, DetectingAreaR);

            foreach (Collider2D obj in detectedObjects)
            {
                if (obj.CompareTag("Player"))
                {
                    player = obj.transform;
                    // Debug.Log("�÷��̾� Ž�� ����");

                    targetPosition = player.position;
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
                // Debug.Log("�÷��̾� Ž������");

                switch (monsterType)
                {
                    case MonsterType.M_V1:
                        yield return new WaitForSeconds(WaittingTime);
                        yield return RandomMoveAfterSearchFail();
                        animator.SetBool("Move", false);
                        break;
                    case MonsterType.M_V2:
                        yield return new WaitForSeconds(WaittingTime);
                        yield return RandomMoveAfterSearchFail();
                        Debug.Log("Move!!");
                        break;
                    case MonsterType.M_V3:
                        yield return new WaitForSeconds(WaittingTime);
                        yield return RandomMoveAfterSearchFail();
                        Debug.Log("Move!!");
                        break;
                    case MonsterType.M_CardPack:
                        yield return new WaitForSeconds(WaittingTime);
                        Debug.Log(" Not Move!!");
                        break;
                }

                if (monsterType == MonsterType.M_V1)
                {
                }
                else if (monsterType == MonsterType.M_V2)
                {
                }
                else if (monsterType == MonsterType.M_V3)
                {
                }
                else if (monsterType == MonsterType.M_CardPack)
                {
                }
            }
        }
    }

    // ���� �غ� �ִϸ��̼� ó��
    private IEnumerator AttackPreparation()
    {
        // ���� ����
        if (targetPosition.x > transform.position.x)
        {
            spriteRenderer.flipX = true;   // ������ �ٶ� (Flip X Ȱ��ȭ)
        }
        else if (targetPosition.x < transform.position.x)
        {
            spriteRenderer.flipX = false;  // �������� �ٶ� (Flip X ��Ȱ��ȭ)
        }


        // ���� �غ� ���·� ����
        isAttacking = true;

        // Debug.Log("���� �غ���");

        if (monsterType == MonsterType.M_V1)
        {
            yield return new WaitForSeconds(AttackDelayTime);

            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                animator.SetBool("Move", true);
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);
                yield return null;
            }
            animator.SetBool("Move", false);
        }
        else if (monsterType == MonsterType.M_V2)
        {
            animator.SetTrigger("Shot");
            yield return new WaitForSeconds(AttackDelayTime);

            Fire();
        }
        else if (monsterType == MonsterType.M_V3)
        {
            // ������ �̵���(��Ȱ��ȭ)
            // StartCoroutine(MoveAlongCurve());

            // �����̵�
            animator.SetTrigger("Jump");
            yield return new WaitForSeconds(1.7f);
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);
                yield return null;
            }
        }
        else if (monsterType == MonsterType.M_CardPack)
        {
            // �÷��̾� �ٶ󺸱�
            Vector3 direction = targetPosition - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);

            // ���� ó��
            yield return CardFire();
        }

        // Debug.Log("Attack End, CoolTime Start");
        yield return new WaitForSeconds(AttackCoolTime);
        isAttacking = false;
    }

    IEnumerator RandomMoveAfterSearchFail()
    {
        targetPosition = GetRanomPositionAround();

        // ���� ����
        if (targetPosition.x > transform.position.x)
        {
            spriteRenderer.flipX = true;   // ������ �ٶ� (Flip X Ȱ��ȭ)
        }
        else if (targetPosition.x < transform.position.x)
        {
            spriteRenderer.flipX = false;  // �������� �ٶ� (Flip X ��Ȱ��ȭ)
        }

        // �̵� ����
        if (monsterType == MonsterType.M_V1)
        {
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f && isMoving)
            {
                animator.SetBool("Move", true);
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);
                yield return null;  // �� ������ ���
            }
        }
        else if (monsterType == MonsterType.M_V2)
        {
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f && isMoving)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);
                yield return null;
            }
        }
        else if (monsterType == MonsterType.M_V3)
        {
            // ������ �̵���(��Ȱ��ȭ)
            // StartCoroutine(MoveAlongCurve());

            // �����̵�
            animator.SetTrigger("Jump");
            yield return new WaitForSeconds(1.7f);
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);
                yield return null;
            }
        }
        else if (monsterType == MonsterType.M_CardPack)
        {
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f && isMoving)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);
                yield return null;
            }
        }
        // Debug.Log("���� ��ġ�� �̵� �Ϸ�");
    }

    Vector3 GetRanomPositionAround()
    {
        float randomAngle = Random.Range(0f, 360f);
        float randomDistance = Random.Range(0f, DetectingAreaR);

        float x = transform.position.x + randomDistance * Mathf.Cos(randomAngle * Mathf.Deg2Rad);
        float y = transform.position.y + randomDistance * Mathf.Sin(randomAngle * Mathf.Deg2Rad);

        return new Vector3(x, y, 0);
    }

    IEnumerator MoveAlongCurve()
    {
        animator.SetTrigger("Jump");
        yield return new WaitForSeconds(1.7f);
        float t = 0f;
        Vector3 startPosition = transform.position;

        while (t < 1f)
        {
            t += Time.deltaTime / travelTime;

            Vector3 newPosition = (1 - t) * (1 - t) * startPosition +
                                  2 * (1 - t) * t * controlPoint +
                                  t * t * targetPosition;

            transform.position = newPosition;

            yield return null;
        }

        transform.position = targetPosition;
    }

    // ���� ����
    void Fire()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        Vector2 fireDirection = (targetPosition - firePoint.position).normalized;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = fireDirection * bulletSpeed;
    }

    IEnumerator CardFire()
    {
        int CardNum = Random.Range(0, 4);

        /*
         * 2�ʵ��� ���� �ִϸ��̼� ����. -> �߻� -> �ٷ� ��� �ִ�.
            4���߿� �ϳ� Ŭ�ι� ��Ʈ �����̵� ���̾�
            �����̵� ���̾ƴ� ������ �߻� �ѹ�.
         */

        Vector3 direction = targetPosition - firePoint.position;
        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Debug.Log("CardNum Animation");
        switch (CardNum)
        {
            case 0:
                animator.SetTrigger("Hearth");
                break;
            case 1:
                animator.SetTrigger("Clover");
                break;
            case 2:
                animator.SetTrigger("Spade");
                break;
            case 3:
                animator.SetTrigger("Dia");
                break;
            default:
                Debug.Log("CardNum Error");
                break;

        }
        

        yield return new WaitForSeconds(1.5f);
        if (CardNum == 0 || CardNum == 1)
        {
            float[] angles = { baseAngle - 30, baseAngle, baseAngle + 30 };

            foreach (float angle in angles)
            {

                GameObject bullet = Instantiate((CardNum == 0 ? CardPrefab1 : CardPrefab2), firePoint.position, Quaternion.Euler(0, 0, angle));
                Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());

                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                bullet.transform.Rotate(0, 0, 90);
                if (rb != null)
                {
                    Vector2 fireDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
                    rb.velocity = fireDirection * bulletSpeed;
                }
            }
        }
        else
        {
            GameObject bullet = Instantiate((CardNum == 2 ? CardPrefab3 : CardPrefab4), firePoint.position, Quaternion.Euler(0, 0, baseAngle));
            Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            bullet.transform.Rotate(0, 0, 90);
            if (rb != null)
            {
                Vector2 fireDirection = (targetPosition - firePoint.position).normalized;
                rb.velocity = fireDirection * bulletSpeed;
            }
        }
    }


    // ========== Ž������ ǥ�ÿ�(��������) ==========
    // Ž�� ������ �ð������� ǥ�� (������ ����)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, DetectingAreaR);
    }

}
