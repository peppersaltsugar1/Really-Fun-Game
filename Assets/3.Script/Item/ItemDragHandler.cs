using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Item item;  // 드래그할 아이템
    public GameObject windowUI;  // WindowUI 참조
    private Transform originalParent;  // 드래그 전 원래 부모 저장
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    UI_3_MyDocument ui_3_MyDocument;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        ui_3_MyDocument = UI_3_MyDocument.Instance;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 중에는 최상위 레이어에로 이동하고 레이캐스트를 무시하도록 설정
        originalParent = transform.parent;
        transform.SetParent(transform.root);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / CanvasScalerFactor();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!IsPointerInsideUI(eventData.position, windowUI))
        {
            // WindowUI 바깥으로 드롭했을 경우 아이템 버리기
            Debug.Log("Item Dropped Outside WindowUI, Item Deleted");
            ItemManager.Instance.RemoveItem(item);
            ui_3_MyDocument.UpdateStorage();
            ui_3_MyDocument.RemoveItemDetail();
            Destroy(gameObject);
        }
        else
        {
            // 원래 자리로 복귀
            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = Vector2.zero;
            ui_3_MyDocument.GenerateItemList();
        }
        canvasGroup.blocksRaycasts = true;
    }

    private bool IsPointerInsideUI(Vector2 pointerPosition, GameObject uiObject)
    {
        RectTransform uiRect = uiObject.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(uiRect, pointerPosition, null);
    }

    private float CanvasScalerFactor()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        return canvas.scaleFactor;
    }
}