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
    public void MoveToFolder(FolderNode folder)
    {
        if (folder == null) return;

        folder.SetFolderActive();
        CurrentFolder.SetFolderDeActive();
        CurrentFolder = folder;

        SetMonsterCount(folder);
        ui_0_HUD.UpdateHUD();

        // Ŭ���� ���ο� ���� ���� Ȯ�� �� ��Ż�� Ȱ��ȭ 
        CurrentFolder.CheckCurrentFolder();
    }

    // ���� ������ �̵�(���� ��Ż)
    public void MoveToPreviousFolder()
    {
        if (CurrentFolder == null || CurrentFolder.Parent == null)
        {
            // Debug.Log("No previous folder available.");
            return;
        }

        MoveToFolder(CurrentFolder.Parent);

        // �÷��̾� ��ġ ����: ���� ������ ����� ������ ��Ż ��ó�� �̵�
        Portal CurrentPortal = CurrentFolder.Left_Portal;
        FolderNode DestinationFolder = CurrentPortal.ConnectedFolder;
        int DestinationFolderPortalIndex = CurrentPortal.ParentPortalIndex;

        if (Player != null)
        {
            Vector3 newPosition = DestinationFolder.Portals[DestinationFolderPortalIndex].transform.position;
            newPosition.x -= 1f;
            newPosition.y -= 1f;
            Player.transform.position = newPosition;
        }

    }

    // ���� ������ �̵� (������ ��Ż�� �̿��ϴ� ���)
    public void MoveToNextFolder(int portalIndex)
    {
        Debug.Log("MoveToNextFolder");
        if (CurrentFolder == null) return;
        if (portalIndex < 0 || portalIndex > 2) return;
     
        Portal CurrentPortal = CurrentFolder.Portals[portalIndex];
        if (CurrentPortal.ConnectedFolder == null)
        {
            Debug.Log("CurrentPortal.ConnectedFolder is null");
            return;
        }

        MoveToFolder(CurrentPortal.ConnectedFolder);

        // �÷��̾� ��ġ ����: ���� ������ ���� ��Ż ��ó�� �̵�
        FolderNode DestinationFolder = CurrentPortal.ConnectedFolder;

        if (DestinationFolder == null)
        {
            Debug.Log("DestinationFolder is null");
            return;
        }

        if (Player != null)
        {
            Vector3 newPosition = DestinationFolder.Left_Portal.transform.position;
            newPosition.x += 1f; 
            newPosition.y -= 1f; 
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
        Debug.Log($"Current MonsterCount: {CurrentFolderMonsterCount}");
        CheckMonsterCount();
    }

    private void CheckMonsterCount()
    {
        if (CurrentFolderMonsterCount > 0) return;
        else
        {
            CurrentFolder.IsCleared = true;
            CurrentFolder.ActivePortal();
        }
    }

}
