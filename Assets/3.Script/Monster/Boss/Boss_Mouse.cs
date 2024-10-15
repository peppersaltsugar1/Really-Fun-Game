using System.Collections;
using UnityEngine;

public class Boss_Mouse : MonsterBase
{
    // ���� �߰� �� Heal �տ� ���� ��.
    public enum Pattern { Idle, Fist, Click, Gaurd, Heal };
    int pattern = 0; // ������ Idle����
    private int NextPattern;
    public float PatternCoolTime;

    // Idel Pattern
    public float IdelDurationTime;

    // Fist Pattern
    public float DashSpeed = 20f;
    public float DashDuration = 0.5f;
    private bool isDashing = false;

    // Click Pattern
    public float ClickAttackDuration;
    public GameObject BulletPrefab;
    public Transform FirePoint;
    public float BulletSpeed = 10f;

    // Gaurd Pattern
    public float GaurdMoveSpeed;

    // Heal Pattern
    public bool HealPatternReady = true;
    private float rotationAngle = 180f;
    private float duration = 1.0f;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        BaseHP = HP;
        monsterType = MonsterType.Boss_Mouse;
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
            if (!DetectionPlayerPosition())
                Debug.Log("�÷��̾� ����");

            switch (pattern)
            {
                case (int)Pattern.Idle:
                    yield return Idle();
                    break;
                case (int)Pattern.Fist:
                    yield return Fist();
                    break;
                case (int)Pattern.Click:
                    yield return Click();
                    break;
                case (int)Pattern.Gaurd:
                    yield return Gaurd();
                    break;
                case (int)Pattern.Heal:
                    yield return Heal();
                    break;
            }

            yield return new WaitForSeconds(PatternCoolTime);

            // �������� Heal�������� ���X
            do
            {
                NextPattern = Random.Range(0, System.Enum.GetValues(typeof(Pattern)).Length - 1);
            } while (NextPattern == pattern);
            pattern = NextPattern;

            if (HealPatternReady && HP < BaseHP * 0.3f)
            {
                HealPatternReady = false;
                HP += BaseHP * 0.3f;
                pattern = (int)Pattern.Heal;
            }

            yield return null;
        }
    }

    // <idle> - ��� ��� (�⺻ ���콺), �÷��̾� �������� ������ ������.
    public IEnumerator Idle()
    {
        var speed = MoveSpeed * Time.deltaTime;
        float elapsedTime = 0f;
        float preparationTime = 1.5f;

        while (elapsedTime < preparationTime)
        {
            DetectionPlayerPosition();

            Vector3 direction = TargetPosition - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90 - 22);

            rb.MovePosition(Vector3.MoveTowards(rb.position, TargetPosition, MoveSpeed * Time.fixedDeltaTime));

            elapsedTime += Time.deltaTime;

            yield return new WaitForFixedUpdate();
        }
        rb.velocity = Vector3.zero;
    }

    // <Fist> - [���� ����1] (�ָ�) �÷��̾� ������ ����(0.5�� ��) ���� �ӵ��� ����. �� or �÷��̾�� ������ ����.
    public IEnumerator Fist()
    {
        rb.mass = 100.0f;
        float elapsedTime = 0f;
        float dashDuration = 0.5f;
        isDashing = true;
        MAnimator.SetBool("Fist", true);
        DetectionPlayerPosition();

        Vector3 direction = (TargetPosition - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);

        while (elapsedTime < dashDuration)
        {
            rb.MovePosition(rb.position + (Vector2)(direction * DashSpeed * Time.fixedDeltaTime));
            elapsedTime += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        rb.velocity = Vector3.zero;
        MAnimator.SetBool("Fist", false);
        isDashing = false;
        Debug.Log("Fist ���� �ð� ���� �� �̵� ����");
        yield return new WaitForSeconds(1.0f);
        rb.mass = 1.0f;
        // �÷��̾ �ٽ� �ٶ󺸵��� ����
        DetectionPlayerPosition();
        direction = (TargetPosition - transform.position).normalized;
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90 - 22);
    }

    // <Click> - [���� ����2] �÷��̾� ������ �ٶ󺸰� �հ��� ������. �����ӵ��� źȯ �߻�. ������. 
    public IEnumerator Click()
    {
        MAnimator.SetBool("Click", true);
        float ElapsedTime = 0f;
        while (ElapsedTime < ClickAttackDuration)
        {
            DetectionPlayerPosition();

            Vector3 direction = TargetPosition - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90 - 22);

            Fire();

            ElapsedTime += 0.05f;
            yield return new WaitForSeconds(0.05f);
        }

        MAnimator.SetBool("Click", false);
        Debug.Log("Click ���� �ð� ���� �� �̵� ����");
    }

    // <Gaurd> - [��� ����] , �÷��̾� ������ ���� ������ ������. ������ 1/4�� �ޱ� ���ӽð� 10��
    public IEnumerator Gaurd()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        MAnimator.SetBool("Gaurd", true);
        DefenseRate = 1.0f / 4.0f;
        float elapsedTime = 0f;
        float dashDuration = 10.0f;

        while (elapsedTime < dashDuration)
        {
            DetectionPlayerPosition();

            Vector3 direction = (TargetPosition - transform.position).normalized;
            rb.MovePosition(rb.position + (Vector2)(direction * GaurdMoveSpeed * Time.fixedDeltaTime));

            elapsedTime += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        MAnimator.SetBool("Gaurd", false);
        DefenseRate = 1.0f;
    }

    // <Heal> - ���ڸ� ����, ü���� 30% ������ ���������� 10�ʰ� ü���� 30%�� ȸ����. <�̶� ������ ������ ������ ȸ������ ���� ������> (�𷡽ð�)
    public IEnumerator Heal()
    {
        Debug.Log("Heal");
        // ȸ�� �ʱ�ȭ
        transform.rotation = Quaternion.Euler(0, 0, 0);

        MAnimator.SetTrigger("Heal"); // �ִϸ��̼� Ʈ���� Ȱ��ȭ

        // �ִϸ��̼��� 2�� ���, 1�ʰ� ȸ��, 2�ʰ� �������� �ɷ� �ٽ� ����ؾ� ��
        for (int i = 0; i < 2; i++)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            yield return new WaitForSeconds(1.9f);
            
            float ElapsedTime = 0f;
            float initialAngle = transform.eulerAngles.z;
            float targetAngle = initialAngle + rotationAngle;

            while (ElapsedTime < duration)
            {
                float currentAngle = Mathf.Lerp(initialAngle, targetAngle, ElapsedTime / duration);
                transform.rotation = Quaternion.Euler(0, 0, currentAngle);
                ElapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }

    private void Fire()
    {
        GameObject bullet = Instantiate(BulletPrefab, FirePoint.position, FirePoint.rotation);
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        Vector2 fireDirection = (TargetPosition - FirePoint.position).normalized;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = fireDirection * BulletSpeed;
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDashing)
        {
            if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Wall"))
            {
                Debug.Log("�浹 ����");
                isDashing = false;
                rb.velocity = Vector2.zero;
            }
        }
    }
}
