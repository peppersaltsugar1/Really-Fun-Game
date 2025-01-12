using Cinemachine;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class FolderNode : MonoBehaviour
{
    #region Definition

    [Header("���� �⺻����")]
    public GameObject CurrentFolder;
    public string FolderName; // ���� �̸�
    public FolderNode Parent; // �θ� ����
    public List<FolderNode> Children; // �ڽ� ������ (�ִ� 3��)
    public bool isDetectionDone = false;
    public List<MonsterBase> monsters = new List<MonsterBase>();

    public enum FolderType
    {
        Start,           // ���� ����
        Boss,            // ���� ����
        Shop,            // ���� ����
        Download,        // �ٿ�ε� ����
        RandomSpecial,   // ���� Ư�� ����
        Hidden,          // ������ ���� (Ư�� ���ǿ����� ���� ����)
        Middle,          // �Ϲ� ���� (��� ����)
        End,             // ��������Ʈ ���� (������ ��Ż ����)
        MiddleBoss       // �߰� ���� ����
    }

    public FolderType Type; // ���� ���� Ÿ��

    [SerializeField] private int MonsterCount; // ���� ������ ���� ��
    public bool IsCleared = false; // Ŭ���� ����
    public float nowPosition;

    // ��Ż
    [Header("���� ��Ż")]
    public Portal Left_Portal = null; // ���� ��Ż
    [Header("������ ��Ż ����Ʈ")]
    public Portal[] Portals; // ������ ��Ż ����Ʈ

    // ī�޶�
    public CinemachineVirtualCamera mapCamera;

    #endregion

    #region Default function

    private void Awake()
    {
        Children = new List<FolderNode>(); // �ڽ� ���� �ʱ�ȭ
    }
    void Update()
    {
        nowPosition = transform.position.x;
    }

    #endregion

    #region Setting FolderNode Connection

    // �θ� ������ ����
    public void SetParent(FolderNode parent)
    {
        Parent = parent;
    }

    // �ڽ� ���� �߰�
    public bool AddChild(FolderNode child)
    {
        if (Children.Count >= 3) // �ڽ��� �ִ� 3���� ��� �߰� �Ұ�
        {
            return false;
        }
        child.SetParent(this); // �ڽ��� �θ� ����
        Children.Add(child);
        return true;
    }

    #endregion

    #region Activation FolderNodeElement
    // ���� ������ Ȱ��ȭ
    public void SetFolderActive()
    {
        if (CurrentFolder == null) return;

        Debug.Log("SetFolderActive");
        CurrentFolder.SetActive(true);
    }

    // ���� ������ ��Ȱ��ȭ
    public void SetFolderDeActive()
    {
        if (CurrentFolder == null) return;

        // Debug.Log("SetFolderDeActive");
        CurrentFolder.SetActive(false);
    }

    // ���� ������ �ִ� ��Ż�� ���� Ȱ��ȭ
    public void ActivePortal()
    {
        // Debug.Log("ActivePortal");

        // ���� ��Ż Ȱ��ȭ
        if (Left_Portal != null)
        {
            Left_Portal.isActive = true;
            Left_Portal.SetClearTrigger();
        }

        // ������ ��Ż�� Ȱ��ȭ
        foreach (var portal in Portals)
        {
            portal.isActive = true;
            portal.SetClearTrigger();
        }
    }

    // ���� ������ �ִ� ��Ż�� ���� ��Ȱ��ȭ
    public void DeActivePortal()
    {
        // Debug.Log("DeActivePortal");

        // ���� ��Ż ��Ȱ��ȭ
        if (Left_Portal != null)
            Left_Portal.isActive = false;

        // ������ ��Ż�� ��Ȱ��ȭ
        foreach (var portal in Portals)
        {
            portal.DeActivePortal();
        }
    }

    // Ŭ���� ���ο� ���� ���� ���θ� üũ �� ��Ż�� Ȱ��ȭ ��Ŵ
    public void CheckCurrentFolder()
    {
        if (IsCleared)
        {
            ActivePortal();
            return;
        }

        if (MonsterCount > 0) return;
        else IsCleared = true;

        if (IsCleared == false) return;

        Debug.Log("Folder is cleared");
        ActivePortal();
    }

    #endregion

    #region Related to Monsters

    // ���� óġ �� ȣ��
    public void ChangeMonsterCount()
    {
        MonsterCount--;

        if (MonsterCount <= 0)
        {
            IsCleared = true;
            ActivePortal();
        }
    }

    public int GetMonsterCount()
    {
        return MonsterCount;
    }

    public List<MonsterBase> FindAndReturnMonster()
    {
        // �ڽ� ������Ʈ�� ��ȸ
        foreach (Transform child in transform)
        {
            // �ڽ� ������Ʈ�� MonsterBase ������Ʈ�� �ִ��� Ȯ��
            MonsterBase monster = child.GetComponent<MonsterBase>();

            // MonsterBase Ÿ���� ���� ��� ����Ʈ�� �߰�
            if (monster != null)
            {
                monsters.Add(monster);
            }
        }

        return monsters;
    }


    #endregion
}
