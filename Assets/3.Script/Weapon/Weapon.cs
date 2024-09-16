using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private GameObject[] bulletList;
    private Transform firePoint;
    public float attackSpeed;
    private float nextFireTime = 0f;
    public enum WeaponType
    {
        BasicWeapon
    }
    public WeaponType weaponType;
    // Start is called before the first frame update
    void Start()
    {
        weaponType = WeaponType.BasicWeapon;
        firePoint = gameObject.transform;
        UseBasicWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0)) // 왼쪽 마우스 버튼 클릭 감지
        {
            TryFire();
        }
    }

    public void UseWeapon()
    {
        switch (weaponType)
        {
            case WeaponType.BasicWeapon:
                UseBasicWeapon();
                break;
        }
    }

    public void UseBasicWeapon()
    {
        attackSpeed =0.5f;
    }
    public void Fire()
    {
        PoolingManager poolingManager = PoolingManager.Instance;
        if (poolingManager.bulletPool.Count > 0)
        {
            Bullet bullet = poolingManager.bulletPool.Dequeue();

            // 총알의 발사 위치와 방향을 설정합니다.
            Vector3 firePosition = transform.position;
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f; // 2D 환경에서 z축을 0으로 설정
            Vector3 direction = (mousePosition - firePosition).normalized; // 방향 벡터를 정규화

            bullet.transform.position = firePosition;
            bullet.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
            bullet.gameObject.SetActive(true);

            // Rigidbody2D를 사용하여 총알을 발사합니다.
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                bulletRb.velocity = direction * bullet.speed; // 일정한 속도로 발사
            }
        }
        /*PoolingManager poolingManager = PoolingManager.Instance;
        if (poolingManager.bulletPool.Count > 0)
        {
            Bullet bullet = poolingManager.bulletPool.Dequeue();
            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation;
            bullet.gameObject.SetActive(true);
        }*/
    }
    private void TryFire()
    {
        // 현재 시간이 다음 공격 시간보다 크거나 같을 때만 공격
        if (Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + attackSpeed; // 다음 공격 가능한 시간 업데이트
        }
    }
}
