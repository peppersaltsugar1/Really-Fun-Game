using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportManager : MonoBehaviour
{
    private static TeleportManager instance = null;
    CameraManager cameraManager;
    GameManager gameManager;
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public static TeleportManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
    MapGenerator mapGenerator;
    //맵을 이동시킬거리
    float distance;
    // Start is called before the first frame update
    void Start()
    {
        cameraManager = CameraManager.Instance;
        gameManager = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void PlayerTeleport(Player player,Portal currentPortal,Portal connectPortal)
    {
        Debug.Log("텔레포트시킴");
        StartCoroutine(PlayerTeleport_Co(currentPortal, connectPortal));
        Vector2 newPosition = connectPortal.transform.position;
        player.transform.position = newPosition;
    }
    public void MapTeleportPortal(GameObject moveMap,bool isRightMove,GameObject currentMap)
    {
        BoxCollider2D collider = moveMap.GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            
            float distance = collider.size.x;
            distance *= 1.5f;
            if (isRightMove)
            {

                moveMap.transform.position = new Vector2(currentMap.transform.position.x, currentMap.transform.position.y);
                moveMap.transform.position = new Vector2(moveMap.transform.position.x + distance, moveMap.transform.position.y);
            }
            else
            {
                moveMap.transform.position = new Vector2(currentMap.transform.position.x, currentMap.transform.position.y);
                moveMap.transform.position = new Vector2(moveMap.transform.position.x - distance, moveMap.transform.position.y);
            }
            Debug.Log(moveMap.transform.position);
        }
        moveMap.SetActive(true);
    }
    private IEnumerator PlayerTeleport_Co(Portal currentPortal, Portal connectPortal)
    {
        Debug.Log("코루틴사용");
        currentPortal.isUse = false;
        connectPortal.isUse = false;
        yield return new WaitForSeconds(2f);  // 1초 후 다시 포탈 활성화
        Debug.Log("코루틴종료");
        currentPortal.portalCollider.enabled = true;
        connectPortal.portalCollider.enabled = true;
        currentPortal.isUse = true;
        connectPortal.isUse = true;

    }
    public void LocalDisckTel(int maplistIndex)
    {
        Transform telPos = mapGenerator.mapList[maplistIndex].transform.Find("TeleportPoint");
        Debug.Log(telPos);
        gameManager.player.transform.position = telPos.position;
        int mapIndex = 0;
        //현제 맵이 몇번째 index인지 확인
        for (int i = 0; i < mapGenerator.mapList.Count; i++)
        {
            Map map = mapGenerator.mapList[i];

            // 현재 활성화된 맵인지 확인
            if (map.transform.gameObject.activeSelf)
            {
                mapIndex = i;
                continue; // 활성화된 맵의 인덱스 반환
            }
        }
        mapGenerator.mapList[mapIndex].transform.gameObject.SetActive(false);
        mapGenerator.mapList[maplistIndex].transform.gameObject.SetActive(true);

    }
}
