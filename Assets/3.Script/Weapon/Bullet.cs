using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5f; // 총알 속도
    public int damage = 10; // 총알 데미지
    public Rigidbody2D rb;
    public float lifetime = 10f; // 총알이 활성화된 상태를 유지할 시간
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
            // 총알이 활성화될 때 방향을 설정합니다.
            rb.velocity = Vector2.zero; // 기존 속도를 리셋
            rb.angularVelocity = 0f; // 기존 각속도 리셋

            Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;

            // 발사 방향으로 힘을 적용
            rb.AddForce(direction * speed, ForceMode2D.Impulse);
        }
        StartCoroutine(DisableAfterTime(lifetime));
    }
    private IEnumerator DisableAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false); // 비활성화
        pool.bulletPool.Enqueue(this);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        gameObject.SetActive(false); // 비활성화

    }
}


