using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5f; // �Ѿ� �ӵ�
    public int damage = 10; // �Ѿ� ������
    public Rigidbody2D rb;
    public float lifetime = 10f; // �Ѿ��� Ȱ��ȭ�� ���¸� ������ �ð�
    private PoolingManager pool;
    // Start is called before the first frame update
    void Start()
    {
         pool = PoolingManager.Instance;

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        if (rb != null)
        {
            // �Ѿ��� Ȱ��ȭ�� �� ������ �����մϴ�.
            rb.velocity = Vector2.zero; // ���� �ӵ��� ����
            rb.angularVelocity = 0f; // ���� ���ӵ� ����

            Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;

            // �߻� �������� ���� ����
            rb.AddForce(direction * speed, ForceMode2D.Impulse);
        }
        StartCoroutine(DisableAfterTime(lifetime));
    }
    private IEnumerator DisableAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false); // ��Ȱ��ȭ
        pool.bulletPool.Enqueue(this);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        gameObject.SetActive(false); // ��Ȱ��ȭ

    }
}


