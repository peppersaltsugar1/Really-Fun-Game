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
        if (Input.GetMouseButton(0)) // ���� ���콺 ��ư Ŭ�� ����
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
        attackSpeed = 1.5f;
    }
    public void Fire()
    {
        PoolingManager poolingManager = PoolingManager.Instance;
        if (poolingManager.bulletPool.Count > 0)
        {
            Bullet bullet = poolingManager.bulletPool.Dequeue();
            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation;
            bullet.gameObject.SetActive(true);
        }
    }
    private void TryFire()
    {
        // ���� �ð��� ���� ���� �ð����� ũ�ų� ���� ���� ����
        if (Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + attackSpeed; // ���� ���� ������ �ð� ������Ʈ
        }
    }
}
