using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Weapon weapon;
    //플레이어 에임
    public Transform sPoint;
    private float angleRange = 35f; // 최소 각도
    //플레이어체력관련
    public float maxHp; // 최대 체력
    public float currentHp; // 현재 체력
    public float temHp; // 임시 체력(아이템
    public float shield; // 공격 막아주는 것
    public float shieldHp; // 
    public float elect; // 
    //플레이어공격관련
    public float atk; // 공격력
    public float atkSpeed; // 공격속도
    public float pushPower; // 어택시 밀격
    //플레이어 이동속도
    public float moveSpeed;
    //그외 플레이어 정보
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
    //플레이어 카메라
    public Camera camera;
    // 프로그램
    public bool EnomyDelete = false;
    private float DelayTime = 1.5f;
    private float NextDeleteTime = 0.0f;
    public float detectionRadius = 5.0f; 

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
        FDeleteMonster();
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

        if (mousePosition.y > transform.position.y)
        {
            // 각도 계산
            float angle = Vector3.Angle(Vector3.up, direction);

            // 각도가 -30도에서 30도 사이에 있는지 확인
            if (angle <= angleRange)
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
            Debug.Log("체력이까임");
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
        Debug.Log("뒤짐");
    }

    public void Heal(int healNum)
    {
        if (healing_Co != null)
        {
            StopCoroutine(Healing_Co(healNum));
        }
        healing_Co = StartCoroutine(Healing_Co(healNum));


        Debug.Log("HP+");
    }
    public void Elect()
    {
        //번개배터리 터질경우 효과구현해야함
    }

    private IEnumerator Healing_Co(int healNum)
    {
        for (int i = 0; i < healNum; i++)
        {
            if (currentHp < maxHp)
            {
                currentHp += 1; // 체력을 증가시킵니다.
                currentHp = Mathf.Min(currentHp, maxHp); // 체력이 최대 체력을 넘지 않도록 합니다.
            }

            yield return new WaitForSeconds(healTime); // healTime만큼 대기합니다.
        }

        healing_Co = null; // 코루틴이 끝났음을 표시합니다.
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
        coin += coinNum;
    }
    public Weapon GetWeapon()
    {
        return weapon;
    }

    public void SetSpeed(float newSpeed)
    {
        this.moveSpeed += newSpeed;
    }

    // 수정 중. 몬스터 삭제 알고리즘.
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
}
