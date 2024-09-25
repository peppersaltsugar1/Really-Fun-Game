using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportManager : MonoBehaviour
{
    MapGenerator mapGenerator;
    //맵을 이동시킬거리
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

        // Collider2D가 있는지 확인
        if (collider != null)
        {
            // Collider2D의 bounds를 이용해 x 축 방향 크기를 가져옴
            distance = collider.bounds.size.x;
            distance = (float)(distance * 1.5);
            if (vec.x > 0)
            {
                // 오른쪽으로 이동 (x축 양의 방향)
                map.transform.position = new Vector2(map.transform.position.x + distance, map.transform.position.y);
            }
            else if (vec.x < 0)
            {
                // 왼쪽으로 이동 (x축 음의 방향)
                map.transform.position = new Vector2(map.transform.position.x - distance, map.transform.position.y);
            }
        }
    }
}
