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
    [SerializeField]
    private CinemachineVirtualCamera cinemachineVir;
    GameManager gameManager;
    [Header("Ư���� ī�޶� ����")]
    [SerializeField]
    private float chargeCamera;
    [SerializeField]
    private float guardCamera;
    [SerializeField]
    private float trashCamera;
    [SerializeField]
    private float cafeCamera;
    [SerializeField]
    private float downloadCamera;
    [SerializeField]
    private float shopCamera;
    

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
    
    public void CameraLimit(GameObject map)
    {
        PolygonCollider2D collider = map.GetComponent<PolygonCollider2D>();
        cinemachine.m_BoundingShape2D = collider;
    }
    public void CameraPlayerSet()
    {
        playerCamera.transform.position = 
            new Vector3(gameManager.player.transform.position.x,gameManager.player.transform.position.y,playerCamera.transform.position.z);
    }

    //public void SpecialMapCamera(Map specialMap)
    //{
    //    switch (specialMap.mapName)
    //    {
    //        case "���� �ɼ�":
    //            //���ڸ� �����ϸ�� �⺻ 6f
    //            cinemachineVir.m_Lens.OrthographicSize = chargeCamera;
    //            break;
    //        case "Window ��ȭ��":
    //            //���ڸ� �����ϸ�� �⺻ 6f
    //            cinemachineVir.m_Lens.OrthographicSize = guardCamera;
    //            break;
    //        case "JuvaCafe":
    //            //���ڸ� �����ϸ�� �⺻ 6f
    //            cinemachineVir.m_Lens.OrthographicSize = cafeCamera;
    //            break;
    //        case "������":
    //            //���ڸ� �����ϸ�� �⺻ 6f
    //            cinemachineVir.m_Lens.OrthographicSize = trashCamera;
    //            break;
    //        case "Download":
    //            //���ڸ� �����ϸ�� �⺻ 6f
    //            cinemachineVir.m_Lens.OrthographicSize = downloadCamera;
    //            break;
    //        case "Shop":
    //            //���ڸ� �����ϸ�� �⺻ 6f
    //            cinemachineVir.m_Lens.OrthographicSize = shopCamera;
    //            break;
    //    }
    //}
}

