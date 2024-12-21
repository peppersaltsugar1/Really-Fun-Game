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

        // ���� ���� ��Ż�� ��� ��Ȱ��ȭ
        // DeAcitivateAllPortals();

        // Ŭ���� ���ο� ���� ���� Ȯ�� �� ��Ż�� Ȱ��ȭ 
        // CurrentFolder.CheckCurrentFolder();
        // ������ ��� ��Ż Ȱ��ȭ
        // CurrentFolder.ActivePortal();
    }

    private void DeAcitivateAllPortals()
    {
        if(CurrentFolder.Left_Portal != null)
            CurrentFolder.Left_Portal.DeActivatePortal();
        
        foreach(Portal portal in CurrentFolder.Portals)
        {
            portal.DeActivatePortal();
        }
    }

    // ���� ������ �̵�(���� ��Ż)
    public void MoveToPreviousFolder(int ParentPortalIndex)
    {
        if (CurrentFolder == null || CurrentFolder.Parent == null)
        {
            // Debug.Log("No previous folder available.");
            return;
        }
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
    }

    // ���� ������ �̵� (������ ��Ż�� �̿��ϴ� ���)
    public void MoveToNextFolder(int portalIndex)
    {
        Debug.Log("MoveToNextFolder");
        if (CurrentFolder == null) return;
        if (portalIndex < 0 || portalIndex >= CurrentFolder.Portals.Length)
        {
            Debug.Log("out of index");
            return;
        }
        Portal CurrentPortal = CurrentFolder.Portals[portalIndex];
        if(CurrentPortal == null)
        {
            Debug.Log("CurrentPortal is null");
            return;
        }    
        if (CurrentPortal.ConnectedFolder == null)
        {
            Debug.Log("CurrentPortal.ConnectedFolder is null");
            return;
        }

        FolderNode PreviousFolderNode = CurrentFolder;
        MoveToFolder(CurrentPortal.ConnectedFolder);

        // �÷��̾� ��ġ ����: ���� ������ ���� ��Ż ��ó�� �̵�
        if (Player != null)
        {
            Vector3 newPosition = CurrentFolder.Left_Portal.transform.position;
            newPosition.x += 0.5f;
            newPosition.y -= 0.5f;
            Player.transform.position = newPosition;
        }

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
