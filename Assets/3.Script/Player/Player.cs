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

    //플레이어 카메라
    public Camera camera;

    // Manager
    private UIManager uiManager;
    private StatusManager statusManager;

    // 임시 특수공격
    public bool isSpecilHit = false;
    public int lastInt = 0;
    public int randomInt = 0;

    public GameObject window1;
    public GameObject window2;
    public GameObject window3;
    public GameObject window4;


    // Program(Temp)
    /*
    public bool EnomyDelete = false;
    private float DelayTime = 1.5f;
    private float NextDeleteTime = 0.0f;
    public float detectionRadius = 5.0f; 
    */

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
        // 몬스터 강제삭제 알고리즘
        // FDeleteMonster();
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
        playerRigid.velocity = moveVelocity.normalized * statusManager.MoveSpeed;
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

        if (mousePosition.y > transform.position.y)
        {
            // 각도 계산
            float angle = Vector3.Angle(Vector3.up, direction);

            // 각도가 -30도에서 30도 사이에 있는지 확인
            if (angle <= statusManager.AngleRange)
            {
                anim.SetBool("isBack", true); // 조건을 만족했을 때 isBack 값을 true로 설정
            }
            else
            {
                anim.SetBool("isBack", false); // 조건을 만족하지 않았을 때 isBack 값을 false로 설정
            }
        }
        else
        {
            anim.SetBool("isBack", false); // 마우스가 캐릭터 아래에 있을 때 isBack 값을 false로 설정
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
        weapon.transform.position = transform.position + direction * statusManager.WeaponDistance;
    }
   
    public Weapon GetWeapon()
    {
        return weapon;
    }
    public void SpecialHit()
    {
        if (isSpecilHit == false)
        {
            Debug.Log("발동!");
            isSpecilHit = true;
            randomInt = Random.Range(1, 4);

            while (randomInt == lastInt)
            {
                randomInt = Random.Range(1, 4);
                InfiniteLoopDetector.Run();
            }

        lastInt = randomInt;
        switch (randomInt)
            {
            case 1:
                StartCoroutine(PlayerYflip());
                break;
            case 2:
                StartCoroutine(Window12());
                break;
            case 3:
                StartCoroutine(Window34());
                break;
            }
        }
        else
        {
            Debug.Log("특수공격 발동 도중이라 효과 없음.");
        }

    }
    IEnumerator PlayerYflip()
    {
        Debug.Log("패턴1");
        playerSprite.flipY = true;
        // 10초 대기
        Debug.Log("10초 후 대기");
        yield return new WaitForSeconds(10f);
        playerSprite.flipY = false;
        yield return new WaitForSeconds(1f);
        isSpecilHit = false;
        Debug.Log("플레이어 Yflip코루틴 끝");
    }

    IEnumerator Window12()
    {
        Debug.Log("패턴2");
        window1.SetActive(true);
        window2.SetActive(true);
        Debug.Log("5초 후 대기");
        yield return new WaitForSeconds(5f);
        isSpecilHit = false;
        Debug.Log("패턴2 끝");
    }
    IEnumerator Window34()
    {
        Debug.Log("패턴3");
        window3.SetActive(true);
        window4.SetActive(true);
        Debug.Log("5초 후 대기");
        yield return new WaitForSeconds(5f);
        isSpecilHit = false;
        Debug.Log("패턴3 끝");
    }


    // ====== 몬스터 강제 삭제 알고리즘(수정 중) ======
    /*
    public void FDeleteMonster()
    {
        if (EnomyDelete)
        {
            if (Time.time >= NextDeleteTime)
            {
                // 캐릭터 주변의 모든 몬스터 탐색
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

                NextDeleteTime = Time.time + DelayTime; // 다음 삭제 가능 시간 업데이트
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
    */
    // ====== 몬스터 강제 삭제 알고리즘(수정 중) ======
}
