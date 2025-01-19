using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance; // �̱��� �ν��Ͻ�

    public CinemachineVirtualCamera primaryCamera;
    [SerializeField] private CinemachineVirtualCamera currentCamera;
    [SerializeField] private CinemachineConfiner2D confiner2D;
    [SerializeField] private Player playerController;

    private FolderManager folderManager;


    #region Lagacy Code
    /*
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
    */
    #endregion

    #region Default Function

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
        // gameManager = GameManager.Instance;
        currentCamera = primaryCamera;
        folderManager = FolderManager.Instance;
    }

    #endregion


    public void switcherCamera(CinemachineVirtualCamera newCamera)
    {
        currentCamera.gameObject.SetActive(false);
        newCamera.gameObject.SetActive(true);
        currentCamera = newCamera;
        currentCamera.Follow = playerController.transform;
    }

    public void switchPrimaryCamera()
    {
        currentCamera.gameObject.SetActive(false );
        primaryCamera.gameObject.SetActive(true);
        currentCamera = primaryCamera;
    }

    public void SetCollider()
    {
        Collider2D curCollider = folderManager?.CurrentFolder.GetComponent<Collider2D>();

        if (curCollider == null)
        {
            Debug.LogWarning("CurrentFolder�� Collider2D�� �����ϴ�.");
            return;
        }

        if (confiner2D != null)
        {
            confiner2D.m_BoundingShape2D = curCollider;
            Debug.Log("Collider�� ���������� �Ҵ�Ǿ����ϴ�.");
        }
        else
        {
            Debug.LogError("confiner2D is null");
        }
    }

}

