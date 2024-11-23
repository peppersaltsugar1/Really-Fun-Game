using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportManager : MonoBehaviour
{
    private static TeleportManager instance = null;
    CameraManager cameraManager;
    GameManager gameManager;
    [SerializeField]
    MapGenerator mapGenerator;
    [Header("�ڷ���Ʈ �Ǵ°Ÿ�")]
    [SerializeField]
    float playerTelDisX;
    [SerializeField]
    float playerTelDisY;
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
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            HiddenTel();
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
    // Start is called before the first frame update
    void Start()
    {
        cameraManager = CameraManager.Instance;
        gameManager = GameManager.Instance;
    }

    
    public void PlayerTeleport(Player player,Portal currentPortal,Portal connectPortal)
    {

        // �θ� ��ü (Map) Ȯ��
        Transform currentMap = currentPortal.transform.parent;
        Transform connectMap = connectPortal.transform.parent;

        // MapGenerator�� �ڽ� ��ü�κ��� index Ȯ��
        int currentMapIndex = currentMap.GetSiblingIndex();       // ���� ���� �ε���
        int connectMapIndex = connectMap.GetSiblingIndex();       // ����� ���� �ε���

        // �⺻ �̵� ��ġ (connectPortal ��ġ)
        Vector2 newPosition = connectPortal.transform.position;

        // �ε����� ���Ͽ� �̵� ���� ����
        if (connectMapIndex < currentMapIndex)
        {
            // connectMap�� �ε����� �� ���� �� �������� �̵�
            newPosition.x -= playerTelDisX; // �ʿ信 ���� ���� ����
            newPosition.y += playerTelDisY;
        }
        else if (connectMapIndex > currentMapIndex)
        {
            // connectMap�� �ε����� �� ���� �� ���������� �̵�
            newPosition.x += playerTelDisX; // �ʿ信 ���� ���� ����
            newPosition.y += playerTelDisY;

        }

        // �÷��̾� �̵�
        player.transform.position = newPosition;

        // ��Ż ��� ���� ���� ����
        currentPortal.isUse = false;
        connectPortal.isUse = false;
        if (connectMap.GetComponent<Map>().Type == Map.MapType.RandomSpecial || 
            connectMap.GetComponent<Map>().Type == Map.MapType.Shop || 
            connectMap.GetComponent<Map>().Type == Map.MapType.Download)
        {
            cameraManager.SpecialMapCamera(connectMap.GetComponent<Map>());
        }
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
        }
        moveMap.SetActive(true);
    }
    private IEnumerator PlayerTeleport_Co(Portal currentPortal)
    {
        
        yield return new WaitForSeconds(1f);  // 1�� �� �ٽ� ��Ż Ȱ��ȭ
        currentPortal.isUse = true;

    }
    public void LocalDisckTel(GameObject telMap)
    {
        int currentMapIndex =0;
        for (int i = 0; i < mapGenerator.mapList.Count; i++)
        {
            Map map = mapGenerator.mapList[i];

            // ���� Ȱ��ȭ�� ������ Ȯ��
            if (map.transform.gameObject.activeSelf)
            {
                currentMapIndex = i;
                continue; // Ȱ��ȭ�� ���� �ε��� ��ȯ
            }
        }
        int telMapIndex = mapGenerator.mapList.IndexOf(telMap.GetComponent<Map>());
        if(currentMapIndex != telMapIndex)
        {
            mapGenerator.mapList[currentMapIndex].gameObject.SetActive(false);
            telMap.SetActive(true);
            Vector2 telPoint = telMap.transform.Find("TeleportPoint").transform.position;
            gameManager.player.transform.position = telPoint;
            cameraManager.CameraLimit(telMap);
        }
    }
    public void PortalUse_co(Portal currentPortal)
    {
        StartCoroutine(PlayerTeleport_Co(currentPortal));
    }
    public void StartPlayerTel()
    {
        Debug.Log(gameManager);
        gameManager.player.transform.position = mapGenerator.mapList[0].transform.Find("TeleportPoint").transform.position;
    }
    public void HiddenTel()
    {
        int currentMapIndex = 0;
        for (int i = 0; i < mapGenerator.mapList.Count; i++)
        {
            Map map = mapGenerator.mapList[i];

            // ���� Ȱ��ȭ�� ������ Ȯ��
            if (map.transform.gameObject.activeSelf)
            {
                currentMapIndex = i;
                continue; // Ȱ��ȭ�� ���� �ε��� ��ȯ
            }
        }
        mapGenerator.mapList[currentMapIndex].gameObject.SetActive(false);

        // �������� ���� �� ����
        int hiddenIndex = Random.Range(0, mapGenerator.hiddenMap.transform.childCount);
        GameObject hiddenMap = mapGenerator.hiddenMap.transform.GetChild(hiddenIndex).gameObject;
        Debug.Log(hiddenMap);
        // ������ ���� �� Ȱ��ȭ
        hiddenMap.SetActive(true);

        // �÷��̾��� �ڷ���Ʈ ��ġ ����
        Vector2 telPoint = hiddenMap.transform.Find("TeleportPoint").transform.position;
        gameManager.player.transform.position = telPoint;

        // ī�޶� ���� ����
        cameraManager.CameraLimit(hiddenMap);
    }
}
