using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FishController : MonoBehaviour
{
    public bool IsMoving = false; // Moving ���� ����
    public bool IsFlipped = false; // X �ø� ����
    public float Speed = 0f; // ������ �ӵ�
    public float MaxPlusSpeed;
    public float MinPlusSpeed;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = IsFlipped; // X �ø� ���� ����ȭ
        }
    }
}

