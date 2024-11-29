using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Item item;  // �巡���� ������
    public GameObject windowUI;  // WindowUI ����
    public GameObject Button_Drag_Prefab; // �巡�� �� ǥ���� ������
    private Transform originalParent;  // �巡�� �� ���� �θ� ����
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private GameObject dragIcon; // �巡�� �� ǥ�õǴ� ������
    UI_3_MyDocument ui_3_MyDocument;

    private ItemManager itemManager;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        ui_3_MyDocument = UI_3_MyDocument.Instance;
        itemManager = ItemManager.Instance;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Button_Drag_Prefab == null)
        {
            Debug.LogError("Button_Drag_Prefab si null");
            return;
        }

        // �巡�� ���� �� �巡�׿� ������ ����
        dragIcon = Instantiate(Button_Drag_Prefab, transform.root);
        dragIcon.transform.SetAsLastSibling();

        // �巡�� ������ �̹��� ����
        Image dragImage = dragIcon.GetComponentInChildren<Image>();
        if (dragImage != null)
        {
            dragImage.sprite = GetItemSprite(item.itemType);
        }
        else
        {
            Debug.LogError("dragImage is null");
        }


        // ���� ��ư ���� ����
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
        {
            dragIcon.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // �巡�� ������ ����
        if (dragIcon != null)
        {
            Destroy(dragIcon);
        }

        if (!IsPointerInsideUI(eventData.position, windowUI))
        {
            // WindowUI �ٱ����� ������� ���
            if (IsPointerInRightSide(eventData.position))
            {
                // ������ ������ ������ ����
                // Debug.Log("Right Side Generate Item");
                itemManager.DropItem(item, true);
            }
            else
            {
                // ���� ������ ������ ����
                // Debug.Log("Left Side Generate Item");
                itemManager.DropItem(item, false);
            }

            ui_3_MyDocument.UpdateStorage();
            ui_3_MyDocument.RemoveItemDetail();
            Destroy(gameObject);
        }
        else
        {
            ui_3_MyDocument.GenerateItemList();
        }

        canvasGroup.alpha = 1f; // ���� ������ ����
        canvasGroup.blocksRaycasts = true;
    }

    private bool IsPointerInRightSide(Vector2 pointerPosition)
    {
        return pointerPosition.x > Screen.width / 2;
    }

    private bool IsPointerInsideUI(Vector2 pointerPosition, GameObject uiObject)
    {
        RectTransform uiRect = uiObject.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(uiRect, pointerPosition, null);
    }

    private Sprite GetItemSprite(Item.ItemType itemType)
    {
        string spriteSheetName = itemManager.GetSpriteSheetName(itemType);
        int spriteIndex = itemManager.GetImageIndex(itemType);
        Sprite[] sprites = Resources.LoadAll<Sprite>(spriteSheetName);
        if (spriteIndex >= 0 && spriteIndex < sprites.Length)
        {
            return sprites[spriteIndex];
        }
        else
        {
            Debug.LogError($"Sprite not found for item type: {itemType}");
            return null;
        }
    }
}
