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
        Debug.Log("�ε���");
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("�÷��̾����");
            collision.transform.position = connectPortal.transform.position;
            Vector3 portalPos = new Vector3(transform.position.x, playerCamera.transform.position.y,playerCamera.transform.position.z);
            playerCamera.transform.position = portalPos;
            // ī�޶� �̵� �ڷ�ƾ ����
            StartCoroutine(MoveCameraSmoothly(connectPortal.transform.position));

        }
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
    public void Connect(Portal coPotal)
    {
        connectPortal = coPotal;
        coPotal.connectPortal = connectPortal;
    }
}

