using UnityEngine;

public class Portal : MonoBehaviour
{
    public enum PortalDirection { Left, Right } // 포탈 방향
    public PortalDirection Direction; // 현재 포탈의 방향
    public int PortalIndex; // 현재 포탈에서 포탈 인덱스 (왼쪽 포탈은 최대 한개만 존재하고, 인덱스는 0이 기본값,
                            // 오른쪽 포탈들은 오른쪽 포탈끼리 인덱스를 정하며 0, 1, 2 순으로 할당)
    public FolderNode ConnectedFolder; // 연결된 폴더 정보
    public int ParentPortalIndex = 0; // 왼쪽 포탈의 경우 상위 폴더의 몇 번째 포탈에 연결된 것인지를
                                      // 나타내는 인덱스(0 ~ 2 범위)

    private bool isActive = false; // 포탈 활성화 여부

    FolderManager folderManager = null;

    public void Start()
    {
        folderManager = FolderManager.Instance;
    }

    // 포탈 활성화 함수
    public void ActivatePortal()
    {
        isActive = true;
    }

    public void DeActivatePortal()
    {
        isActive = false;
    }

    // 충돌 감지 함수
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isActive)
        {
            Debug.Log("Portal Enter");
            MovePlayerToConnectedFolder();
        }
        else if (collision.CompareTag("Player") && !isActive)
        {
            Debug.Log($"Portal {Direction} {PortalIndex} is not active yet!");
        }
    }

    // 플레이어 위치를 이동시킴
    public void MovePlayerToConnectedFolder()
    {
        if (ConnectedFolder == null) return;

        Debug.Log($"Player moved to folder: {ConnectedFolder.FolderName} portalindex : {PortalIndex}");

        if (PortalIndex == 0)
        {
            folderManager.MoveToPreviousFolder();
        }
        else
        {
            folderManager.MoveToNextFolder(PortalIndex);
        }
    }
}
