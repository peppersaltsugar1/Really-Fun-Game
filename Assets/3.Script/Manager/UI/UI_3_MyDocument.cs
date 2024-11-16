using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_3_MyDocument : MonoBehaviour
{
    private static UI_3_MyDocument instance = null;

    // UI Window
    public GameObject UI_W_MyDocument = null;

    // Item Drag Area
    public GameObject WindowUI;

    // Detail
    public GameObject Button_Item_Prefab;
    public GameObject i_Item_Detail_Image_Prefab;
    public Text t_Item_Detail_Name_Prefab;
    public Text t_Item_Detail_Explanation_Prefab;
    public Text t_Item_Detail_Size_Prefab;

    public Image i_StorageView;
    public Text t_StorageRate;

    public Transform ContentItemGroup;

    // Manager
    private ItemManager itemManager = null;
    private StatusManager statusManager = null;

    public static UI_3_MyDocument Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UI_3_MyDocument>();

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(UI_3_MyDocument).Name);
                    instance = singletonObject.AddComponent<UI_3_MyDocument>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        itemManager = ItemManager.Instance;
        statusManager = StatusManager.Instance;
        GenerateItemList();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenUI()
    {
        if (UI_W_MyDocument != null)
        {
            UI_W_MyDocument.SetActive(true);
            // Debug.Log("OpenUI : UI_3_MyDocument");
        }
    }

    public void CloseUI()
    {
        if (UI_W_MyDocument != null)
        {
            UI_W_MyDocument.SetActive(false);
            // Debug.Log("CloseUI : UI_3_MyDocument");
        }
    }

    public void GenerateItemList()
    {
        foreach (Transform child in ContentItemGroup)
        {
            Destroy(child.gameObject);
        }

        foreach (var kvp in itemManager.itemList)
        {
            string itemName = kvp.Key;
            List<Item> items = kvp.Value;

            foreach (Item itemInfo in items)
            {
                GameObject newButton = Instantiate(Button_Item_Prefab, ContentItemGroup);

                ItemDragHandler dragHandler = newButton.AddComponent<ItemDragHandler>();  // ItemDragHandler 추가
                dragHandler.item = itemInfo;  // 아이템 정보 전달
                dragHandler.windowUI = WindowUI;  // WindowUI 오브젝트 참조 전달

                Transform childImageTransform = newButton.transform.Find("Image");
                Transform childTextTransform = newButton.transform.Find("Text");
                if (childImageTransform != null)
                {
                    Image ButtonImage = childImageTransform.GetComponent<Image>();
                    Text ButtonText = childTextTransform.GetComponent<Text>();
                    if (ButtonImage != null)
                    {
                        SetItemImage(ButtonImage, itemInfo.itemType);
                        ButtonText.text = itemInfo.ItemName;

                        newButton.GetComponent<Button>().onClick.AddListener(() => OpenItemDetail(itemInfo));
                    }
                    else
                    {
                        Debug.LogError("Error: Button image not found.");
                    }
                }
                else
                {
                    Debug.LogError("자식 오브젝트를 찾을 수 없습니다.");
                }

            }
        }
    }

    public void SetItemImage(Image buttonImage, Item.ItemType itemType)
    {
        string spriteSheetName = "";
        Sprite[] sprites;
        Sprite itemSprite = null;
        int ImageIndex = Item.ImageNumber[(int)itemType];

        // 이미지 설정
        // 아이템 타입에 따른 이미지 경로 설정
        switch (itemType)
        {
            case Item.ItemType.Coin1:
            case Item.ItemType.Coin5:
            case Item.ItemType.Coin10:
            case Item.ItemType.Coin15:
                spriteSheetName = "Item/use_Coin";
                break;
            case Item.ItemType.Key:
            case Item.ItemType.CardPack:
            case Item.ItemType.ProgramRemove:
            case Item.ItemType.ProgramRecycle:
                spriteSheetName = "Item/use_DropItem";
                break;

            /* 힐 아이템  추가할 것
            //  case Item.ItemType.Heal:
            //  case Item.ItemType.TemHp:
            //  case Item.ItemType.Shiled:
            //  case Item.ItemType.Spark:
            //      spriteSheetName = "Sprites/Items/Heal";
                    sprites = Resources.LoadAll<Sprite>(spriteSheetName);
                    itemSprite = sprites[ImageIndex];
            //      break;
            */

            default:
                Debug.LogError("Unknown item type.");
                return;
        }
        sprites = Resources.LoadAll<Sprite>(spriteSheetName);
        itemSprite = sprites[ImageIndex];

        if (itemSprite != null)
        {
            buttonImage.sprite = itemSprite;
            Debug.Log($"Item Image loaded: {itemType} -> {itemSprite.name}");
        }
        else
        {
            Debug.LogError($"Sprite not found for item type: {itemType} at path: {spriteSheetName}");
        }
    }

    public void OpenItemDetail(Item CurItem)
    {
        Image DetailImage = i_Item_Detail_Image_Prefab.GetComponent<Image>();
        SetItemImage(DetailImage, CurItem.itemType);
        t_Item_Detail_Name_Prefab.text = CurItem.ItemName;
        t_Item_Detail_Explanation_Prefab.text = CurItem.ItemInfomation;
        t_Item_Detail_Size_Prefab.text = CurItem.ItemSize.ToString() + "MB";
    }

    public void UpdateStorage()
    {
        i_StorageView.fillAmount = (float)statusManager.CurrentStorage / (statusManager.B_MaxStorage);
        t_StorageRate.text = statusManager.MaxStorage.ToString() + "MB 중 " + (statusManager.MaxStorage - statusManager.CurrentStorage).ToString() + "MB 사용 가능";
    }

    public void RemoveItemDetail()
    {
        Image DetailImage = i_Item_Detail_Image_Prefab.GetComponent<Image>();
        DetailImage.sprite = null;
        t_Item_Detail_Name_Prefab.text = "";
        t_Item_Detail_Explanation_Prefab.text = "";
        t_Item_Detail_Size_Prefab.text = "";
    }

    public void FItemDelete_Button()
    {
        //if (CurrentProgram != -1)
        //{
        //    programManager.RemoveProgram(CurrentProgram);
        //    CurrentProgram = -1;

        //    // i_Program_Detail_Image_Prefab
        //    t_Program_Detail_Name_Prefab.text = "";
        //    t_Program_Detail_Explanation_Prefab.text = "";
        //    t_Program_Detail_PowerExplanation_Prefab.text = "";


        //    GenerateProgramList();
        //}

        //Image detailImage = i_Program_Detail_Image_Prefab.GetComponent<Image>();

        //if (detailImage != null)
        //{
        //    detailImage.sprite = null;
        //}
        //else
        //{
        //    Debug.LogError("Image component not found");
        //}
    }
}
