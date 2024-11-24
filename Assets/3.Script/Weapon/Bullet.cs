using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed; // �Ѿ� �ӵ�
    public Rigidbody2D rb;
    private float lifetime;
    [SerializeField]
    private Material bulletMa;
    private PoolingManager pool;
    private StatusManager statusManager;
    [Header("�Ѿ� �⺻��")]
    [SerializeField]
    private float sizeX;
    [SerializeField]
    private float sizeY;
    [SerializeField]
    private float sizeZ;
    [SerializeField]
    private float colorA;
    void Start()
    {
        pool = PoolingManager.Instance;
        statusManager = StatusManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (rb != null)
        {
            Vector2 currentDirection = rb.velocity.normalized;
            rb.velocity = currentDirection * statusManager.BulletSpeed;
            lifetime = statusManager.BulletMaximumRange / statusManager.BulletSpeed;
        }
    }

    private void OnEnable()
    {
        if (statusManager == null || rb == null)
        {
            statusManager = StatusManager.Instance;
            rb = GetComponent<Rigidbody2D>();
            if (rb == null || statusManager == null)
            {
                Debug.LogError("Bullet initialization error: StatusManager or Rigidbody2D is not set up.");
                return;
            }
        }

        // Debug.Log("Bullet activated with damage: " + statusManager.AttackPower);
        if (rb != null)
        {
            // �Ѿ��� Ȱ��ȭ�� �� ������ �����մϴ�.
            rb.velocity = Vector2.zero; // ���� �ӵ��� ����
            rb.angularVelocity = 0f; // ���� ���ӵ� ����

            Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;

            // �߻� �������� ���� ����
            rb.AddForce(direction * statusManager.BulletSpeed, ForceMode2D.Impulse);
        }
        lifetime = statusManager.BulletMaximumRange / statusManager.BulletSpeed;
        StartCoroutine(DisableAfterTime(lifetime));
    }
    private IEnumerator DisableAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false); // ��Ȱ��ȭ
        pool.bulletPool.Enqueue(this);
    }

    private void OnCollisionTrigger2D(Collision2D collision)
    {
        gameObject.SetActive(false); // ��Ȱ��ȭ
    }
    public void SizeReset()
    {
        transform.localScale = new Vector3(sizeX, sizeY, sizeZ);
        // ���� ���� ��������
        Color color = bulletMa.color;

        // ���İ��� ������ ������ ����
        color.a = colorA;

        // ���� �ٽ� ����
        bulletMa.color = color;
        
    }
}


