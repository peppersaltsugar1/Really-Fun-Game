using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class FolderManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    private static FolderManager instance = null;

    public GameObject Player; // 캐릭터 위치

    public List<FolderNode> AllFolders = new List<FolderNode>(); // 모든 폴더 리스트
    public FolderNode CurrentFolder; // 현재 폴더
    public FolderGenerator FolderGenerator; // 폴더 생성기
    public int CurrentFolderMonsterCount = 0;


    private UI_0_HUD ui_0_HUD; // HUD를 갱신하기 위한 참조

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

        // 맵 생성 시작
        GenerateMap();

        AllFolderDeActivate();

        SetCurrentFolder(CurrentFolder);
    }

    // 맵 생성 시작
    public void GenerateMap()
    {
        FolderGenerator.GenerateMap();
        FolderNode rootFolder = FolderGenerator.GetRootNode();

        if (rootFolder == null)
        {
            Debug.LogError("Root folder is null. Map generation failed.");
            return;
        }

        CurrentFolder = rootFolder; // 루트 폴더를 현재 폴더로 설정
        Debug.Log("Map generation completed.");
    }

    // 현재 폴더 설정
    public void SetCurrentFolder(FolderNode folder)
    {
        if (folder == null) return;

        CurrentFolder = folder;
        CurrentFolder.SetFolderActive();

        SetMonsterCount(folder);
        Debug.Log($"Current MonsterCount: {CurrentFolderMonsterCount}");
        ui_0_HUD.UpdateHUD();

        // 클리어 여부와 몬스터 수를 확인 후 포탈을 활성화 
        CurrentFolder.CheckCurrentFolder(); 
        Debug.Log($"Current folder set to: {CurrentFolder.FolderName}");
    }

    // 폴더 이동
    public void MoveToFolder(FolderNode folder, int index = 0)
    {
        if (folder == null) return;

        folder.SetFolderActive();
        CurrentFolder.SetFolderDeActive();
        CurrentFolder = folder;

        SetMonsterCount(folder);
        ui_0_HUD.UpdateHUD();

        // 클리어 여부와 몬스터 수를 확인 후 포탈을 활성화 
        // CurrentFolder.CheckCurrentFolder();
        // 디버깅용 상시 포탈 활성화
        CurrentFolder.ActivePortal();
    }

    // 상위 폴더로 이동(왼쪽 포탈)
    public void MoveToPreviousFolder(int index)
    {
        if (CurrentFolder == null || CurrentFolder.Parent == null)
        {
            // Debug.Log("No previous folder available.");
            return;
        }
        // 플레이어 위치 조정: 상위 폴더의 연결된 오른쪽 포탈 근처로 이동
        Portal CurrentPortal = CurrentFolder.Left_Portal;
        FolderNode DestinationFolder = CurrentPortal.ConnectedFolder;
        int DestinationFolderPortalIndex = CurrentPortal.ParentPortalIndex;

        // 지금 여기에ㅐ 버그가 있음
        MoveToFolder(CurrentFolder.Parent, index);

        if (Player != null)
        {
            Vector3 newPosition = DestinationFolder.Portals[DestinationFolderPortalIndex].transform.position;
            newPosition.x -= 1.5f;
            newPosition.y -= 1.5f;
            Player.transform.position = newPosition;
        }

    }

    // 하위 폴더로 이동 (오른쪽 포탈을 이용하는 경우)
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

        // 플레이어 위치 조정: 하위 폴더의 왼쪽 포탈 근처로 이동

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
        // 모든 폴더를 비활성화
        foreach (FolderNode Folder in FindObjectsOfType<FolderNode>())
        {
            Folder.SetFolderDeActive();
        }
    }

    // 맵 입장시 지정된 몬스터 개수를 불러옴
    public void SetMonsterCount(FolderNode folder)
    {
        CurrentFolderMonsterCount = folder.GetMonsterCount();
    }

    // 몬스터 수 갱신
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
