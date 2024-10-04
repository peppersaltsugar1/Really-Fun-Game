using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScroll : MonoBehaviour
{
    public GameObject[] tiles;  // ��ũ���� Ÿ�� �迭
    public float scrollSpeed = 2f;  // Ÿ���� �̵��ϴ� �ӵ�
    public BoxCollider2D scrollingArea;  // Ÿ�ϵ��� �̵��� �� �ִ� ������ �����ϴ� BoxCollider

    private float areaLeftBound;
    private float areaRightBound;

    private void Start()
    {
        // BoxCollider�� ��� ����
        areaLeftBound = scrollingArea.bounds.min.x;  // ���� ���
        areaRightBound = scrollingArea.bounds.max.x;  // ������ ���
    }

    private void Update()
    {
        // �� Ÿ���� ���������� �̵���Ŵ
        foreach (GameObject tile in tiles)
        {
            tile.transform.Translate(Vector3.right * scrollSpeed * Time.deltaTime);

            // Ÿ���� ������ ��踦 �Ѿ�� �������� ���ġ
            if (tile.transform.position.x > areaRightBound)
            {
                RepositionTile(tile);
            }
        }
    }

    // Ÿ���� �������� ���ġ�ϴ� �Լ�
    private void RepositionTile(GameObject tile)
    {
        // ���� ���ʿ� �ִ� Ÿ���� X ��ǥ�� ã�Ƽ� �� �տ� ���ġ
        float leftMostTileX = GetLeftMostTileX();
        float tileWidth = GetTileWidth(tile);
        tile.transform.position = new Vector3(leftMostTileX - tileWidth, tile.transform.position.y, tile.transform.position.z);
    }

    // ���� ���ʿ� �ִ� Ÿ���� X ��ǥ�� ã�� �Լ�
    private float GetLeftMostTileX()
    {
        float leftMostX = float.MaxValue;
        foreach (GameObject tile in tiles)
        {
            if (tile.transform.position.x < leftMostX)
            {
                leftMostX = tile.transform.position.x;
            }
        }
        return leftMostX;
    }

    // Ÿ���� BoxCollider�� �ʺ� ����ϴ� �Լ�
    private float GetTileWidth(GameObject tile)
    {
        BoxCollider2D Box = tile.GetComponent<BoxCollider2D>();
        if (Box != null)
        {
            return Box.size.x * tile.transform.localScale.x;  // Ÿ���� x�� ũ�⸦ ���
        }
        else
        {
            Debug.LogError("Tile�� BoxCollider�� �����ϴ�!");
            return 1f;  // �⺻������ 1�� ��ȯ (�ӽ�)
        }
    }
}
