using System.Collections;
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


    // ����� ������ �ۺ����� ������.
    // private bool isActive = true; // ��Ż Ȱ��ȭ ����
    // private bool isMoving = false; // ��Ż �̵� �� ����
    public bool isActive = true; // ��Ż Ȱ��ȭ ����
    public bool isMoving = false; // ��Ż �̵� �� ����
    FolderManager folderManager = null;

    public void Start()
    {
        folderManager = FolderManager.Instance;
    }

    // OnTriggerEnter2D, OnTriggerStay2D �� �ΰ��� ������ ����
    // �÷��̾ ���� ���¸� üũ�ؼ� �̵���Ű�� ������ ����ϴ� �Լ�

    // �ݶ��̴� �浹 ���� �Լ�
    // Ȱ��ȭ�� ��Ż�� �� ��쿡 �÷��̾ �̵���Ų��.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ��Ż�� ��Ȱ��ȭ�ų� �̵� ���̸� ���� X
        if (!isActive || isMoving) 
            return;

        // Ŭ���� ���� üũ(Ŭ��� �ƴϸ� �̵��� �ʿ� X)
        if (folderManager != null && !folderManager.IsClear())
            return;

        if (collision.CompareTag("Player") && isActive)
        {
            Debug.Log("Portal Enter");
            isActive = false;
            isMoving = true;

            // ����� ������ �̵�
            MovePlayerToConnectedFolder();
        }
        else if (collision.CompareTag("Player") && !isActive)
        {
            Debug.Log($"Portal {Direction} {PortalIndex} is not clear yet!");
        }
    }

    // �ݶ��̴� ���� �������� �����ϴ� �Լ�.
    // ��Ż�� �����ڸ��� �ٽ� �ݶ��̴� ������ ���� ��� �÷��̾ �̵���Ŵ
    private void OnTriggerStay2D(Collider2D collision)
    {
        // ��Ż�� ��Ȱ��ȭ�ų� �̵� ���̸� ���� X
        if (!isActive || isMoving)
            return;

        // Ŭ���� ���� üũ(Ŭ��� �ƴϸ� �̵��� �ʿ�X)
        if (folderManager != null && !folderManager.IsClear())
            return;

        if (collision.CompareTag("Player") && isActive)
        {
            Debug.Log("Portal Enter");
            isActive = false;
            isMoving = true;  // �̵� �� �÷��� ����

            // ����� ������ �̵���Ų��.
            MovePlayerToConnectedFolder();
        }
        else if (collision.CompareTag("Player") && !isActive)
        {
            Debug.Log($"Portal {Direction} {PortalIndex} is not clear yet!");
        }
    }

    // �ݶ��̴� Ż�� ���� �Լ�
    // �ݶ��̴��� ����� ���� �� ��Ż�� Ȱ��ȭ��Ų��.
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (folderManager == null)
        {
            Debug.Log("folderManager is null");
            return;
        }

        // ��Ȱ��ȭ ������Ʈ������ ������� ����(�̵� �� ���� ��Ż)
        if (!isActive)
            return;

        // Ȱ��ȭ�� ��Ż�� Ż���ϸ�
        if (collision.CompareTag("Player") && isActive)
        {
            Debug.Log("Portal Exit");

            StartCoroutine(DelayAfterPortalActive(1.5f));
        }
    }

    private IEnumerator DelayAfterPortalActive(float delay)
    {
        // delay��ŭ ���
        yield return new WaitForSeconds(delay);
        
        // ���� �� ������ �Լ� ȣ��
        Debug.Log("DelayAfterPortalActive");
        isMoving = false;

        if (folderManager.PreviousPortal != null)
            folderManager.PreviousPortal.isMoving = false;
    }


    // �÷��̾� ��ġ�� �̵���Ų��.
    // ��Ż ���⿡ ���� �̵���Ŵ(Left, Right)
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

    // ��Ż ����� �Լ�
    // �� �����⿡�� ���
    public void SetConnectedFolder(FolderNode conneted, int parentPortalIndex)
    {
        ConnectedFolder = conneted;
        ParentPortalIndex = parentPortalIndex;
    }

    public void SetIsMovingFalse()
    {
        isMoving = false;
    }
}
