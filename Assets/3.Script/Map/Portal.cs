using System.Collections;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Portal : MonoBehaviour
{

    public enum PortalDirection { Left, Right } // ��Ż ����

    [Header("��Ż �⺻����")]
    public PortalDirection Direction; // ���� ��Ż�� ����
    public int PortalIndex; // ���� ��Ż���� ��Ż �ε��� (���� ��Ż�� �ִ� �Ѱ��� �����ϰ�, �ε����� 0�� �⺻��,
                            // ������ ��Ż���� ������ ��Ż���� �ε����� ���ϸ� 0, 1, 2 ������ �Ҵ�)
    public FolderNode ConnectedFolder; // ����� ���� ����
    public int ParentPortalIndex = 0; // ���� ��Ż�� ��� ���� ������ �� ��° ��Ż�� ����� ��������
                                      // ��Ÿ���� �ε���(0 ~ 2 ����)

    [Header("�̵� ���� ����")]
    public bool isActive = true; // ��Ż Ȱ��ȭ ����
    public bool isMoving = false; // ��Ż �̵� �� ����
    public bool isLocking = false; // ���� ��� ����
    FolderManager folderManager = null;

    // ��Ÿ ����
    private Animator animator = null;
    public Animator childAnimator = null;
    public SpriteRenderer childspriteRenderer = null;

    // Ű ������ ���
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

    // OnTriggerEnter2D, OnTriggerStay2D �� �ΰ��� ������ ����
    // �÷��̾ ���� ���¸� üũ�ؼ� �̵���Ű�� ������ ����ϴ� �Լ�

    // �ݶ��̴� �浹 ���� �Լ�
    // Ȱ��ȭ�� ��Ż�� �� ��쿡 �÷��̾ �̵���Ų��.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �÷��̾� ����
        if (collision.CompareTag("Player") == false)
            return;

        // Ŭ���� ���� üũ
        if (folderManager != null && !folderManager.IsClear())
            return;

        // ��Ż�� (��Ȱ��ȭ or �̵� ��) �̸� ���� X
        if (isActive == false || isMoving == true)
            return;

        // ����(�ٿ�ε�, ����)�� ��� 
        if (isLocking == true && itemManager.KeyUse())
        {
            // Ű�� �̿��� �̵� �ϴ��ϰ� �ϴ� ����.
            // ��Ż�� ��Ȱ��ȭ �� �ִϸ��̼� ��� -> ����� ������ ��Ż Ȱ��ȭ
            isActive = false;
            animator.SetBool("KeyOpen", true);
            childAnimator.SetBool("KeyOpen", true);
            childspriteRenderer.sprite = null;
            isLocking = false;
            StartCoroutine(DelayAfterPortalActive(1.5f));
        }
        else if (isLocking == false)
        {
            // �������� ���� �۵� �κ�
            Debug.Log("Portal Enter");
            isActive = false;
            isMoving = true;

            // ����� ������ �̵�
            MovePlayerToConnectedFolder();
        }

        // ��Ż�� ������ ��� �����̸�(Ű ��� ����) ���� X
        if (isLocking)
            return;
    }



    // �ݶ��̴� ���� �������� �����ϴ� �Լ�.
    // ��Ż�� �����ڸ��� �ٽ� �ݶ��̴� ������ ���� ��� �÷��̾ �̵���Ŵ
    private void OnTriggerStay2D(Collider2D collision)
    {
        // �÷��̾� ����
        if (collision.CompareTag("Player") == false)
            return;

        // Ŭ���� ���� üũ
        if (folderManager != null && !folderManager.IsClear())
            return;

        // ��Ż�� (��Ȱ��ȭ or �̵� ��) �̸� ���� X
        if (isActive == false || isMoving == true)
            return;

        // ����(�ٿ�ε�, ����)�� ��� 
        if (isLocking == true && itemManager.KeyUse())
        {
            // Ű�� �̿��� �̵� �ϴ��ϰ� �ϴ� ����.
            // ��Ż�� ��Ȱ��ȭ �� �ִϸ��̼� ��� -> ����� ������ ��Ż Ȱ��ȭ
            isActive = false;
            animator.SetBool("KeyOpen", true);
            childAnimator.SetBool("KeyOpen", true);
            childspriteRenderer.sprite = null;
            isLocking = false;
            StartCoroutine(DelayAfterPortalActive(1.5f));
        }
        else if(isLocking == false)
        {
            // �������� ���� �۵� �κ�
            Debug.Log("Portal Enter");
            isActive = false;
            isMoving = true;

            // ����� ������ �̵�
            MovePlayerToConnectedFolder();
        }

        // ��Ż�� ������ ��� �����̸�(Ű ��� ����) ���� X
        if (isLocking)
            return;
    }

    private IEnumerator DelayAfterPortalActive(float delay)
    {
        // delay��ŭ ���
        yield return new WaitForSeconds(delay);

        isActive = true;
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

            StartCoroutine(DelayAfterPortalisMoving(1.5f));
        }
    }

    private IEnumerator DelayAfterPortalisMoving(float delay)
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
            ConnectedFolder.Left_Portal.isMoving = true;
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

    public void SetClearTrigger()
    {
        if (animator == null)
        {
            Debug.LogError("animator is null");
            return;
        }

        switch (ConnectedFolder.name)
        {
            case "Donwload_room(Clone)":
                // �̺�Ʈ ó��
                Debug.Log("Donwload_room Trigger is not Set");
                break;
            case "Store_room(Clone)":
                // �̺�Ʈ ó��
                Debug.Log("Store_room Trigger is not Set");
                break;
            default:
                animator.SetBool("Clear", true);
                break;
        }
    }
}
