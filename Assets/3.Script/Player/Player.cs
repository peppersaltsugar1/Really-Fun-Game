using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Weapon weapon;
    //�÷��̾� ����
    public Transform sPoint;
    private float angleRange = 35f; // �ּ� ����
    //�÷��̾�ü�°���
    public float maxHp;
    public float currentHp;
    public float temHp;
    public float shield;
    public float shieldHp;
    public float elect;
    //�÷��̾���ݰ���
    public float atk;
    public float atkSpeed;
    public float pushPower;
    //�÷��̾� �̵��ӵ�
    public float moveSpeed;
    //�׿� �÷��̾� ����
    public int coin;
    public float weaponDistance;
    private float healTime = 0.2f;
    private Coroutine healing_Co;
    [SerializeField]
    private Collider2D playerCol;
    Rigidbody2D playerRigid;
    SpriteRenderer playerSprite;
    Animator anim;
    private UIManager uiManager;
    private bool isHit = false;
    private float hitTime = 2.0f;
    //�÷��̾� ī�޶�
    public Camera camera;

    void Start()
    {
        playerRigid = gameObject.GetComponent<Rigidbody2D>();
        playerSprite = gameObject.GetComponent<SpriteRenderer>();
        anim = gameObject.GetComponent<Animator>();
        uiManager = UIManager.Instance;
        currentHp = maxHp;

    }

    // Update is called once per frame
    void Update()
    {
        Move();
        RotateWeapon();
        RotatePlayer();
    }

    public void Move() //�÷��̾� �̵�
    {
        Vector3 moveVelocity = Vector3.zero;


        // Horizontal �Է� ó��
        float h = Input.GetAxisRaw("Horizontal");
        if (h < 0)
        {
            moveVelocity += Vector3.left;
        }
        else if (h > 0)
        {
            moveVelocity += Vector3.right;
        }

        // Vertical �Է� ó��
        float v = Input.GetAxisRaw("Vertical");
        if (v > 0)
        {
            moveVelocity += Vector3.up;
        }
        else if (v < 0)
        {
            moveVelocity += Vector3.down;
        }

        // Rigidbody�� �ӵ� ����
        playerRigid.velocity = moveVelocity.normalized * moveSpeed;
        // �ִϸ����� �޸��� ���ǵ�
        anim.SetFloat("Speed", moveVelocity.magnitude);
    }

    public void RotatePlayer()//�÷��̾� ���� ���⿡ ���� ��������Ʈ ����
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // 2D ȯ���̹Ƿ� z���� 0���� ����
        // ĳ���Ϳ� ���콺 ������ ���� ���� ���� ���
        Vector3 direction = mousePosition - transform.position;
        direction.Normalize();
        if (mousePosition.x > transform.position.x)
        {
            playerSprite.flipX = true;
        }
        else if (mousePosition.x < transform.position.x)
        {
            playerSprite.flipX = false;
        }

        if (mousePosition.y > transform.position.y)
        {
            // ���� ���
            float angle = Vector3.Angle(Vector3.up, direction);

            // ������ -30������ 30�� ���̿� �ִ��� Ȯ��
            if (angle <= angleRange)
            {
                anim.SetBool("isBack", true); // ������ �������� �� isBack ���� true�� ����
            }
            else
            {
                anim.SetBool("isBack", false); // ������ �������� �ʾ��� �� isBack ���� false�� ����
            }
        }
        else
        {
            anim.SetBool("isBack", false); // ���콺�� ĳ���� �Ʒ��� ���� �� isBack ���� false�� ����
        }

    }
    void RotateWeapon()//���� ��ġ����
    {
        // ���콺 ������ ��ġ ��������
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // 2D ȯ���̹Ƿ� z���� 0���� ����

        // ĳ���Ϳ� ���콺 ������ ���� ���� ���� ���
        Vector3 direction = mousePosition - transform.position;
        direction.Normalize();

        // ���� ��ġ ����
        weapon.transform.position = transform.position + direction * weaponDistance;
    }
    public void TakeDamage(float atk)
    {
        if (shieldHp > 0)
        {
            shieldHp -= atk;
            uiManager.ShiledSet();
            StartCoroutine(Hit_co());

            if (shieldHp < 0)
            {
                shieldHp = 0;
            }
            shield = maxHp / 3;
            return;
        }
        if (elect > 0)
        {
            elect -= atk;
            uiManager.ElectDel();
            StartCoroutine(Hit_co());

            if (elect < 0)
            {
                elect = 0;
            }
            Elect();
            return;
        }
        if (temHp > 0)
        {
            temHp -= atk;
            uiManager.TemHpSet();
            StartCoroutine(Hit_co());

            if (temHp <= 0)
            {
                uiManager.TemHpDel();
                temHp = 0;
            }
            return;
        }
        if (shield > 0)
        {
            if (shield * 3 >= currentHp)
            {
                shield -= 1;
                uiManager.ShiledOff();
                StartCoroutine(Hit_co());

                if (shield < 0)
                {
                    shield = 0;
                }
                return;
            }
        }
        if (currentHp > 0)
        {
            Debug.Log("ü���̱���");
            currentHp -= atk;
            uiManager.HpSet();
            StartCoroutine(Hit_co());
            if (currentHp <= 0)
            {
                Die();
            }
            return;
        }
    }
    private void Die()
    {
        Debug.Log("����");
    }
    
    public void Heal(int healNum)
    {
        if (healing_Co != null)
        {
            StopCoroutine(Healing_Co(healNum));
        }
        healing_Co = StartCoroutine(Healing_Co(healNum));
    }
    public void Elect()
    {
        //�������͸� ������� ȿ�������ؾ���
    }

    private IEnumerator Healing_Co(int healNum)
    {
        for (int i = 0; i < healNum; i++)
        {
            if (currentHp < maxHp)
            {
                currentHp += 1; // ü���� ������ŵ�ϴ�.
                currentHp = Mathf.Min(currentHp, maxHp); // ü���� �ִ� ü���� ���� �ʵ��� �մϴ�.
            }

            yield return new WaitForSeconds(healTime); // healTime��ŭ ����մϴ�.
        }

        healing_Co = null; // �ڷ�ƾ�� �������� ǥ���մϴ�.
    }
    private IEnumerator Hit_co()
    {
        isHit = true;
        playerCol.enabled = false;

        yield return new WaitForSeconds(hitTime);

        playerCol.enabled = true;
        isHit = false;
    }
    public void TemHpUp(int temHpNum)
    {
        temHp += temHpNum;
        uiManager.TemHpSet();
    }
    public void ShileHpUp(int shuldNum)
    {
        shieldHp += shuldNum;
    }

    public void ElectUp(int electNum)
    {
        elect += electNum;
    }
    public void CoinUp(int coinNum)
    {

    }
}
