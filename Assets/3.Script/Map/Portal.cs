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
    Animator portalAnimator;
    // Start is called before the first frame update
    private void Awake()
    {
        portalCollider = GetComponent<Collider2D>();
        currentMap = transform.parent.gameObject;
        teleportManager = TeleportManager.Instance;
        cameraManager = CameraManager.Instance;
        isUse = true;
        portalAnimator = GetComponent<Animator>();

    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        /*portalAnimator.SetBool("Clear", currentMap.GetComponentInParent<Map>().isClear);*/
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")&& isUse)
        {
            int currentPortalIndex = transform.parent.GetSiblingIndex();
            int connectPortalIndex = connectPortal.transform.parent.GetSiblingIndex();

            Player player = collision.GetComponent<Player>();
            //플레이어가 충돌할경우 이동할맵을 가져옴
            GameObject moveMap = connectPortal.transform.parent.gameObject;
            //플레이어가 충돌한 위치를 계산
            if(currentPortalIndex < connectPortalIndex)
            {
                isRightMove = transform;
            }
            else
            {
                isRightMove = false;
            }
            
            teleportManager.MapTeleportPortal(moveMap,isRightMove,transform.gameObject);
            teleportManager.PlayerTeleport(player, this,connectPortal);
            cameraManager.CameraLimit(moveMap);
            currentMap = transform.parent.gameObject;
            currentMap.SetActive(false);

        }
    }
   
    private void OnTriggerExit2D(Collider2D collision)
    {
        teleportManager.PortalUse_co(this);
    }


}

