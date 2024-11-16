using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Item item;  // �巡���� ������
    public GameObject windowUI;  // WindowUI ����
    private Transform originalParent;  // �巡�� �� ���� �θ� ����
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
        // �巡�� �߿��� �ֻ��� ���̾�� �̵��ϰ� ����ĳ��Ʈ�� �����ϵ��� ����
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
            // WindowUI �ٱ����� ������� ��� ������ ������
            Debug.Log("Item Dropped Outside WindowUI, Item Deleted");
            ItemManager.Instance.RemoveItem(item);
            ui_3_MyDocument.UpdateStorage();
            ui_3_MyDocument.RemoveItemDetail();
            Destroy(gameObject);
        }
        else
        {
            // ���� �ڸ��� ����
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