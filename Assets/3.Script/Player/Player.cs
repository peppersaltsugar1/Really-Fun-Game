using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Weapon weapon;
    public Transform sPoint;
    public float hp;
    public float atk;
    public float atkSpeed;
    public float moveSpeed;
    public float weaponDistance;
    public float pushPower;
    public int coin;
    private float angleRange = 35f; // �ּ� ����


    Rigidbody2D playerRigid;
    SpriteRenderer playerSprite;
    Animator anim;

    void Start()
    {
        playerRigid = gameObject.GetComponent<Rigidbody2D>();
        playerSprite = gameObject.GetComponent<SpriteRenderer>();
        anim = gameObject.GetComponent<Animator>();
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

        hp = hp - atk;
        Debug.Log("�ĸ���");
    }
}
