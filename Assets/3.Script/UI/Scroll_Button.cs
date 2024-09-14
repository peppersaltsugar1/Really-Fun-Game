using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Scroll_Button : MonoBehaviour
{
    public ScrollRect scrollRect; // Scroll Rect ����
    public float scrollSpeed = 0.1f; // ��ũ�� �ӵ� ����

    // ��ũ���� ���� �ø��� �Լ�
    public void ScrollUp()
    {
        scrollRect.verticalNormalizedPosition += scrollSpeed;
        if (scrollRect.verticalNormalizedPosition > 1f)
            scrollRect.verticalNormalizedPosition = 1f;
    }

    // ��ũ���� �Ʒ��� ������ �Լ�
    public void ScrollDown()
    {
        scrollRect.verticalNormalizedPosition -= scrollSpeed;
        if (scrollRect.verticalNormalizedPosition < 0f)
            scrollRect.verticalNormalizedPosition = 0f;
    }
}
