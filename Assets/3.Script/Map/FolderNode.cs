using System.Collections.Generic;
using UnityEngine;

public class FolderNode : MonoBehaviour
{
    [Header("���� �⺻����")]
    public GameObject CurrentFolder;
    public string FolderName; // ���� �̸�
    public FolderNode Parent; // �θ� ����
    public List<FolderNode> Children; // �ڽ� ������ (�ִ� 3��)

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

    private void Awake()
    {
        Children = new List<FolderNode>(); // �ڽ� ���� �ʱ�ȭ
    }
    void Update()
    {
        nowPosition = transform.position.x;
    }

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
        foreach (var portal in Portals)
        {
            portal.ActivatePortal();
        }
        if (Left_Portal != null)
            Left_Portal.ActivatePortal();
    }

    // ���� ������ �ִ� ��Ż�� ���� ��Ȱ��ȭ
    public void DeActivePortal()
    {
        // Debug.Log("DeActivePortal");
        foreach (var portal in Portals)
        {
            portal.DeActivatePortal();
        }
        if (Left_Portal != null)
            Left_Portal.DeActivatePortal();
    }

    // Ŭ���� ���ο� ���� ���� ���θ� üũ �� ��Ż�� Ȱ��ȭ ��Ŵ
    public void CheckCurrentFolder()
    {
        if (MonsterCount > 0) return;
        else IsCleared = true;

        if (IsCleared == false) return;

        Debug.Log("Folder is cleared");
        ActivePortal();
    }

    // ���� óġ �� ȣ��
    public void OnMonsterDefeated()
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

}
