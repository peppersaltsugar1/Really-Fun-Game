using UnityEngine;
using UnityEngine.EventSystems;

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
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // Ŭ���� UI���� �߻��� ���, �ƹ� �۾��� ���� ����
                return;
            }

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
        attackSpeed = 0.5f;
    }
    public void Fire()
    {
        Debug.Log("Fire");
        PoolingManager poolingManager = PoolingManager.Instance;
        if (poolingManager.bulletPool.Count > 0)
        {
            Bullet bullet = poolingManager.bulletPool.Dequeue();

            // �Ѿ��� �߻� ��ġ�� ������ �����մϴ�.
            Vector3 firePosition = transform.position;
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f; // 2D ȯ�濡�� z���� 0���� ����
            Vector3 direction = (mousePosition - firePosition).normalized; // ���� ���͸� ����ȭ

            bullet.transform.position = firePosition;
            bullet.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
            bullet.gameObject.SetActive(true);

            // Rigidbody2D�� ����Ͽ� �Ѿ��� �߻��մϴ�.
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                bulletRb.velocity = direction * bullet.speed; // ������ �ӵ��� �߻�
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
        float attackInterval = 1f / StatusManager.Instance.AttackSpeed;

        if (Time.time >= nextFireTime)
        {
            Fire();
            SoundManager.Instance.PlayerFireSound();
            nextFireTime = Time.time + attackInterval; // ���� ���� ������ �ð� ������Ʈ
        }
    }

    public void SetAttackSpeed(float speed)
    {
        // Debug.Log("ChangeSpeed");
        attackSpeed += speed;

        if (attackSpeed < 0.1f)
        {
            attackSpeed = 0.1f;
        }

        // Debug.Log("Attack Speed set to: " + attackSpeed);
    }

}
