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
    ItemManager itemManager;
    // Start is called before the first frame update
    private void Awake()
    {
        portalCollider = GetComponent<Collider2D>();
        currentMap = transform.parent.gameObject;
        teleportManager = TeleportManager.Instance;
        cameraManager = CameraManager.Instance;
        isUse = true;
        portalAnimator = GetComponent<Animator>();
        itemManager = ItemManager.Instance;

    }
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        Map parentMap = transform.parent.GetComponent<Map>();
        if (parentMap != null)
        {
            portalAnimator.SetBool("Clear", parentMap.isClear);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isLock)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                if (isLock)
                {
                    // Ű ��� ���� ���ο� ���� isLock ������Ʈ
                    if (itemManager.KeyUse())
                    {
                        isLock = false;  // Ű ��� ������ ��� ����
                    }
                    else
                    {
                        isLock = true;  // Ű�� ������ ��� ����
                    }
                }
            }
        }
        if (collision.gameObject.CompareTag("Player")&& isUse&& currentMap.GetComponent<Map>().isClear==true)
        {
            int currentPortalIndex = transform.parent.GetSiblingIndex();
            int connectPortalIndex = connectPortal.transform.parent.GetSiblingIndex();

            Player player = collision.GetComponent<Player>();
            //�÷��̾ �浹�Ұ�� �̵��Ҹ��� ������
            GameObject moveMap = connectPortal.transform.parent.gameObject;
            //�÷��̾ �浹�� ��ġ�� ���
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

