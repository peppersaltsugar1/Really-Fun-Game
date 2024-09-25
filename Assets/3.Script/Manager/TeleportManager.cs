using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportManager : MonoBehaviour
{
    MapGenerator mapGenerator;
    //���� �̵���ų�Ÿ�
    float distance;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Teleport(GameObject map, Vector2 vec)
    {
        Collider2D collider = map.GetComponent<Collider2D>();

        // Collider2D�� �ִ��� Ȯ��
        if (collider != null)
        {
            // Collider2D�� bounds�� �̿��� x �� ���� ũ�⸦ ������
            distance = collider.bounds.size.x;
            distance = (float)(distance * 1.5);
            if (vec.x > 0)
            {
                // ���������� �̵� (x�� ���� ����)
                map.transform.position = new Vector2(map.transform.position.x + distance, map.transform.position.y);
            }
            else if (vec.x < 0)
            {
                // �������� �̵� (x�� ���� ����)
                map.transform.position = new Vector2(map.transform.position.x - distance, map.transform.position.y);
            }
        }
    }
}
