using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScroll : MonoBehaviour
{
    public GameObject[] tiles;  // 스크롤할 타일 배열
    public float scrollSpeed = 2f;  // 타일이 이동하는 속도
    public BoxCollider2D scrollingArea;  // 타일들이 이동할 수 있는 범위를 제공하는 BoxCollider

    private float areaLeftBound;
    private float areaRightBound;

    private void Start()
    {
        // BoxCollider의 경계 설정
        areaLeftBound = scrollingArea.bounds.min.x;  // 왼쪽 경계
        areaRightBound = scrollingArea.bounds.max.x;  // 오른쪽 경계
    }

    private void Update()
    {
        // 각 타일을 오른쪽으로 이동시킴
        foreach (GameObject tile in tiles)
        {
            tile.transform.Translate(Vector3.right * scrollSpeed * Time.deltaTime);

            // 타일이 오른쪽 경계를 넘어가면 왼쪽으로 재배치
            if (tile.transform.position.x > areaRightBound)
            {
                RepositionTile(tile);
            }
        }
    }

    // 타일을 왼쪽으로 재배치하는 함수
    private void RepositionTile(GameObject tile)
    {
        // 가장 왼쪽에 있는 타일의 X 좌표를 찾아서 그 앞에 재배치
        float leftMostTileX = GetLeftMostTileX();
        float tileWidth = GetTileWidth(tile);
        tile.transform.position = new Vector3(leftMostTileX - tileWidth, tile.transform.position.y, tile.transform.position.z);
    }

    // 가장 왼쪽에 있는 타일의 X 좌표를 찾는 함수
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

    // 타일의 BoxCollider로 너비를 계산하는 함수
    private float GetTileWidth(GameObject tile)
    {
        BoxCollider2D Box = tile.GetComponent<BoxCollider2D>();
        if (Box != null)
        {
            return Box.size.x * tile.transform.localScale.x;  // 타일의 x축 크기를 계산
        }
        else
        {
            Debug.LogError("Tile에 BoxCollider가 없습니다!");
            return 1f;  // 기본값으로 1을 반환 (임시)
        }
    }
}
