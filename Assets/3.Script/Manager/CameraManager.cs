using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance; // 싱글톤 인스턴스
    [SerializeField]
    private Camera playerCamera;  // 주 카메라
    [SerializeField]
    private CinemachineConfiner2D cinemachine;
    GameManager gameManager;

    private void Awake()
    {
        // 싱글톤 패턴 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // 매니저가 다른 씬으로 넘어가도 파괴되지 않게 설정
        }
        else
        {
            Destroy(gameObject);  // 이미 존재하면 새로운 매니저는 파괴
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

        // 카메라 이동 시간
        float duration = 0.2f;
        float elapsedTime = 0f;

        // 현재 카메라 위치
        Vector3 startingPosition = playerCamera.transform.position;

        // 카메라를 부드럽게 이동시키기
        while (elapsedTime < duration)
        {
            playerCamera.transform.position = Vector3.Lerp(startingPosition, cameraTargetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 최종적으로 목표 위치에 정확히 위치시키기
        playerCamera.transform.position = cameraTargetPosition;
    }
    public void CameraLimit(GameObject map)
    {
        PolygonCollider2D collider = map.GetComponent<PolygonCollider2D>();
        cinemachine.m_BoundingShape2D = collider;

        /*Bounds bounds = collider.bounds;
        float halfHeight = playerCamera.orthographicSize;
        float halfWidth = halfHeight * playerCamera.aspect;

        // 카메라의 이동 가능 범위 설정
        float minX = bounds.min.x + halfWidth;
        float maxX = bounds.max.x - halfWidth;
        float minY = bounds.min.y + halfHeight;
        float maxY = bounds.max.y - halfHeight;
        // 카메라 위치 제한 (카메라가 맵을 벗어나지 않도록 Clamp 사용)
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

