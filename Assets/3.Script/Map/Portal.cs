using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal connectPortal;
    public GameObject currentMap;
    public Collider2D portalCollider;
    TeleportManager teleportManager;
    CameraManager cameraManager;
    public bool isUse;
    public bool isLock;
    bool isRightMove = true;
    // Start is called before the first frame update
    private void Awake()
    {
        portalCollider = GetComponent<Collider2D>();
        currentMap = transform.parent.gameObject;
        
    }
    void Start()
    {
        teleportManager = TeleportManager.Instance;
        cameraManager = CameraManager.Instance;
        isUse = true;
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")&& isUse)
        {
            portalCollider.enabled = false;
            connectPortal.portalCollider.enabled = false;
            Player player = collision.GetComponent<Player>();
            //플레이어가 충돌할경우 이동할맵을 가져옴
            GameObject moveMap = connectPortal.transform.parent.gameObject;
            //플레이어가 충돌한 위치를 계산
            if (collision.transform.position.x > 0 && transform.position.x > 0)
            {
                if (collision.transform.position.x - transform.position.x > 0)
                {
                    isRightMove = false;
                }
                else
                {
                    isRightMove = true;
                }
            }
            else if (collision.transform.position.x > 0 && transform.position.x < 0)
            {
                isRightMove = false;
            }
            else if (collision.transform.position.x < 0 && transform.position.x > 0)
            {
                isRightMove = true;
            }
            else if (collision.transform.position.x > 0 && transform.position.x > 0)
            {
                if ((collision.transform.position.x * -1) - (transform.position.x * -1) > 0)
                {
                    isRightMove = false;
                }
                else
                {
                    isRightMove = true;
                }
            }
            teleportManager.MapTeleportPortal(moveMap,isRightMove,transform.gameObject);
            teleportManager.PlayerTeleport(player, this,connectPortal);
            cameraManager.PortalCameraMove(transform.gameObject,moveMap);
            cameraManager.CameraLimit(moveMap);
            currentMap = transform.parent.gameObject;
            currentMap.SetActive(false);

        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        isUse = true;
    }
    
    
}

