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

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
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
            UIManager.Instance.UpdateStorage();
            UIManager.Instance.RemoveItemDetail();
            Destroy(gameObject);
        }
        else
        {
            // 원래 자리로 복귀
            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = Vector2.zero;
            UIManager.Instance.GenerateItemList();
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