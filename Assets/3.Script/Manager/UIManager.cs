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
    [SerializeField]
    GameObject localDiskContent;
    [SerializeField]
    MapGenerator mapGenerator;
    //��Ż Ui����
    [SerializeField]
    GameObject UIPortal;
    [SerializeField]
    List<Sprite> portalUiList = new();
    [SerializeField]
    List<Sprite> closePortalUiList = new();
    [SerializeField]
    List<GameObject> itemImageList = new();

    //�ּҰ���
    public List<Map> addressList = new();
    [SerializeField]
    GameObject address;

    // Basic UI
    public GameObject FirstStartUI;
    public Button StartButton;
    public GameObject Start_Back;
    public GameObject Start_Line;

    public GameObject DeathUI;
    public Button ReStartButton;
    public Button GoToDesktop;
    public Text PlayTimeText;
    public Text DeathSign;
    public bool HPUIActive;

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

    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;

    public Button MasterButton;
    public GameObject MasterVolumeBaseImage;
    public GameObject MasterVolumeMuteImage;

    public Button BGMButton;
    public GameObject BGMVolumeBaseImage;
    public GameObject BGMVolumeMuteImage;

    public Button SFXButton;
    public GameObject SFXVolumeBaseImage;
    public GameObject SFXVolumeMuteImage;

    // Program Manger
    private ProgramManager programManager;

    // Status Manager
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
        player = FindObjectOfType<Player>();
        BInstance = FindObjectOfType<PoolingManager>();
        statusManager = StatusManager.Instance;
        HPUIActive = true;

        // HpBarSet();

        // UI Panel ��Ȱ��ȭ ����
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
        programManager = ProgramManager.Instance;  // ProgramManager �̱��� ����
        GenerateButtons();

        // Delete Button Setting
        DeleteButton.onClick.AddListener(FDelete_Button);

        // ControllOptionUI Setting
        screenModeDropdown.onValueChanged.AddListener(delegate { ChangeScreenMode(screenModeDropdown.value); });
        resolutionDropdown.onValueChanged.AddListener(delegate { ChangeResolution(resolutionDropdown.value); });
        qualityDropdown.onValueChanged.AddListener(delegate { ChangeQuality(qualityDropdown.value); });

        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        MasterButton.onClick.AddListener(FMasterButton);
        MasterVolumeMuteImage.SetActive(false);

        BGMButton.onClick.AddListener(FBGMButton);
        BGMVolumeMuteImage.SetActive(false);

        SFXButton.onClick.AddListener(FSFXButton);
        SFXVolumeMuteImage.SetActive(false);

        
        Text addressText = address.GetComponent<Text>();
        addressText.text = "�� PC";

        // Basic UI Setting
        FirstStartFunc();
        StartButton.onClick.AddListener(FStartButton);
        ReStartButton.onClick.AddListener(FReStartButton);
        GoToDesktop.onClick.AddListener(FDesktop_Button);
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
        //hp ü�¹� ����
        if (hpList.Count > 0)
        {
            for (int i = hpList.Count - 1; i >= 0; i--)
            {
                GameObject removeHp = hpList[i];
                hpList.RemoveAt(i);
                Destroy(removeHp);
            }
            hpNum = 0;
        }

        if (hpPrefabsList.Count < 4)
        {
            Debug.LogError("hpPrefabsList�� �ʿ��� �������� �����մϴ�.");
            return;
        }

        //�÷��̾��� ü�� ��Ȳ������ ü�¹� �����
        if (statusManager.MaxHp > 0)
        {
            //�ִ�ü�� 3�� ü�º��͸� 1�� ������ ����Ʈ�� �߰�
            for (int i = 0; i < statusManager.MaxHp / 3; i++)
            {
                GameObject newHp = Instantiate(hpPrefabsList[0], canvas.transform);
                newHp.transform.SetParent(canvas.transform, false);
                RectTransform rectTransform = newHp.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(i * interval, 0); // ��ġ ���� (���Ƿ� ����)
                hpList.Add(newHp);
                hpNum += 1;
            }
            //�ӽ�ü�� 3�� �ӽ�ü�º��͸� 1�� ������ ����Ʈ�� �߰�
            if (statusManager.TemHp > 0)
            {
                for (int i = 0; i < statusManager.TemHp / 3; i++)
                {
                    GameObject newTemHp = Instantiate(hpPrefabsList[1], canvas.transform);
                    newTemHp.transform.SetParent(canvas.transform, false);
                    RectTransform rectTransform = newTemHp.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = new Vector2(hpNum * interval, 0); // ��ġ ���� (���Ƿ� ����)
                    hpList.Add(newTemHp);
                    hpNum += 1;

                }
            }
            //����1 ���⺣�͸� 1�� ������ ����Ʈ�� �߰�

            if (statusManager.Elect > 0)
            {
                for (int i = 0; i < statusManager.Elect; i++)
                {
                    GameObject spark = Instantiate(hpPrefabsList[2], canvas.transform);
                    spark.transform.SetParent(canvas.transform, false);
                    RectTransform rectTransform = spark.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = new Vector2(hpNum * interval, 0); // ��ġ ���� (���Ƿ� ����)
                    hpList.Add(spark);
                    hpNum += 1;

                }
            }
            //����ü��1�� ü�º��͸� 1�� ������ ����Ʈ�� �߰�
            if (statusManager.ShieldHp > 0)
            {
                for (int i = 0; i < statusManager.ShieldHp; i++)
                {
                    GameObject newShildHp = Instantiate(hpPrefabsList[3], canvas.transform);
                    newShildHp.transform.SetParent(canvas.transform, false);
                    RectTransform rectTransform = newShildHp.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = new Vector2(hpNum * interval, 0); // ��ġ ���� (���Ƿ� ����)
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
        //����ü���� �Ҹ�ɶ� ����ü���� ����
        for (int i = hpNum - 1; i >= 0; i--)
        {
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
        //Hpü�¹��� ���带 Ȱ��ȭ
        for (int i = 0; i < hpNum; i++)
        {
            if (hpList[i].name == "R_Heart(Clone)")
            {
                // ù ��° �ڽ��� ������ Ȱ��ȭ
                if (hpList[i].transform.childCount > 0)
                {
                    hpList[i].transform.GetChild(0).gameObject.SetActive(true);
                }
            }
        }
    }

    public void ShiledOff()
    {
        //hpü�¹��� ���带 ��Ȱ��ȭ
        for (int i = hpNum - 1; i >= statusManager.Shield; i--)
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
        if (statusManager.CurrentHp <= 3)
        {
            switch (statusManager.CurrentHp)
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
        switch (statusManager.CurrentHp % 3)
        {
            case 0:
                for (int i = 0; i < statusManager.MaxHp / 3; i++)
                {
                    hpList[i].transform.GetChild(1).gameObject.SetActive(false);
                    hpList[i].transform.GetChild(2).gameObject.SetActive(false);
                    hpList[i].transform.GetChild(3).gameObject.SetActive(false);
                }
                for (int i = 0; i < statusManager.CurrentHp / 3; i++)
                {
                    hpList[i].transform.GetChild(1).gameObject.SetActive(true);
                    hpList[i].transform.GetChild(2).gameObject.SetActive(true);
                    hpList[i].transform.GetChild(3).gameObject.SetActive(true);
                }
                break;
            case 1:
                hpList[((int)statusManager.CurrentHp / 3)].transform.GetChild(1).gameObject.SetActive(true);
                hpList[((int)statusManager.CurrentHp / 3)].transform.GetChild(2).gameObject.SetActive(false);
                hpList[((int)statusManager.CurrentHp / 3)].transform.GetChild(3).gameObject.SetActive(false);

                break;
            case 2:
                hpList[((int)statusManager.CurrentHp / 3)].transform.GetChild(1).gameObject.SetActive(true);
                hpList[((int)statusManager.CurrentHp / 3)].transform.GetChild(2).gameObject.SetActive(true);
                hpList[((int)statusManager.CurrentHp / 3)].transform.GetChild(3).gameObject.SetActive(false);

                break;
        }
    }

    public void TemHpSet()
    {
        //�ӽ�ü�� ���� 
        for (int i = hpNum - 1; i >= 0; i--)
        {
            if (hpList[i].name == "RDis_Heart(Clone)")
            {
                switch (statusManager.TemHp % 3)
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
    public void FirstStartFunc()
    {
        if (FirstStartUI != null)
        {
            FirstStartUI.SetActive(true);

            Start_Back.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
            Start_Line.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
            Start_Back.GetComponent<Animator>().SetTrigger("Start");
            Start_Line.GetComponent<Animator>().SetTrigger("Start");

            Time.timeScale = 0;
        }
        else
        {
            Debug.Log("FirstStartUI is not allocated");
        }
    }

    public void FStartButton()
    {
        if (FirstStartUI != null)
        {
            // �Ҹ� ���
            SoundManager.Instance.StartButtonSound();
            StartCoroutine(DelayUIAndGameStart(3.0f));
        }
        else
        {
            Debug.Log("FirstStartUI is not allocated");
        }
    }

    private IEnumerator DelayUIAndGameStart(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        FirstStartUI.SetActive(false);
        Time.timeScale = 1;
        HpBarSet();
    }

    public void PlayerIsDead()
    {
        Time.timeScale = 0;
        StartCoroutine(DelayUIAndGameOver(2.0f));
    }

    private IEnumerator DelayUIAndGameOver(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        float playTime = Time.time - GameManager.Instance.StartTime;
        int hours = Mathf.FloorToInt(playTime / 3600);
        int minutes = Mathf.FloorToInt((playTime % 3600) / 60);
        int seconds = Mathf.FloorToInt(playTime % 60);
        PlayTimeText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);

        MonsterBase.MonsterType monsterType = statusManager.DeathSign;
        if (MonsterBase.MonsterNameDict.TryGetValue(monsterType, out string monsterName))
        {
            DeathSign.text = monsterName + "���� ����.";
        }
        else
        {
            DeathSign.text = "�� �� ���� �������� ����.";
        }


        DeathUI.SetActive(true);
        HPUIActiveSetting();
    }

    public void HPUIActiveSetting()
    {
        if (HPUIActive)
        {
            foreach (GameObject HPList in hpList)
            {
                HPList.SetActive(false);
            }

            HPUIActive = false;
        }
        else
        {
            foreach (GameObject HPList in hpList)
            {
                HPList.SetActive(true);
            }

            HPUIActive = true;
        }
    }

    public void FReStartButton()
    {
        DeathUI.SetActive(false);
        Time.timeScale = 1;
        statusManager.InitializeStatus();
        HPUIActiveSetting();

        HpBarSet();
        GameManager.Instance.ReStartGame();
        GameManager.Instance.ResetPlayTime();
    }



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
                // ���� �ֽ�ȭ
                UpdateStats();
                GenerateButtons();

                // UI�� Ȱ��ȭ�ϰ� ������ �Ͻ� ����
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
            AttackText.text = statusManager.AttackPower.ToString();
            AttackSpeedText.text = statusManager.AttackSpeed.ToString();
            BulletVelocityText.text = BInstance.bulletPool.Peek().speed.ToString();
            RangeText.text = statusManager.AngleRange.ToString();
            MoveSpeedText.text = statusManager.MoveSpeed.ToString();
        }
    }

    // Program Update Funtion
    public void GenerateButtons()
    {
        // �ڼ� ����
        foreach (Transform child in ContentGroup)
        {
            Destroy(child.gameObject);
        }
        
        for (int i = 0; i < programManager.ProgramList.Count; i++)
        {
            GameObject newButton = Instantiate(Button_Program_Prefab, ContentGroup);

            PInformation programInfo = programManager.ProgramList[i];
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
        if (programManager.ProgramList.Count == 0)
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
        t_Program_Detail_Name_Prefab.text = programManager.ProgramList[index].ProgramName;
        t_Program_Detail_Explanation_Prefab.text = programManager.ProgramList[index].Explanation;
        t_Program_Detail_PowerExplanation_Prefab.text = programManager.ProgramList[index].PowerExplanation;

        // Image Setting
        Image detailImage = i_Program_Detail_Image_Prefab.GetComponent<Image>();

        if (detailImage != null)
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>(programManager.ProgramList[index].spriteSheetName);

            if (sprites != null && programManager.ProgramList[index].spriteIndex >= 0 && programManager.ProgramList[index].spriteIndex < sprites.Length)
            {
                detailImage.sprite = sprites[programManager.ProgramList[index].spriteIndex];
                Debug.Log("Detail Image sprite set: " + sprites[programManager.ProgramList[index].spriteIndex].name);
            }
            else
            {
                Debug.LogError("Sprite not found or invalid index for spriteSheetName: " + programManager.ProgramList[index].spriteSheetName);
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
            programManager.RemoveProgram(CurrentProgram);
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
        Text addressText = address.GetComponent<Text>();
        addressText.text = "�� PC";
    }

    public void FDownLoad_Button()
    {
        UIDeactivation();
        DownLoad_UI.SetActive(true);
        Text addressText = address.GetComponent<Text>();
        addressText.text = "�ٿ�ε�";
    }

    public void FMy_Documents_Button()
    {
        UIDeactivation();
        My_Documents_UI.SetActive(true);
        Text addressText = address.GetComponent<Text>();
        addressText.text = "�� ����";
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
        Text addressText = address.GetComponent<Text>();
        addressText.text = "������";
    }

    public void FHelp_Button()
    {
        UIDeactivation();
        Help_UI.SetActive(true);
        Text addressText = address.GetComponent<Text>();
        addressText.text = "����";
    }

    public void FDesktop_Button()
    {
        // PC ����
        Application.Quit();

        // �����Ϳ��� ����
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
                Debug.Log("��üȭ�� ���");
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                Debug.Log("â���");
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
            case 0: // ����
                QualitySettings.SetQualityLevel(5, true); 
                break;
            case 1: // �߰�
                QualitySettings.SetQualityLevel(3, true);
                break;
            case 2: // ����
                QualitySettings.SetQualityLevel(1, true);
                break;
            default:
                break;
        }
    }

    // Audio
    public void SetMasterVolume(float volume)
    {
        SoundManager.Instance.SetMasterVolume(volume);
    }

    public void SetBGMVolume(float volume)
    {
        SoundManager.Instance.SetBGMVolume(volume);
    }

    public void SetSFXVolume(float volume)
    {
        SoundManager.Instance.SetSFXVolume(volume);
    }

    public void FMasterButton()
    {
        if (!SoundManager.Instance.MasterVolumeMute) // ���Ұ� ���� �ƴҶ�
        {
            MasterVolumeBaseImage.SetActive(false);
            MasterVolumeMuteImage.SetActive(true);
        }
        else
        {
            MasterVolumeBaseImage.SetActive(true);
            MasterVolumeMuteImage.SetActive(false);
        }
        SoundManager.Instance.MasterVolumeMute = !SoundManager.Instance.MasterVolumeMute;
        SetMasterVolume(SoundManager.Instance.MasterVolume);
    }

    public void FBGMButton()
    {
        if (!SoundManager.Instance.BGMVolumeMute) // ���Ұ� ���� �ƴҶ�
        {
            BGMVolumeBaseImage.SetActive(false);
            BGMVolumeMuteImage.SetActive(true);
        }
        else
        {
            BGMVolumeBaseImage.SetActive(true);
            BGMVolumeMuteImage.SetActive(false);
        }
        SoundManager.Instance.BGMVolumeMute = !SoundManager.Instance.BGMVolumeMute;
        SetBGMVolume(SoundManager.Instance.BGMVolume);
    }

    public void FSFXButton()
    {
        if (!SoundManager.Instance.SFXVolumeMute) // ���Ұ� ���� �ƴҶ�
        {
            SFXVolumeBaseImage.SetActive(false);
            SFXVolumeMuteImage.SetActive(true);
        }
        else
        {
            SFXVolumeBaseImage.SetActive(true);
            SFXVolumeMuteImage.SetActive(false);
        }
        SoundManager.Instance.SFXVolumeMute = !SoundManager.Instance.SFXVolumeMute;
        SetSFXVolume(SoundManager.Instance.BGMVolume);
    }

    public void RoomUISet()
    {
        //UI�� �����ִ¿�� Ȯ���� ����
        for (int i = localDiskContent.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = localDiskContent.transform.GetChild(i);

            if (child != null)
            {
                Destroy(child.gameObject); // �ڽ� ��ü ����
            }
        }
        //�� index�ʱ�ȭ
        int mapIndex = 0;
        //���� ���� ���° index���� Ȯ��
        for (int i = 0; i < mapGenerator.mapList.Count; i++)
        {
            Map map = mapGenerator.mapList[i];

            // ���� Ȱ��ȭ�� ������ Ȯ��
            if (map.transform.gameObject.activeSelf)
            {
                mapIndex = i;
                continue; // Ȱ��ȭ�� ���� �ε��� ��ȯ
            }
        }
        /*LocalDisk_UI.GetComponent<LocalDiskUI>().currentLocakDiskMapIndex = mapIndex;*/
        //��list���� �������� ������
        LocalDisckUISet(mapIndex);
    }

    public void LocalDisckUISet(int mapIndex)
    {
        AddressSet(mapIndex);
        for (int i = localDiskContent.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = localDiskContent.transform.GetChild(i);

            if (child != null)
            {
                Destroy(child.gameObject); // �ڽ� ��ü ����
            }
        }
        GameObject currentMap = mapGenerator.mapList[mapIndex].transform.gameObject;
        
        if (mapIndex == 0)
        {
            //0�������϶� �������� ��Ż�� �����ͼ� ui����
            foreach (Transform child in currentMap.transform)
            {
                Portal curretnportal = child.GetComponent<Portal>();
                if (curretnportal != null)
                {
                    GameObject portalUI = Instantiate(UIPortal);
                    portalUI.transform.SetParent(localDiskContent.transform);
                    portalUI.transform.SetAsLastSibling();
                    LocalDisk_UI.GetComponent<LocalDiskUI>().telMap = currentMap;
                    Text mapName = portalUI.GetComponentInChildren<Text>();
                    Image[] images = portalUI.GetComponentsInChildren<Image>(true);
                    Image portalImage = images[1];

                    if (curretnportal.connectPortal != null)
                    {
                        Map connectedMap = curretnportal.connectPortal.transform.parent.GetComponent<Map>();
                        // �� �̸��� �����մϴ�.
                        mapName.text = connectedMap.mapName;
                        // LocalDiskUIPortalPanel�� connectMap�� �����մϴ�.
                        portalUI.GetComponent<LocalDiskUIPortalPanel>().connectMap = 
                            mapGenerator.mapList[mapGenerator.mapList.IndexOf(connectedMap)];

                        if (connectedMap != null)
                        {
                            // connectedMap�� Ŭ���� �Ǿ����� ���ο� ���� ��������Ʈ ����
                            if (connectedMap.isClear)
                            {
                                switch (connectedMap.Type)
                                {
                                    case Map.MapType.Middle:
                                        portalImage.sprite = portalUiList[0];
                                        continue;

                                    case Map.MapType.Boss:
                                        portalImage.sprite = portalUiList[1];
                                        continue;

                                    case Map.MapType.Download:
                                        portalImage.sprite = portalUiList[2];
                                        continue;
                                    case Map.MapType.Shop:
                                        portalImage.sprite = portalUiList[3];
                                        continue;
                                    case Map.MapType.RandomSpecial:
                                        switch (connectedMap.mapName)
                                        {
                                            case "������":
                                                portalImage.sprite = portalUiList[4];
                                                continue;
                                            case "���� �ɼ�":
                                                portalImage.sprite = portalUiList[5];
                                                continue;
                                            case "JuvaCafe":
                                                portalImage.sprite = portalUiList[6];
                                                continue;
                                            case "Window ��ȭ��":
                                                portalImage.sprite = portalUiList[7];
                                                continue;
                                        }
                                        continue;
                                }
                            }
                            else
                            {
                                Debug.Log("����");
                                switch (connectedMap.Type)
                                {
                                    case Map.MapType.Middle:
                                        portalImage.sprite = closePortalUiList[0];
                                        continue;
                                    case Map.MapType.Boss:
                                        portalImage.sprite = closePortalUiList[1];
                                        continue;
                                    case Map.MapType.Download:
                                        if (curretnportal.isLock)
                                        {
                                            portalImage.sprite = closePortalUiList[3];
                                        }
                                        else
                                        {
                                            portalImage.sprite = closePortalUiList[2];
                                        }
                                        continue;
                                    case Map.MapType.Shop:
                                        if (curretnportal.isLock)
                                        {
                                            portalImage.sprite = closePortalUiList[4];
                                        }
                                        else
                                        {
                                            portalImage.sprite = closePortalUiList[5];
                                        }
                                        continue;
                                    case Map.MapType.RandomSpecial:
                                        switch (connectedMap.mapName)
                                        {
                                            case "������":
                                                portalImage.sprite = closePortalUiList[6];
                                                continue;
                                            case "���� �ɼ�":
                                                portalImage.sprite = closePortalUiList[7];
                                                continue;
                                            case "JuvaCafe":
                                                portalImage.sprite = closePortalUiList[8];
                                                continue;
                                            case "Window ��ȭ��":
                                                portalImage.sprite = closePortalUiList[9];
                                                continue;
                                        }
                                        continue;
                                }
                            }
                        }
                    }
                }

            }
            //0��° ���϶� �������� �������� �����ͼ� ui����
            foreach (Transform child in currentMap.transform)
            {
                item fildItem = child.GetComponent<item>();
                if (fildItem != null)
                {
                    switch (fildItem.itemType)
                    {
                        case item.ItemType.Coin:
                            switch (fildItem.itemScore)
                            {
                                case 1:
                                    GameObject oneCoinUI = Instantiate(itemImageList[0]);
                                    oneCoinUI.transform.SetParent(localDiskContent.transform);
                                    oneCoinUI.transform.SetAsLastSibling();
                                    continue;
                                case 5:
                                    GameObject fiveCoinUI = Instantiate(itemImageList[1]);
                                    fiveCoinUI.transform.SetParent(localDiskContent.transform);
                                    fiveCoinUI.transform.SetAsLastSibling();
                                    continue;
                                case 10:
                                    GameObject tenCoinUI = Instantiate(itemImageList[2]);
                                    tenCoinUI.transform.SetParent(localDiskContent.transform);
                                    tenCoinUI.transform.SetAsLastSibling();
                                    continue;
                                case 15:
                                    GameObject fifteenCoinUI = Instantiate(itemImageList[3]);
                                    fifteenCoinUI.transform.SetParent(localDiskContent.transform);
                                    fifteenCoinUI.transform.SetAsLastSibling();
                                    continue;
                            }
                            continue;
                        case item.ItemType.Heal:
                            GameObject healUI = Instantiate(itemImageList[0]);
                            healUI.transform.SetParent(localDiskContent.transform);
                            healUI.transform.SetAsLastSibling();
                            continue;
                        case item.ItemType.TemHp:
                            GameObject itemUI = Instantiate(itemImageList[0]);
                            itemUI.transform.SetParent(localDiskContent.transform);
                            itemUI.transform.SetAsLastSibling();
                            continue;
                        case item.ItemType.Shiled:
                            GameObject shiledUI = Instantiate(itemImageList[0]);
                            shiledUI.transform.SetParent(localDiskContent.transform);
                            shiledUI.transform.SetAsLastSibling();
                            continue;
                        case item.ItemType.Spark:
                            GameObject sparkUI = Instantiate(itemImageList[0]);
                            sparkUI.transform.SetParent(localDiskContent.transform);
                            sparkUI.transform.SetAsLastSibling();
                            continue;
                    }


                }

            }
        }
        else
        {
            List<GameObject> currentPortalList = new();
            foreach (Transform child in currentMap.transform)
            {
                Portal curretnportal = child.GetComponent<Portal>();
                if (curretnportal != null)
                {
                    GameObject portalUI = Instantiate(UIPortal);
                    portalUI.transform.SetParent(localDiskContent.transform);
                    portalUI.transform.SetAsLastSibling();
                    LocalDisk_UI.GetComponent<LocalDiskUI>().telMap = currentMap;
                    currentPortalList.Add(portalUI);
                    Text mapName = portalUI.GetComponentInChildren<Text>();
                    Image[] images = portalUI.GetComponentsInChildren<Image>(true);
                    Image portalImage = images[1];

                    if (curretnportal.connectPortal != null)
                    {
                        Map connectedMap = curretnportal.connectPortal.transform.parent.GetComponent<Map>();
                        mapName.text = connectedMap.mapName;
                        portalUI.GetComponent<LocalDiskUIPortalPanel>().connectMap =
                            mapGenerator.mapList[mapGenerator.mapList.IndexOf(connectedMap)];
                        if (connectedMap != null)
                        {
                            // connectedMap�� Ŭ���� �Ǿ����� ���ο� ���� ��������Ʈ ����
                            if (connectedMap.isClear)
                            {
                                switch (connectedMap.Type)
                                {
                                    case Map.MapType.Middle:
                                        portalImage.sprite = portalUiList[0];
                                        continue;

                                    case Map.MapType.Boss:
                                        portalImage.sprite = portalUiList[1];
                                        continue;

                                    case Map.MapType.Download:
                                        portalImage.sprite = portalUiList[2];
                                        continue;
                                    case Map.MapType.Shop:
                                        portalImage.sprite = portalUiList[3];
                                        continue;
                                    case Map.MapType.RandomSpecial:
                                        switch (connectedMap.mapName)
                                        {
                                            case "������":
                                                portalImage.sprite = portalUiList[4];
                                                continue;
                                            case "���� �ɼ�":
                                                portalImage.sprite = portalUiList[5];
                                                continue;
                                            case "JuvaCafe":
                                                portalImage.sprite = portalUiList[6];
                                                continue;
                                            case "Window ��ȭ��":
                                                portalImage.sprite = portalUiList[7];
                                                continue;
                                        }
                                        continue;
                                }
                            }
                            else
                            {
                                switch (connectedMap.Type)
                                {
                                    case Map.MapType.Middle:
                                        portalImage.sprite = closePortalUiList[0];
                                        continue;
                                    case Map.MapType.Boss:
                                        portalImage.sprite = closePortalUiList[1];
                                        continue;
                                    case Map.MapType.Download:
                                        if (curretnportal.isLock)
                                        {
                                            portalImage.sprite = closePortalUiList[3];
                                        }
                                        else
                                        {
                                            portalImage.sprite = closePortalUiList[2];
                                        }
                                        continue;
                                    case Map.MapType.Shop:
                                        if (curretnportal.isLock)
                                        {
                                            portalImage.sprite = closePortalUiList[4];
                                        }
                                        else
                                        {
                                            portalImage.sprite = closePortalUiList[5];
                                        }
                                        continue;
                                    case Map.MapType.RandomSpecial:
                                        switch (connectedMap.mapName)
                                        {
                                            case "������":
                                                portalImage.sprite = closePortalUiList[6];
                                                continue;
                                            case "���� �ɼ�":
                                                portalImage.sprite = closePortalUiList[7];
                                                continue;
                                            case "JuvaCafe":
                                                portalImage.sprite = closePortalUiList[8];
                                                continue;
                                            case "Window ��ȭ��":
                                                portalImage.sprite = closePortalUiList[9];
                                                continue;
                                        }
                                        continue;
                                }
                            }

                        }
                    }
                }
            }
            Destroy(currentPortalList[0].gameObject);
            currentPortalList.RemoveAt(0);
            foreach (Transform child in currentMap.transform)
            {
                item fildItem = child.GetComponent<item>();
                if (fildItem != null)
                {
                    switch (fildItem.itemType)
                    {
                        case item.ItemType.Coin:
                            switch (fildItem.itemScore)
                            {
                                case 1:
                                    GameObject oneCoinUI = Instantiate(itemImageList[0]);
                                    oneCoinUI.transform.SetParent(localDiskContent.transform);
                                    oneCoinUI.transform.SetAsLastSibling();
                                    continue;
                                case 5:
                                    GameObject fiveCoinUI = Instantiate(itemImageList[1]);
                                    fiveCoinUI.transform.SetParent(localDiskContent.transform);
                                    fiveCoinUI.transform.SetAsLastSibling();
                                    continue;
                                case 10:
                                    GameObject tenCoinUI = Instantiate(itemImageList[2]);
                                    tenCoinUI.transform.SetParent(localDiskContent.transform);
                                    tenCoinUI.transform.SetAsLastSibling();
                                    continue;
                                case 15:
                                    GameObject fifteenCoinUI = Instantiate(itemImageList[3]);
                                    fifteenCoinUI.transform.SetParent(localDiskContent.transform);
                                    fifteenCoinUI.transform.SetAsLastSibling();
                                    continue;
                            }
                            continue;
                        case item.ItemType.Heal:
                            GameObject healUI = Instantiate(itemImageList[0]);
                            healUI.transform.SetParent(localDiskContent.transform);
                            healUI.transform.SetAsLastSibling();
                            continue;
                        case item.ItemType.TemHp:
                            GameObject itemUI = Instantiate(itemImageList[0]);
                            itemUI.transform.SetParent(localDiskContent.transform);
                            itemUI.transform.SetAsLastSibling();
                            continue;
                        case item.ItemType.Shiled:
                            GameObject shiledUI = Instantiate(itemImageList[0]);
                            shiledUI.transform.SetParent(localDiskContent.transform);
                            shiledUI.transform.SetAsLastSibling();
                            continue;
                        case item.ItemType.Spark:
                            GameObject sparkUI = Instantiate(itemImageList[0]);
                            sparkUI.transform.SetParent(localDiskContent.transform);
                            sparkUI.transform.SetAsLastSibling();
                            continue;
                    }


                }

            }
        }
        /*AddressSet(mapIndex);*/
    }
    public void AddressSet(int mapIndex)
    {
        addressList.Clear();
        List<Map> temList = new();
        Map currentMap = mapGenerator.mapList[mapIndex];
        //�ݺ����Ѿ���
        if (currentMap != null)
        {
           
            while (true)
            {
                Debug.Log("�ݺ�����");
                InfiniteLoopDetector.Run();
                if (currentMap.Type == Map.MapType.Start)
                {
                    break;
                }
                //���� ���� �����ͼ� ����ȸ��� ����Ʈ�� �߰�
                int siblingIndex = currentMap.transform.GetSiblingIndex(); // �� ��° �ڽ����� ��������
                Debug.Log(siblingIndex);
                if (siblingIndex != 0)
                {
                    //Portal connectMapPortal = mapGenerator.map.GetChild(siblingIndex).GetComponent<Portal>();
                    Transform nowMap = mapGenerator.map.transform.GetChild(siblingIndex);
                    Portal currentMapPortal = nowMap.GetComponentInChildren<Portal>();
                    Portal connectPortal = currentMapPortal.connectPortal;
                    Map connectMap = connectPortal.currentMap.GetComponent<Map>();
                    temList.Add(currentMap);
                    currentMap = connectMap;
                }
                
            }
            
        }
        
        temList.Add(mapGenerator.mapList[0]);
        
        for (int i = temList.Count-1; i >=0; i--)
        {
            addressList.Add(temList[i]);
        }
        Text addressText = address.GetComponent<Text>();
        addressText.text = "C:\\";
        for(int i = 0; i < addressList.Count; i++)
        {
            addressText.text += addressList[i].mapName + " > ";
        }
        addressText.text = addressText.text.TrimEnd('>');

    }
    

}
