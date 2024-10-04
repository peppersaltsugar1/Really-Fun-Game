using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal connectPortal;
    public Camera playerCamera;
    public bool isUse;
    public bool isLock;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("부딪힘");
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("플레이어맞음");
            collision.transform.position = connectPortal.transform.position;
            Vector3 portalPos = new Vector3(transform.position.x, playerCamera.transform.position.y,playerCamera.transform.position.z);
            playerCamera.transform.position = portalPos;
            // 카메라 이동 코루틴 시작
            StartCoroutine(MoveCameraSmoothly(connectPortal.transform.position));

        }
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
    public void Connect(Portal coPotal)
    {
        connectPortal = coPotal;
        coPotal.connectPortal = connectPortal;
    }
}

