using System.Collections;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Portal : MonoBehaviour
{

    public enum PortalDirection { Left, Right } // 포탈 방향

    [Header("포탈 기본정보")]
    public PortalDirection Direction; // 현재 포탈의 방향
    public int PortalIndex; // 현재 포탈에서 포탈 인덱스 (왼쪽 포탈은 최대 한개만 존재하고, 인덱스는 0이 기본값,
                            // 오른쪽 포탈들은 오른쪽 포탈끼리 인덱스를 정하며 0, 1, 2 순으로 할당)
    public FolderNode ConnectedFolder; // 연결된 폴더 정보
    public int ParentPortalIndex = 0; // 왼쪽 포탈의 경우 상위 폴더의 몇 번째 포탈에 연결된 것인지를
                                      // 나타내는 인덱스(0 ~ 2 범위)

    [Header("이동 관련 변수")]
    public bool isActive = true; // 포탈 활성화 여부
    public bool isMoving = false; // 포탈 이동 중 여부
    public bool isLocking = false; // 열쇠 잠김 여부
    FolderManager folderManager = null;

    // 기타 변수
    private Animator animator = null;
    public Animator childAnimator = null;
    public SpriteRenderer childspriteRenderer = null;

    // 키 아이템 사용
    ItemManager itemManager;

    public void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Start()
    {
        folderManager = FolderManager.Instance;
        itemManager = ItemManager.Instance;
    }

    // OnTriggerEnter2D, OnTriggerStay2D 이 두개는 로직이 같음
    // 플레이어를 현재 상태를 체크해서 이동시키는 역할을 담당하는 함수

    // 콜라이더 충돌 감지 함수
    // 활성화된 포탈에 들어갈 경우에 플레이어를 이동시킨다.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 잠긴방(다운로드, 상점)의 경우 
        if(isLocking && itemManager.KeyUse())
        {
            // 키를 이용해 이동 하능하게 하는 로직.
            // 포탈을 비활성화 후 애니메이션 재생 -> 재생이 끝나면 포탈 활성화
            isActive = false;
            animator.SetBool("KeyOpen", true);
            childAnimator.SetBool("KeyOpen", true); 
            childspriteRenderer.sprite = null;
            isLocking = false;
            StartCoroutine(DelayAfterPortalActive(1.5f));
        }

        // 포탈이 (비활성화 or 이동 중 or 잠김) 이면 실행 X
        if (!isActive || isMoving || isLocking) 
            return;

        // 클리어 상태 체크(클리어가 아니면 이동할 필요 X)
        if (folderManager != null && !folderManager.IsClear())
            return;

        if (collision.CompareTag("Player") && isActive)
        {
            Debug.Log("Portal Enter");
            isActive = false;
            isMoving = true;

            // 연결된 폴더로 이동
            MovePlayerToConnectedFolder();
        }
        else if (collision.CompareTag("Player") && !isActive)
        {
            Debug.Log($"Portal {Direction} {PortalIndex} is not clear yet!");
        }
    }

    // 콜라이더 내부 움직임을 감지하는 함수.
    // 포탈을 나가자마자 다시 콜라이더 안으로 들어올 경우 플레이어를 이동시킴
    private void OnTriggerStay2D(Collider2D collision)
    {
        // 잠긴방(다운로드, 상점)의 경우 
        if (isLocking && itemManager.KeyUse())
        {
            // 키를 이용해 이동 하능하게 하는 로직.
            // 포탈을 비활성화 후 애니메이션 재생 -> 재생이 끝나면 포탈 활성화
            isActive = false;
            childAnimator = GetComponentInChildren<Animator>();
            animator.SetBool("KeyOpen", true);
            childAnimator.SetBool("KeyOpen", true);
            isLocking = false;
            StartCoroutine(DelayAfterPortalActive(1.5f));
        }

        // 포탈이 (비활성화 or 이동 중 or 잠김) 이면 실행 X
        if (!isActive || isMoving || isLocking)
            return;

        // 클리어 상태 체크(클리어가 아니면 이동할 필요X)
        if (folderManager != null && !folderManager.IsClear())
            return;

        if (collision.CompareTag("Player") && isActive)
        {
            Debug.Log("Portal Enter");
            isActive = false;
            isMoving = true;  // 이동 중 플래그 설정

            // 연결된 폴더로 이동시킨다.
            MovePlayerToConnectedFolder();
        }
        else if (collision.CompareTag("Player") && !isActive)
        {
            Debug.Log($"Portal {Direction} {PortalIndex} is not clear yet!");
        }
    }

    private IEnumerator DelayAfterPortalActive(float delay)
    {
        // delay만큼 대기
        yield return new WaitForSeconds(delay);

        isActive = true;
    }


    // 콜라이더 탈출 감지 함수
    // 콜라이더를 벗어나면 몇초 후 포탈을 활성화시킨다.
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (folderManager == null)
        {
            Debug.Log("folderManager is null");
            return;
        }

        // 비활성화 오브젝트에서는 사용하지 않음(이동 전 맵의 포탈)
        if (!isActive)
            return;

        // 활성화된 포탈을 탈출하면
        if (collision.CompareTag("Player") && isActive)
        {
            Debug.Log("Portal Exit");

            StartCoroutine(DelayAfterPortalisMoving(1.5f));
        }
    }

    private IEnumerator DelayAfterPortalisMoving(float delay)
    {
        // delay만큼 대기
        yield return new WaitForSeconds(delay);
        
        // 지연 후 실행할 함수 호출
        Debug.Log("DelayAfterPortalActive");
        isMoving = false;

        if (folderManager.PreviousPortal != null)
            folderManager.PreviousPortal.isMoving = false;
    }

    // 플레이어 위치를 이동시킨다.
    // 포탈 방향에 따라 이동시킴(Left, Right)
    public void MovePlayerToConnectedFolder()
    {
        if (ConnectedFolder == null) return;

        Debug.Log($"Player moved to folder: {ConnectedFolder.FolderName} portalindex : {PortalIndex}");

        if (Direction == PortalDirection.Left)
        {
            Debug.Log("Left");
            ConnectedFolder.Portals[ParentPortalIndex].isMoving = true;
            folderManager.MoveToPreviousFolder(ParentPortalIndex, this);
        }
        else
        {
            Debug.Log("Right");
            ConnectedFolder.Left_Portal.isMoving= true;
            folderManager.MoveToNextFolder(PortalIndex, this);
        }
    }

    // 포탈 연결용 함수
    // 맵 생성기에서 사용
    public void SetConnectedFolder(FolderNode conneted, int parentPortalIndex)
    {
        ConnectedFolder = conneted;
        ParentPortalIndex = parentPortalIndex;
    }

    public void SetIsMovingFalse()
    {
        isMoving = false;
    }

    public void SetClearTrigger()
    {
        if(animator == null)
        {
            Debug.LogError("animator is null");
            return;
        }

        switch(ConnectedFolder.name)
        {
            case "Donwload_room(Clone)":
                // 이벤트 처리
                Debug.Log("Donwload_room Trigger is not Set");
                break;
            case "Store_room(Clone)":
                // 이벤트 처리
                Debug.Log("Store_room Trigger is not Set");
                break;
            default:
                animator.SetBool("Clear", true);
                break;
        }
    }
}
