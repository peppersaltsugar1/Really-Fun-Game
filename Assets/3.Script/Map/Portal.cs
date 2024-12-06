using UnityEngine;

public class Portal : MonoBehaviour
{
    public enum PortalDirection { Left, Right } // ��Ż ����
    public PortalDirection Direction; // ���� ��Ż�� ����
    public int PortalIndex; // ���� ��Ż���� ��Ż �ε��� (���� ��Ż�� �ִ� �Ѱ��� �����ϰ�, �ε����� 0�� �⺻��,
                            // ������ ��Ż���� ������ ��Ż���� �ε����� ���ϸ� 0, 1, 2 ������ �Ҵ�)
    public FolderNode ConnectedFolder; // ����� ���� ����
    public int ParentPortalIndex = 0; // ���� ��Ż�� ��� ���� ������ �� ��° ��Ż�� ����� ��������
                                      // ��Ÿ���� �ε���(0 ~ 2 ����)

    private bool isActive = false; // ��Ż Ȱ��ȭ ����

    FolderManager folderManager = null;

    public void Start()
    {
        folderManager = FolderManager.Instance;
    }

    // ��Ż Ȱ��ȭ �Լ�
    public void ActivatePortal()
    {
        isActive = true;
    }

    public void DeActivatePortal()
    {
        isActive = false;
    }

    // �浹 ���� �Լ�
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

    // �÷��̾� ��ġ�� �̵���Ŵ
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
