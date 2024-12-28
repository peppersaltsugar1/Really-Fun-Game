using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class FolderManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    private static FolderManager instance = null;

    public GameObject Player; // ĳ���� ��ġ

    public List<FolderNode> AllFolders = new List<FolderNode>(); // ��� ���� ����Ʈ
    public FolderNode CurrentFolder; // ���� ����
    public FolderGenerator FolderGenerator; // ���� ������
    public int CurrentFolderMonsterCount = 0;
    public FolderNode rootFolder;

    public Portal PreviousPortal = null;

    private UI_0_HUD ui_0_HUD; // HUD�� �����ϱ� ���� ����
    private UI_4_LocalDisk localDiskUI;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public static FolderManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    void Start()
    {
        if (FolderGenerator == null)
        {
            FolderGenerator = FindObjectOfType<FolderGenerator>();
            if (FolderGenerator == null)
            {
                Debug.LogError("FolderGenerator not found!");
                return;
            }
        }


        ui_0_HUD = UI_0_HUD.Instance;
        localDiskUI = UI_4_LocalDisk.Instance;

        // �� ���� ����
        GenerateMap();

        AllFolderDeActivate();

        SetCurrentFolder(CurrentFolder);

        if (localDiskUI != null && rootFolder != null)
        {
            localDiskUI.GenerateTreeUI(FolderGenerator.GetRootNode());
        }
    }

    // �� ���� ����
    public void GenerateMap()
    {
        FolderGenerator.GenerateMap();
        rootFolder = FolderGenerator.GetRootNode();

        if (rootFolder == null)
        {
            Debug.LogError("Root folder is null. Map generation failed.");
            return;
        }

        CurrentFolder = rootFolder; // ��Ʈ ������ ���� ������ ����
        Debug.Log("Map generation completed.");
    }

    // ���� ���� ����
    public void SetCurrentFolder(FolderNode folder)
    {
        if (folder == null) return;

        CurrentFolder = folder;
        CurrentFolder.SetFolderActive();

        // HUD ������Ʈ
        SetMonsterCount(folder);
        Debug.Log($"Current MonsterCount: {CurrentFolderMonsterCount}");
        ui_0_HUD.UpdateHUD();

        // Ŭ���� ���ο� ���� ���� Ȯ�� �� ��Ż�� Ȱ��ȭ 
        CurrentFolder.CheckCurrentFolder(); 
        Debug.Log($"Current folder set to: {CurrentFolder.FolderName}");

        // ���� ������ �߰� ���·� ����.
        CurrentFolder.isDetectionDone = true;

        // ����� ������ ��� �߰� ���·� ����.
        foreach (Portal portal in CurrentFolder.Portals)
        {
            portal.ConnectedFolder.isDetectionDone = true;
        }
    }

    // ���� �̵�
    public void MoveToFolder(FolderNode folder)
    {
        if (folder == null) return;

        folder.SetFolderActive();
        CurrentFolder.SetFolderDeActive();

        SetCurrentFolder(folder);
    }

    

    // ���� ������ �̵�(���� ��Ż)
    public void MoveToPreviousFolder(int ParentPortalIndex, Portal preportal)
    {
        if (CurrentFolder == null || CurrentFolder.Parent == null)
        {
            Debug.Log("No previous folder available.");
            return;
        }

        FolderNode PreviousFolderNode = CurrentFolder;
        PreviousPortal = preportal;

        // �÷��̾� ��ġ ����: ���� ������ ����� ������ ��Ż ��ó�� �̵�
        Portal CurrentPortal = CurrentFolder.Left_Portal;
        FolderNode DestinationFolder = CurrentPortal.ConnectedFolder;

        if (Player != null)
        {
            Vector3 newPosition = DestinationFolder.Portals[ParentPortalIndex].transform.position;
            newPosition.x -= 0.5f;
            newPosition.y -= 0.5f;
            Player.transform.position = newPosition;
        }

        MoveToFolder(DestinationFolder);
        PreviousFolderNode.DeActivePortal();
    }

    // ���� ������ �̵� (������ ��Ż�� �̿��ϴ� ���)
    public void MoveToNextFolder(int portalIndex, Portal preportal)
    {
        Debug.Log("MoveToNextFolder");
        if (CurrentFolder == null) return;
        if (portalIndex < 0 || portalIndex >= CurrentFolder.Portals.Length)
        {
            Debug.Log("out of index");
            return;
        }

        FolderNode PreviousFolderNode = CurrentFolder;
        PreviousPortal = preportal;

        // �÷��̾� ��ġ ����: ���� ������ ���� ��Ż ��ó�� �̵�
        Portal CurrentPortal = CurrentFolder.Portals[portalIndex];
        FolderNode DestinationFolder = CurrentPortal.ConnectedFolder;

        if (CurrentPortal == null)
        {
            Debug.Log("CurrentPortal is null");
            return;
        }    
        if (CurrentPortal.ConnectedFolder == null)
        {
            Debug.Log("CurrentPortal.ConnectedFolder is null");
            return;
        }

        if (Player != null)
        {
            Vector3 newPosition;

            if (CurrentFolder.Type == FolderNode.FolderType.RandomSpecial 
                || CurrentFolder.Type == FolderNode.FolderType.Download 
                || CurrentFolder.Type == FolderNode.FolderType.Shop )
            {
                // Ư�� ��: Y�ุ 0.5 ���� ����
                newPosition = DestinationFolder.Left_Portal.transform.position;
                newPosition.y += 3.0f; // Y�� �̵�
            }
            else
            {
                // �Ϲ� ��: ���� ����
                newPosition = DestinationFolder.Left_Portal.transform.position;
                newPosition.x += 0.5f; // X�� �̵�
                newPosition.y -= 0.5f; // Y�� �̵�
            }
            Player.transform.position = newPosition;
        }

        MoveToFolder(DestinationFolder);
        PreviousFolderNode.DeActivePortal();
    }

    private void AllFolderDeActivate()
    {
        // ��� ������ ��Ȱ��ȭ
        foreach (FolderNode Folder in FindObjectsOfType<FolderNode>())
        {
            Folder.SetFolderDeActive();
        }
    }

    public void AllPortalActivate()
    {
        if (CurrentFolder.Left_Portal != null)
            CurrentFolder.Left_Portal.isActive = true;

        foreach (Portal portal in CurrentFolder.Portals)
        {
            portal.isActive = true;
        }
    }

    // ���� ���� ��Ż�� ��� �ʱ�ȭ���ִ� �Լ�
    public void ResetCurrentPortal()
    {
        if (CurrentFolder.Left_Portal != null)
            CurrentFolder.Left_Portal.isMoving = false;

        foreach (Portal portal in CurrentFolder.Portals)
        {
            portal.isMoving = false;
        }
    }
    // �̵� �� ���� �ð��� ���������� ���� ���� ��Ż�� isMoving�� false�� �ٲ���� �ϴµ�,.,,

    // �� ����� ������ ���� ������ �ҷ���
    public void SetMonsterCount(FolderNode folder)
    {
        CurrentFolderMonsterCount = folder.GetMonsterCount();
    }

    // ���� �� ����
    public void UpdateMonsterCount(int value)
    {
        CurrentFolderMonsterCount += value;
        CurrentFolder.ChangeMonsterCount();
        ui_0_HUD.UpdateHUD();
        // Debug.Log($"Current MonsterCount: {CurrentFolderMonsterCount}");
        CheckMonsterCount();
    }

    private void CheckMonsterCount()
    {
        if (CurrentFolderMonsterCount > 0) return;
        else
        {
            Debug.Log("Clear!!!");
            CurrentFolder.IsCleared = true;
            CurrentFolder.ActivePortal();
        }
    }

    public bool IsClear()
    { return CurrentFolder.IsCleared; }

}
