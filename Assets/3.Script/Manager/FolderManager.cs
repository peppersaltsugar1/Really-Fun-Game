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


    private UI_0_HUD ui_0_HUD; // HUD�� �����ϱ� ���� ����

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

        // �� ���� ����
        GenerateMap();

        AllFolderDeActivate();

        SetCurrentFolder(CurrentFolder);
    }

    // �� ���� ����
    public void GenerateMap()
    {
        FolderGenerator.GenerateMap();
        FolderNode rootFolder = FolderGenerator.GetRootNode();

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

        SetMonsterCount(folder);
        Debug.Log($"Current MonsterCount: {CurrentFolderMonsterCount}");
        ui_0_HUD.UpdateHUD();

        // Ŭ���� ���ο� ���� ���� Ȯ�� �� ��Ż�� Ȱ��ȭ 
        CurrentFolder.CheckCurrentFolder(); 
        Debug.Log($"Current folder set to: {CurrentFolder.FolderName}");
    }

    // ���� �̵�
    public void MoveToFolder(FolderNode folder, int index = 0)
    {
        if (folder == null) return;

        folder.SetFolderActive();
        CurrentFolder.SetFolderDeActive();
        CurrentFolder = folder;

        SetMonsterCount(folder);
        ui_0_HUD.UpdateHUD();

        // Ŭ���� ���ο� ���� ���� Ȯ�� �� ��Ż�� Ȱ��ȭ 
        // CurrentFolder.CheckCurrentFolder();
        // ������ ��� ��Ż Ȱ��ȭ
        CurrentFolder.ActivePortal();
    }

    // ���� ������ �̵�(���� ��Ż)
    public void MoveToPreviousFolder(int index)
    {
        if (CurrentFolder == null || CurrentFolder.Parent == null)
        {
            // Debug.Log("No previous folder available.");
            return;
        }
        // �÷��̾� ��ġ ����: ���� ������ ����� ������ ��Ż ��ó�� �̵�
        Portal CurrentPortal = CurrentFolder.Left_Portal;
        FolderNode DestinationFolder = CurrentPortal.ConnectedFolder;
        int DestinationFolderPortalIndex = CurrentPortal.ParentPortalIndex;

        // ���� ���⿡�� ���װ� ����
        MoveToFolder(CurrentFolder.Parent, index);

        if (Player != null)
        {
            Vector3 newPosition = DestinationFolder.Portals[DestinationFolderPortalIndex].transform.position;
            newPosition.x -= 1.5f;
            newPosition.y -= 1.5f;
            Player.transform.position = newPosition;
        }

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

        MoveToFolder(CurrentPortal.ConnectedFolder);

        // �÷��̾� ��ġ ����: ���� ������ ���� ��Ż ��ó�� �̵�

        if (Player != null)
        {
            Vector3 newPosition = CurrentFolder.Left_Portal.transform.position;
            newPosition.x += 1.5f; 
            newPosition.y -= 1.5f; 
            Player.transform.position = newPosition;
        }

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

}
