using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScroll : MonoBehaviour
{
    public GameObject[] sprites; // ��ũ���� ��������Ʈ �迭
    public float speed; // �̵� �ӵ�
    public float startPositionX = 93f; // ��������Ʈ�� ���ʱ��� �����ϸ� �ڷ���Ʈ ����
    public float resetPositionX = -93f; // ��������Ʈ�� �� �������� �̵��� ��ġ (ȭ�� ��)

    public GameObject Parents;

    private FolderNode ParentsPos;

    public float Teleposition;
    public float Resetposition;

    void Start()
    {
        ParentsPos = Parents.GetComponent<FolderNode>();
        
    }
    private void Update()
    {

        ParentsPos.nowPosition = Parents.transform.position.x;

        Teleposition = (startPositionX + ParentsPos.nowPosition);
        Resetposition = (resetPositionX + ParentsPos.nowPosition);


        foreach (GameObject sprite in sprites)
        {
            // ���������� �̵�
            sprite.transform.Translate(Vector3.right * speed * Time.deltaTime);

            // ��������Ʈ�� ȭ�� ������ ���� �������� ��
            if (sprite.transform.position.x >= Teleposition)
            {
                Debug.Log(Teleposition);
                // ��������Ʈ�� ���� ������ �̵�
                Vector3 newPosition = sprite.transform.position;
                newPosition.x = Resetposition;
                Debug.Log(newPosition.x);
                sprite.transform.position = newPosition;
                Debug.Log(sprite.transform.position);
            }
        }
    }
}
