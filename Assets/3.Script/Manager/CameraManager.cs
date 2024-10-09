using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance; // �̱��� �ν��Ͻ�
    [SerializeField]
    private Camera playerCamera;  // �� ī�޶�
    [SerializeField]
    private CinemachineConfiner2D cinemachine;
    GameManager gameManager;

    private void Awake()
    {
        // �̱��� ���� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // �Ŵ����� �ٸ� ������ �Ѿ�� �ı����� �ʰ� ����
        }
        else
        {
            Destroy(gameObject);  // �̹� �����ϸ� ���ο� �Ŵ����� �ı�
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PortalCameraMove(GameObject currentPortal,GameObject connectPortal)
    {
        Vector3 portalPos = new Vector3(currentPortal.transform.position.x, playerCamera.transform.position.y, playerCamera.transform.position.z);
        playerCamera.transform.position = portalPos;
        StartCoroutine(MoveCameraSmoothly(connectPortal.transform.position));
    }

    private IEnumerator MoveCameraSmoothly(Vector3 targetPosition)
    {
        Vector3 cameraTargetPosition = new Vector3(targetPosition.x, targetPosition.y, playerCamera.transform.position.z);

        // ī�޶� �̵� �ð�
        float duration = 0.2f;
        float elapsedTime = 0f;

        // ���� ī�޶� ��ġ
        Vector3 startingPosition = playerCamera.transform.position;

        // ī�޶� �ε巴�� �̵���Ű��
        while (elapsedTime < duration)
        {
            playerCamera.transform.position = Vector3.Lerp(startingPosition, cameraTargetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ���������� ��ǥ ��ġ�� ��Ȯ�� ��ġ��Ű��
        playerCamera.transform.position = cameraTargetPosition;
    }
    public void CameraLimit(GameObject map)
    {
        PolygonCollider2D collider = map.GetComponent<PolygonCollider2D>();
        cinemachine.m_BoundingShape2D = collider;

        /*Bounds bounds = collider.bounds;
        float halfHeight = playerCamera.orthographicSize;
        float halfWidth = halfHeight * playerCamera.aspect;

        // ī�޶��� �̵� ���� ���� ����
        float minX = bounds.min.x + halfWidth;
        float maxX = bounds.max.x - halfWidth;
        float minY = bounds.min.y + halfHeight;
        float maxY = bounds.max.y - halfHeight;
        // ī�޶� ��ġ ���� (ī�޶� ���� ����� �ʵ��� Clamp ���)
        Vector3 cameraPosition = playerCamera.transform.position;
        cameraPosition.x = Mathf.Clamp(cameraPosition.x, minX, maxX);
        cameraPosition.y = Mathf.Clamp(cameraPosition.y, minY, maxY);
        playerCamera.transform.position = cameraPosition;*/

    }
    public void CameraPlayerSet()
    {
        playerCamera.transform.position = 
            new Vector3(gameManager.player.transform.position.x,gameManager.player.transform.position.y,playerCamera.transform.position.z);
    }
}

