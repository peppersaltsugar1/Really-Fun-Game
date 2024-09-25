using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal connectPortal;
    public Camera playerCamera;
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
        GameObject moveMap = connectPortal.transform.parent.gameObject;
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRigidbody = collision.GetComponent<Rigidbody2D>();
            if (playerRigidbody != null)
            {
                // 속도 벡터를 정규화하여 방향만 얻기
                Vector2 moveDirection = playerRigidbody.velocity.normalized;

                // 이동할 거리를 정한다. (예: 1.5f)
                float distanceToMove = 1.5f;

                // 캐릭터를 방향에 맞게 조금 이동시킨다
                Vector3 newPosition = collision.transform.position + (Vector3)moveDirection * distanceToMove;
                collision.transform.position = newPosition;

                Debug.Log("플레이어맞음");
                collision.transform.position = connectPortal.transform.position;
                Vector3 portalPos = new Vector3(transform.position.x, playerCamera.transform.position.y, playerCamera.transform.position.z);
                playerCamera.transform.position = portalPos;
                // 카메라 이동 코루틴 시작
                StartCoroutine(MoveCameraSmoothly(connectPortal.transform.position));


            }
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

