using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class FolderManager : MonoBehaviour
{
    #region Manager
    // �̱��� �ν��Ͻ�
    private static FolderManager instance = null;

    private UI_0_HUD ui_0_HUD; // HUD�� �����ϱ� ���� ����
    private UI_4_LocalDisk localDiskUI;

    #endregion

    #region Definition

    public GameObject Player; // ĳ���� ��ġ
    public List<FolderNode> AllFolders = new List<FolderNode>(); // ��� ���� ����Ʈ
    public List<GameObject> specialFolderList = new List<GameObject>(); // ����� ���� ����Ʈ
    public FolderNode previousFolder; // ���� ����
    public FolderNode CurrentFolder; // ���� ����
    public FolderGenerator FolderGenerator; // ���� ������
    public int CurrentFolderMonsterCount = 0;
    public FolderNode rootFolder;

    [Header("Portal")]
    public Portal PreviousPortal = null;

    // Camera
    [SerializeField] private CinemachineVirtualCamera camera;
    CameraManager cameraManager;
    #endregion

    #region Base Function

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
        cameraManager = CameraManager.Instance;
        // �� ���� ����
        GenerateMap();

        AllFolderDeActivate();

        SetCurrentFolder(CurrentFolder);

        if (localDiskUI != null && rootFolder != null)
        {
            localDiskUI.GenerateTreeUI(FolderGenerator.GetRootNode());
        }
    }

    #endregion

    // �� ���� ����
    public void GenerateMap()
    {
        FolderGenerator.GenerateMap();
        rootFolder = FolderGenerator.GetRootNode();
        specialFolderList = FolderGenerator.hiddenFolderList;
         
        if (rootFolder == null)
        {
            Debug.LogError("Root folder is null. Map generation failed.");
            return;
        }

        CurrentFolder = rootFolder; // ��Ʈ ������ ���� ������ ����
    }

    #region Current Folder Setting, Getting Info

    // ���� ���� ����
    public void SetCurrentFolder(FolderNode folder)
    {
        if (folder == null) return;

        CurrentFolder = folder;
        CurrentFolder.SetFolderActive();

        // HUD ������Ʈ
        SetMonsterCount(folder);
        // Debug.Log($"Current MonsterCount: {CurrentFolderMonsterCount}");
        ui_0_HUD.UpdateHUD();

        // Ŭ���� ���ο� ���� ���� Ȯ�� �� ��Ż�� Ȱ��ȭ 
        CurrentFolder.DeActivePortal();
        CurrentFolder.CheckCurrentFolder(); 
        // Debug.Log($"Current folder set to: {CurrentFolder.FolderName}");

        // ���� ������ �߰� ���·� ����.
        CurrentFolder.isDetectionDone = true;

        // ����� ������ ��� �߰� ���·� ����.
        if (CurrentFolder == null)
        {
            Debug.LogError("CurrentFolder is null");
            return;
        }

        if (CurrentFolder.Portals == null)
        {
            Debug.LogError("CurrentFolder.Portals is null");
            return;
        }

        foreach (Portal portal in CurrentFolder.Portals)
        {
            if (portal == null)
            {
                Debug.LogWarning("Portal is null.");
                continue;
            }

            if (portal.ConnectedFolder == null)
            {
                Debug.LogWarning($"Portal {portal.name} does not have a connected folder.");
                continue;
            }

            portal.ConnectedFolder.isDetectionDone = true;
        }
    }

    public bool IsClear(){ return CurrentFolder.IsCleared; }

    #endregion

    #region Folder Move

    // ���� �̵�
    public void MoveToFolder(FolderNode folder)
    {
        Debug.Log("MoveToFolder");
        if (folder == null) return;

        folder.SetFolderActive();
        CurrentFolder.SetFolderDeActive();

        SetCurrentFolder(folder);

        if(folder.Type == FolderNode.FolderType.RandomSpecial)
        {
            // camera.m_Lens.OrthographicSize = 8.0f;
            cameraManager.switcherCamera(folder.mapCamera);
        }
        else
        {
            // camera.m_Lens.OrthographicSize = 5.77f;
            cameraManager.switchPrimaryCamera();
        }

        cameraManager.SetCollider();
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
        // Debug.Log("MoveToNextFolder");
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
        DestinationFolder.Left_Portal.DelayisMovingFalse();
    }

    public void MoveHiddenFolder(string Name)
    {
        Debug.Log("MoveHiddenFolder");

        foreach (var cur in specialFolderList)
        {
            FolderNode folder = cur.GetComponent<FolderNode>();
            if (folder.FolderName == Name)
            {
                Debug.Log("Find");
                previousFolder = CurrentFolder;
                folder.Left_Portal.ConnectedFolder = previousFolder;
                Player.transform.position = folder.transform.Find("TeleportPoint").position;
                MoveToFolder(folder);
                break;   
            }
        }
    }

    public void MoveHiddenToPre(FolderNode folder)
    {
        Debug.Log("MoveHiddenToPre");

        CurrentFolder.DeActivePortal();

        Player.transform.position = folder.transform.Find("TeleportPoint").position;
        MoveToFolder(folder);
    }

    #endregion

    #region Activation Folder, Portal

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

    #endregion

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
    
    #region Monster

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

    #endregion


}
