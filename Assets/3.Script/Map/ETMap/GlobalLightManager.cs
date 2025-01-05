using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GlobalLightManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static GlobalLightManager Instance { get; private set; }

    // �۷ι� ����Ʈ�� �����ϱ� ���� ����
    public Light2D globalLight;

    private void Awake()
    {
        // �̱��� �ʱ�ȭ: ���� Instance�� ��� ������ �� ������Ʈ�� Instance�� ����
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // �̹� Instance�� �����ϸ� �ߺ� ������Ʈ�� ����
            Destroy(gameObject);
        }
    }
}