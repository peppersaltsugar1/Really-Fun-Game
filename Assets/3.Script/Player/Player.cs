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

    public void Move() //플레이어 이동
    {
        Vector3 moveVelocity = Vector3.zero;

        
        // Horizontal 입력 처리
        float h = Input.GetAxisRaw("Horizontal");
        if (h < 0)
        {
            moveVelocity += Vector3.left;
        }
        else if (h > 0)
        {
            moveVelocity += Vector3.right;
        }

        // Vertical 입력 처리
        float v = Input.GetAxisRaw("Vertical");
        if (v > 0)
        {
            moveVelocity += Vector3.up;
        }
        else if (v < 0)
        {
            moveVelocity += Vector3.down;
        }

        // Rigidbody에 속도 적용
        playerRigid.velocity = moveVelocity.normalized * moveSpeed;
        // 애니메이터 달리기 스피드
        anim.SetFloat("Speed", moveVelocity.magnitude);
    }

    public void RotatePlayer()//플레이어 보는 방향에 따라 스프라이트 변경
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // 2D 환경이므로 z축을 0으로 설정
        // 캐릭터와 마우스 포인터 간의 방향 벡터 계산
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

    }
    void RotateWeapon()//무기 위치조정
    {
        // 마우스 포인터 위치 가져오기
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // 2D 환경이므로 z축을 0으로 설정

        // 캐릭터와 마우스 포인터 간의 방향 벡터 계산
        Vector3 direction = mousePosition - transform.position;
        direction.Normalize();

        // 무기 위치 설정
        weapon.transform.position = transform.position + direction * weaponDistance;
    }
}
