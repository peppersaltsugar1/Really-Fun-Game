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

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
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
            UIManager.Instance.UpdateStorage();
            UIManager.Instance.RemoveItemDetail();
            Destroy(gameObject);
        }
        else
        {
            // ���� �ڸ��� ����
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