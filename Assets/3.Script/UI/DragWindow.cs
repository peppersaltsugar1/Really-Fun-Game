using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragWindow : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    private RectTransform rectTransform;
    private Vector2 offset;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // �巡�� ���� �� ���콺�� â�� ����� ��ġ�� ���
    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out offset
        );
    }

    // �巡�� �� â�� ��ġ ������Ʈ
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out mousePosition
        );

        // â�� ���ο� ��ġ ���
        Vector2 newPosition = mousePosition - offset;

        // �θ� RectTransform ��������
        RectTransform parentRect = rectTransform.parent as RectTransform;

        if (parentRect != null)
        {
            // �̵� ���� ����
            float minX = -parentRect.rect.width / 2 + rectTransform.rect.width / 2;
            float maxX = parentRect.rect.width / 2 - rectTransform.rect.width / 2;
            float minY = -parentRect.rect.height / 2 + rectTransform.rect.height / 2;
            float maxY = parentRect.rect.height / 2 - rectTransform.rect.height / 2;

            rectTransform.anchoredPosition = new Vector2(
                Mathf.Clamp(newPosition.x, minX, maxX),
                Mathf.Clamp(newPosition.y, minY, maxY)
            );
        }
    }
}

