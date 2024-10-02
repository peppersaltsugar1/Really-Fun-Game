using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance = null;
    [SerializeField]
    private Player player;
    [SerializeField]
    private List<GameObject> hpPrefabsList;
    private List<GameObject> hpList = new();
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private int interval;

    // Window UI
    public GameObject WindowUI;
    public GameObject MyPC_UI;
    public GameObject DownLoad_UI;
    public GameObject My_Documents_UI;
    public GameObject LocalDisk_UI;
    public GameObject ControlOptions_UI;
    public GameObject Help_UI;
    // First Start Check
    private GameObject Start_UI;

    // Left_Button 
    public Button MyPC_Button;
    public Button DownLoad_Button;
    public Button My_Documents_Button;
    public Button LocalDisk_Button;
    public Button ControlOptions_Buttonton;
    public Button Help_Button;
    public Button Desktop_Button;

    // Top_Button 
    public Button UnderBar_Button;
    public Button X_Button;

    // Status Text
    public Text AttackText;
    public Text AttackSpeedText;
    public Text BulletVelocityText;
    public Text RangeText;
    public Text MoveSpeedText;

    // BulletManger
    private PoolingManager BInstance;

    // Program UI Member
    public GameObject Button_Program_Prefab;
    public GameObject i_Program_Detail_Image_Prefab;
    public Text t_Program_Detail_Name_Prefab;
    public Text t_Program_Detail_Explanation_Prefab;
    public Text t_Program_Detail_PowerExplanation_Prefab;

    public Transform ContentGroup;

    public Button DeleteButton;

    private int CurrentProgram = -1;

    // Setting UI Member
    public Dropdown screenModeDropdown;
    public Dropdown resolutionDropdown;
    public Dropdown qualityDropdown;

    // Status Manger
    private StatusManager statusManager;

    private int hpNum = 0;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public static UIManager Instance
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
        HpBarSet();

        player = FindObjectOfType<Player>();
        BInstance = FindObjectOfType<PoolingManager>();

        // UI Panel 비활성화 시작
        WindowUI.SetActive(false);
        UIDeactivation();
        Start_UI = null;

        // Left Button Setting
        MyPC_Button.onClick.AddListener(FMyPC_Button);
        DownLoad_Button.onClick.AddListener(FDownLoad_Button);
        My_Documents_Button.onClick.AddListener(FMy_Documents_Button);
        LocalDisk_Button.onClick.AddListener(FLocalDisk_Button);
        ControlOptions_Buttonton.onClick.AddListener(FControlOptions_Button);
        Help_Button.onClick.AddListener(FHelp_Button);
        Desktop_Button.onClick.AddListener(FDesktop_Button);

        // Top Button Setting
        UnderBar_Button.onClick.AddListener(SetWindowUI);
        X_Button.onClick.AddListener(SetWindowUI);

        // ProgramList Setting
        statusManager = StatusManager.Instance;  // StatusManager 싱글턴 참조
        GenerateButtons();

        // Delete Button Setting
        DeleteButton.onClick.AddListener(FDelete_Button);

        // ControllOptionUI Setting
        screenModeDropdown.onValueChanged.AddListener(delegate { ChangeScreenMode(screenModeDropdown.value); });
        resolutionDropdown.onValueChanged.AddListener(delegate { ChangeResolution(resolutionDropdown.value); });
        qualityDropdown.onValueChanged.AddListener(delegate { ChangeQuality(qualityDropdown.value); });
}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetWindowUI();
        }
    }

    public void HpBarSet()
    {
        Debug.Log("UI생성함");
        //hp 체력바 리셋
        if (hpList.Count > 0)
        {
            for (int i = hpList.Count - 1; i >= 0; i++)
            {
                GameObject removeHp = hpList[i];
                hpList.RemoveAt(i);
                Destroy(removeHp);
            }
            hpNum = 0;
        }
        //플레이어의 체력 상황에따라 체력바 재생성
        if (player.maxHp > 0)
        {
            //최대체력 3당 체력베터리 1개 생성후 리스트에 추가
            for (int i = 0; i < player.maxHp / 3; i++)
            {
                GameObject newHp = Instantiate(hpPrefabsList[0], canvas.transform);
                newHp.transform.SetParent(canvas.transform, false);
                RectTransform rectTransform = newHp.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(i * interval, 0); // 위치 조정 (임의로 설정)
                hpList.Add(newHp);
                hpNum += 1;
            }
            //임시체력 3당 임시체력베터리 1개 생성후 리스트에 추가
            if (player.temHp > 0)
            {
                for (int i = 0; i < player.temHp / 3; i++)
                {
                    GameObject newTemHp = Instantiate(hpPrefabsList[1], canvas.transform);
                    newTemHp.transform.SetParent(canvas.transform, false);
                    RectTransform rectTransform = newTemHp.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = new Vector2(hpNum * interval, 0); // 위치 조정 (임의로 설정)
                    hpList.Add(newTemHp);
                    hpNum += 1;

                }
            }
            //전기1 전기베터리 1개 생성후 리스트에 추가

            if (player.elect > 0)
            {
                for (int i = 0; i < player.elect; i++)
                {
                    GameObject spark = Instantiate(hpPrefabsList[2], canvas.transform);
                    spark.transform.SetParent(canvas.transform, false);
                    RectTransform rectTransform = spark.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = new Vector2(hpNum * interval, 0); // 위치 조정 (임의로 설정)
                    hpList.Add(spark);
                    hpNum += 1;

                }
            }
            //쉴드체력1당 체력베터리 1개 생성후 리스트에 추가
            if (player.shieldHp > 0)
            {
                for (int i = 0; i < player.shieldHp; i++)
                {
                    GameObject newShildHp = Instantiate(hpPrefabsList[3], canvas.transform);
                    newShildHp.transform.SetParent(canvas.transform, false);
                    RectTransform rectTransform = newShildHp.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = new Vector2(hpNum * interval, 0); // 위치 조정 (임의로 설정)
                    hpList.Add(newShildHp);
                    hpNum += 1;

                }
            }
        }
    }
    public void HpBarPlus()
    {

    }
    public void ShiledSet()
    {
        //쉴드체력이 소모될때 쉴드체력을 삭제
        for (int i = hpNum - 1; i >= 0; i--)
        {
            Debug.Log("여기들어옴");
            if (hpList[i].name == "Shield_Heart(Clone)")
            {
                GameObject removeHp = hpList[i];
                hpList.RemoveAt(i);
                Destroy(removeHp);
                hpNum--;
                ShiledOn();
                return;
            }
        }
    }
    private void ShiledOn()
    {
        //Hp체력바의 쉴드를 활성화
        for (int i = 0; i < hpNum; i++)
        {
            if (hpList[i].name == "R_Heart(Clone)")
            {
                // 첫 번째 자식을 가져와 활성화
                if (hpList[i].transform.childCount > 0)
                {
                    hpList[i].transform.GetChild(0).gameObject.SetActive(true);
                }
            }
        }
    }

    public void ShiledOff()
    {
        //hp체력바의 쉴드를 비활성화
        for (int i = hpNum - 1; i >= player.shield; i--)
        {
            if (hpList[i].name == "R_Heart(Clone)" && hpList[i].transform.GetChild(0).gameObject.activeSelf)
            {
                hpList[i].transform.GetChild(0).gameObject.SetActive(false);
                return;
            }
        }
    }
    public void HpSet()
    {
        if (player.currentHp <= 3)
        {
            switch (player.currentHp)
            {
                case 1:
                    hpList[0].transform.GetChild(1).gameObject.SetActive(true);
                    hpList[0].transform.GetChild(2).gameObject.SetActive(false);
                    hpList[0].transform.GetChild(3).gameObject.SetActive(false);
                    return;
                case 2:
                    hpList[0].transform.GetChild(1).gameObject.SetActive(true);
                    hpList[0].transform.GetChild(2).gameObject.SetActive(true);
                    hpList[0].transform.GetChild(3).gameObject.SetActive(false);
                    return;
                case 3:
                    hpList[0].transform.GetChild(1).gameObject.SetActive(true);
                    hpList[1].transform.GetChild(1).gameObject.SetActive(false);
                    hpList[0].transform.GetChild(2).gameObject.SetActive(true);
                    hpList[0].transform.GetChild(3).gameObject.SetActive(true);
                    return;
            }
        }
        switch (player.currentHp % 3)
        {
            case 0:
                for (int i = 0; i < player.maxHp / 3; i++)
                {
                    hpList[i].transform.GetChild(1).gameObject.SetActive(false);
                    hpList[i].transform.GetChild(2).gameObject.SetActive(false);
                    hpList[i].transform.GetChild(3).gameObject.SetActive(false);
                }
                for (int i = 0; i < player.currentHp / 3; i++)
                {
                    hpList[i].transform.GetChild(1).gameObject.SetActive(true);
                    hpList[i].transform.GetChild(2).gameObject.SetActive(true);
                    hpList[i].transform.GetChild(3).gameObject.SetActive(true);
                }
                break;
            case 1:
                hpList[((int)player.currentHp / 3)].transform.GetChild(1).gameObject.SetActive(true);
                hpList[((int)player.currentHp / 3)].transform.GetChild(2).gameObject.SetActive(false);
                hpList[((int)player.currentHp / 3)].transform.GetChild(3).gameObject.SetActive(false);

                break;
            case 2:
                hpList[((int)player.currentHp / 3)].transform.GetChild(1).gameObject.SetActive(true);
                hpList[((int)player.currentHp / 3)].transform.GetChild(2).gameObject.SetActive(true);
                hpList[((int)player.currentHp / 3)].transform.GetChild(3).gameObject.SetActive(false);

                break;
        }
    }

    public void TemHpSet()
    {
        //임시체력 관리 
        for (int i = hpNum - 1; i >= 0; i--)
        {
            if (hpList[i].name == "RDis_Heart(Clone)")
            {
                switch (player.temHp % 3)
                {
                    case 0:
                        hpList[i].transform.GetChild(0).gameObject.SetActive(true);
                        hpList[i].transform.GetChild(1).gameObject.SetActive(true);
                        hpList[i].transform.GetChild(2).gameObject.SetActive(true);
                        break;

                    case 1:
                        hpList[i].transform.GetChild(0).gameObject.SetActive(true);
                        hpList[i].transform.GetChild(1).gameObject.SetActive(false);
                        hpList[i].transform.GetChild(2).gameObject.SetActive(false);
                        break;

                    case 2:
                        hpList[i].transform.GetChild(0).gameObject.SetActive(true);
                        hpList[i].transform.GetChild(1).gameObject.SetActive(true);
                        hpList[i].transform.GetChild(2).gameObject.SetActive(false);
                        break;
                }
                return;
            }
        }
    }
    public void TemHpDel()
    {
        for (int i = hpNum - 1; i >= 0; i--)
        {
            if (hpList[i].name == "RDis_Heart(Clone)")
            {
                GameObject removeHp = hpList[i];
                hpList.RemoveAt(i);
                Destroy(removeHp);
                hpNum--;
                return;
            }
        }
    }
    public void HpDel()
    {
        for (int i = hpNum - 1; i >= 0; i--)
        {
            if (hpList[i].name == "R_Heart(Clone)")
            {
                GameObject removeHp = hpList[i];
                hpList.RemoveAt(i);
                Destroy(removeHp);
                hpNum--;
                return;
            }
        }
    }

    public void ElectDel()
    {
        for (int i = hpNum - 1; i >= 0; i--)
        {
            if (hpList[i].name == "Elect_Heart(Clone)")
            {
                GameObject removeHp = hpList[i];
                hpList.RemoveAt(i);
                Destroy(removeHp);
                hpNum--;
                return;
            }
        }
    }

    // ========== UI Section ==========

    // Input ESC -> Show UI 
    public void SetWindowUI()
    {
        if (WindowUI != null)
        {
            bool isActive = WindowUI.activeSelf;
            if (isActive)
            {
                WindowUI.SetActive(false);
                Time.timeScale = 1;
            }
            else
            {
                // 상태 최신화
                UpdateStats();
                GenerateButtons();

                // UI를 활성화하고 게임을 일시 정지
                if (Start_UI == null)
                {
                    Start_UI = MyPC_UI;
                    Start_UI.SetActive(true);
                }
                WindowUI.SetActive(true);
                Time.timeScale = 0;
            }      
        }
    }

    // Status Update Function
    private void UpdateStats()
    {
        if (player != null)
        {
            AttackText.text = player.atk.ToString();
            AttackSpeedText.text = player.atkSpeed.ToString();
            BulletVelocityText.text = BInstance.bulletPool.Peek().speed.ToString();
            RangeText.text = player.angleRange.ToString();
            MoveSpeedText.text = player.moveSpeed.ToString();
        }
    }

    // Program Update Funtion
    public void GenerateButtons()
    {
        // 자손 제거
        foreach (Transform child in ContentGroup)
        {
            Destroy(child.gameObject);
        }
        
        for (int i = 0; i < statusManager.ProgramList.Count; i++)
        {
            GameObject newButton = Instantiate(Button_Program_Prefab, ContentGroup);

            PInformation programInfo = statusManager.ProgramList[i];
            Image buttonImage = newButton.GetComponent<Image>();

            if (buttonImage != null)
            {
                SetSpriteFromSheet(buttonImage, programInfo.spriteSheetName, programInfo.spriteIndex);
            }
            else
            {
                Debug.LogError("Error");
            }

            int index = i;
            newButton.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(newButton));
        }

        // Delete Button Activation
        if (statusManager.ProgramList.Count == 0)
            DeleteButton.gameObject.SetActive(false);
    }

    public void SetSpriteFromSheet(Image buttonImage, string spriteSheetName, int spriteIndex)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(spriteSheetName);

        if (sprites != null && spriteIndex >= 0 && spriteIndex < sprites.Length)
        {
            buttonImage.sprite = sprites[spriteIndex];
            Debug.Log("Sprite loaded: " + sprites[spriteIndex].name);

        }
        else
        {
            Debug.LogError("Sprite not found or invalid index for spriteSheetName: " + spriteSheetName);
        }
    }


    public void OpenProgramDetail(int index)
    {
        CurrentProgram = index;
        // Detail Setting
        t_Program_Detail_Name_Prefab.text = statusManager.ProgramList[index].ProgramName;
        t_Program_Detail_Explanation_Prefab.text = statusManager.ProgramList[index].Explanation;
        t_Program_Detail_PowerExplanation_Prefab.text = statusManager.ProgramList[index].PowerExplanation;

        // Image Setting
        Image detailImage = i_Program_Detail_Image_Prefab.GetComponent<Image>();

        if (detailImage != null)
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>(statusManager.ProgramList[index].spriteSheetName);

            if (sprites != null && statusManager.ProgramList[index].spriteIndex >= 0 && statusManager.ProgramList[index].spriteIndex < sprites.Length)
            {
                detailImage.sprite = sprites[statusManager.ProgramList[index].spriteIndex];
                Debug.Log("Detail Image sprite set: " + sprites[statusManager.ProgramList[index].spriteIndex].name);
            }
            else
            {
                Debug.LogError("Sprite not found or invalid index for spriteSheetName: " + statusManager.ProgramList[index].spriteSheetName);
            }
        }
        else
        {
            Debug.LogError("i_Program_Detail_Image_Prefab does not have an Image component.");
        }

        DeleteButton.gameObject.SetActive(true);

        Debug.Log("OpenProgramDetail");
    }

    public void FDelete_Button()
    {
        if(CurrentProgram != -1)
        { 
            statusManager.RemoveProgram(CurrentProgram);
            CurrentProgram = -1;

            // i_Program_Detail_Image_Prefab
            t_Program_Detail_Name_Prefab.text = "";
            t_Program_Detail_Explanation_Prefab.text = "";
            t_Program_Detail_PowerExplanation_Prefab.text = "";

            DeleteButton.gameObject.SetActive(false);

            GenerateButtons();
        }

        Image detailImage = i_Program_Detail_Image_Prefab.GetComponent<Image>();

        if (detailImage != null)
        {
            detailImage.sprite = null;
        }
        else
        {
            Debug.LogError("Image component not found");
        }
    }

    // UI Deactivation
    public void UIDeactivation()
    {
        MyPC_UI.SetActive(false);
        DownLoad_UI.SetActive(false);
        My_Documents_UI.SetActive(false);
        LocalDisk_UI.SetActive(false);
        ControlOptions_UI.SetActive(false);
        Help_UI.SetActive(false);
    }

    // Button OnClickFuction
    public void FMyPC_Button()
    {
        UIDeactivation();
        MyPC_UI.SetActive(true);
    }

    public void FDownLoad_Button()
    {
        UIDeactivation();
        DownLoad_UI.SetActive(true);
    }

    public void FMy_Documents_Button()
    {
        UIDeactivation();
        My_Documents_UI.SetActive(true);
    }

    public void FLocalDisk_Button()
    {
        UIDeactivation();
        LocalDisk_UI.SetActive(true);
    }

    public void FControlOptions_Button()
    {
        UIDeactivation();
        ControlOptions_UI.SetActive(true);
    }

    public void FHelp_Button()
    {
        UIDeactivation();
        Help_UI.SetActive(true);
    }

    public void FDesktop_Button()
    {
        // PC 종료
        Application.Quit();

        // 에디터에서 종료
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    void OnButtonClick(GameObject clickedButton)
    {

        int index = clickedButton.transform.GetSiblingIndex();
        OpenProgramDetail(index);
    }

    // Setting Function

    // ScreenMode
    public void ChangeScreenMode(int index)
    {
        switch (index)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                Debug.Log("전체화면 모드");
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                Debug.Log("창모드");
                break;
            default:
                Debug.LogWarning("Error");
                break;
        }
    }
    
    // Resolution
    public void ChangeResolution(int index)
    {
        switch (index)
        {
            case 0: // 1920 x 1080
                Screen.SetResolution(1920, 1080, Screen.fullScreen);
                break;
            case 1: // 1600 x 900
                Screen.SetResolution(1600, 900, Screen.fullScreen);
                break;
            case 2: // 1280 x 720
                Screen.SetResolution(1280, 720, Screen.fullScreen);
                break;
            default:
                break;
        }
    }

    public void ChangeQuality(int index)
    {
        switch (index)
        {
            case 0: // 좋음
                QualitySettings.SetQualityLevel(5, true); 
                break;
            case 1: // 중간
                QualitySettings.SetQualityLevel(3, true);
                break;
            case 2: // 낮음
                QualitySettings.SetQualityLevel(1, true);
                break;
            default:
                break;
        }
    }
}
