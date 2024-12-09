using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Material : MonoBehaviour
{
    public SpriteRenderer parentRenderer; // �θ� Sprite Renderer
    public Material animatedMaterial;     // �´� ȿ���� ����� Material
    public float effectDuration;   // ȿ�� ���� �ð�

    private Material originalMaterial;    // �θ��� ���� Material
    private bool isEffectActive = false;  // ȿ�� ���� ������ ����

    void Start()
    {
        // �θ��� ���� Material ����
        if (parentRenderer != null)
        {
            originalMaterial = parentRenderer.material;
        }
    }

    public void TriggerHitEffect()
    {
        if (!isEffectActive) // ȿ���� ���� ���� �ƴ� ���� ����
        {
            Debug.Log("�Ѿ��");

            StartCoroutine(HitEffectCoroutine());
        }
    }

    private IEnumerator HitEffectCoroutine()
    {
        isEffectActive = true;
        Debug.Log("�ڷ�ƾ ����");

        if (parentRenderer != null)
        {
            // �θ��� Material�� �´� ȿ�� Material�� ����
            parentRenderer.material = animatedMaterial;
        }

        // ȿ�� ���� �ð� ���� ���
        yield return new WaitForSeconds(effectDuration);

        if (parentRenderer != null)
        {
            // �θ��� Material�� ���� Material�� ����
            parentRenderer.material = originalMaterial;
        }

        isEffectActive = false;
    }
}
