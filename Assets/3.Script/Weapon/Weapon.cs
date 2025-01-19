using UnityEngine;
using UnityEngine.EventSystems;

public class Weapon : MonoBehaviour
{
    #region Variables

    private GameObject[] bulletList;
    private Transform firePoint;
    private float nextFireTime = 0f;

    public enum WeaponType
    {
        BasicWeapon
    }

    public WeaponType weaponType;

    #endregion

    #region Manager

    private SpecialAttack specialAttack;
    private StatusManager statusManager;
    private PoolingManager poolingManager;
    private SoundManager soundManager;

    #endregion

    #region Default Function

    // Start is called before the first frame update
    void Start()
    {
        weaponType = WeaponType.BasicWeapon;
        firePoint = gameObject.transform;

        specialAttack = SpecialAttack.Instance;
        statusManager = StatusManager.Instance;
        poolingManager = PoolingManager.Instance;
        soundManager = SoundManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (!(specialAttack?.GetAttackMode() ?? false) && Input.GetMouseButton(0)) // ���� ���콺 ��ư Ŭ�� ����
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // Ŭ���� UI���� �߻��� ���, �ƹ� �۾��� ���� ����
                return;
            }

            TryFire();
        }
    }

    #endregion

    #region Fire

    private void TryFire()
    {
        float attackInterval = 1f / (statusManager?.AttackSpeed ?? 1.0f);

        if (Time.time >= nextFireTime)
        {
            Fire();
            soundManager?.PlayerFireSound();
            nextFireTime = Time.time + attackInterval; // ���� ���� ������ �ð� ������Ʈ
        }
    }

    public void Fire()
    {
        if (poolingManager?.bulletPool.Count > 0)
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
    }

    #endregion

}
