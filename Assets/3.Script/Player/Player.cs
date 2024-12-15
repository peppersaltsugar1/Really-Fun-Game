using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Weapon weapon;
    Rigidbody2D playerRigid;
    SpriteRenderer playerSprite;
    Animator anim;

    //�÷��̾� ī�޶�
    public Camera camera;

    // Manager
    private UIManager uiManager;
    private StatusManager statusManager;

    void Start()
    {
        playerRigid = gameObject.GetComponent<Rigidbody2D>();
        playerSprite = gameObject.GetComponent<SpriteRenderer>();
        anim = gameObject.GetComponent<Animator>();
        uiManager = UIManager.Instance;
        statusManager = StatusManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        RotateWeapon();
        RotatePlayer();
        // ���� �������� �˰���
        // FDeleteMonster();
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
        playerRigid.velocity = moveVelocity.normalized * statusManager.MoveSpeed;
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
            if (angle <= statusManager.AngleRange)
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
        weapon.transform.position = transform.position + direction * statusManager.WeaponDistance;
    }
   
    public Weapon GetWeapon()
    {
        return weapon;
    }
 


    // ====== ���� ���� ���� �˰���(���� ��) ======
    /*
    public void FDeleteMonster()
    {
        if (EnomyDelete)
        {
            if (Time.time >= NextDeleteTime)
            {
                // ĳ���� �ֺ��� ��� ���� Ž��
                Collider2D[] hitMonsters = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

                foreach (Collider2D collider in hitMonsters)
                {
                    Monster monster = collider.GetComponent<Monster>();

                    if (monster != null)
                    {
                        Destroy(monster.gameObject);
                        Debug.Log("Monster is deleted");
                        break;
                    }
                    else
                    {

                    }
                }

                NextDeleteTime = Time.time + DelayTime; // ���� ���� ���� �ð� ������Ʈ
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
    */
    // ====== ���� ���� ���� �˰���(���� ��) ======
}
