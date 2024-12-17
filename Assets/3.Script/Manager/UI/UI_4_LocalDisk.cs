using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_4_LocalDisk : MonoBehaviour
{
    private static UI_4_LocalDisk instance = null;

    // UI Window
    public GameObject UI_W_LocalDisk = null;

    //�ּҰ���
    // public List<Map> adressList = new();
    [SerializeField]
    GameObject adressParent;
    [SerializeField]
    Adress_Button adressButton;
    public Text Address;

    // �ű� 
    [Header("UI References")]
    public RectTransform content;       // Ʈ�� UI�� Content �׷�
    public GameObject FoldPrefab;       // �븻 ���� ��� ������
    public GameObject DownloadPrefab;   // �ٿ�ε� ���� ��� ������
    public GameObject ShopPrefab;       // ���� ���� ��� ������
    public GameObject BossPrefab;       // ���� ���� ��� ������
    public RectTransform linePrefab;    // ������ ����� ������
    public float xOffset = 200f;        // X�� ����
    public float yOffset = 100f;        // Y�� ����

    private Dictionary<int, int> depthNodeCount; // �� Depth�� ��� ������ ����

    public static UI_4_LocalDisk Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UI_4_LocalDisk>();

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(UI_4_LocalDisk).Name);
                    instance = singletonObject.AddComponent<UI_4_LocalDisk>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenUI()
    {
        if (UI_W_LocalDisk != null)
        {
            UI_W_LocalDisk.SetActive(true);
            // Debug.Log("OpenUI : UI_4_LocalDisk");
        }
    }

    public void CloseUI()
    {
        if (UI_W_LocalDisk != null)
        {
            UI_W_LocalDisk.SetActive(false);
            // Debug.Log("CloseUI : UI_4_LocalDisk");
        }
    }

    public void GenerateTreeUI(List<FolderGenerator.TreeNodeData> nodes)
    {
        if (nodes == null || nodes.Count == 0)
        {
            Debug.LogError("No nodes available for generating the UI.");
            return;
        }

        // ���� UI �ʱ�ȭ
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // Depth�� ��� ���� ���
        CalculateDepthNodeCount(nodes);

        // ��Ʈ ������ ����
        var rootNode = nodes.Find(node => node.ParentId == null);
        if (rootNode == null)
        {
            Debug.LogError("Root node not found.");
            return;
        }

        // ��������� UI ���� ����
        GenerateNodeUI(rootNode, nodes, null, 0, 0);
    }

    // Depth�� ��� ������ ����ϴ� �Լ�
    private void CalculateDepthNodeCount(List<FolderGenerator.TreeNodeData> nodes)
    {
        depthNodeCount = new Dictionary<int, int>();

        foreach (var node in nodes)
        {
            if (!depthNodeCount.ContainsKey(node.Depth))
            {
                depthNodeCount[node.Depth] = 0;
            }
            depthNodeCount[node.Depth]++;
        }
    }

    private void GenerateNodeUI(FolderGenerator.TreeNodeData node, List<FolderGenerator.TreeNodeData> nodes,
                                RectTransform parentUI, int depth, int siblingIndex)
    {
        // ���� ��� UI ����
        GameObject newNodeUI;

        // ��� Ÿ�Կ� ���� ������ ����
        switch (node.Type)
        {
            case FolderNode.FolderType.Download:
                newNodeUI = Instantiate(DownloadPrefab, content);
                break;
            case FolderNode.FolderType.Shop:
                newNodeUI = Instantiate(ShopPrefab, content);
                break;
            case FolderNode.FolderType.Boss:
                newNodeUI = Instantiate(BossPrefab, content);
                break;
            default:
                newNodeUI = Instantiate(FoldPrefab, content);
                break;
        }

        RectTransform rectTransform = newNodeUI.GetComponent<RectTransform>();
        rectTransform.name = $"Node_{node.Id}";

        // X�� ��ġ ���
        float xPos = depth * xOffset;

        // Y�� ��ġ ���: Depth�� ��� ������ ���� �߾ӿ��� �յ� �л�
        int totalNodesAtDepth = depthNodeCount[depth];
        float ySpacing = yOffset; // ��� �� ����
        float yStart = -(totalNodesAtDepth - 1) * ySpacing / 2; // �߾� ���� ������

        float yPos = yStart + siblingIndex * ySpacing;

        rectTransform.anchoredPosition = new Vector2(xPos, yPos);

        // ��� ���� ����
        Text nodeText = newNodeUI.GetComponentInChildren<Text>();
        if (nodeText != null)
        {
            nodeText.text = $"ID: {node.Id}";
        }

        // �θ�� �ڽ� ���̿� �� �׸���
        if (parentUI != null)
        {
            DrawLine(parentUI, rectTransform);
        }

        // �ڽ� ��� ��ȸ
        int childIndex = 0;
        foreach (var childId in node.Children)
        {
            var childNode = nodes.Find(n => n.Id == childId);
            if (childNode != null)
            {
                GenerateNodeUI(childNode, nodes, rectTransform, depth + 1, childIndex);
                childIndex++;
            }
        }
    }
    private void DrawLine(RectTransform startNode, RectTransform endNode)
    {
        // �� ������ ����
        RectTransform line = Instantiate(linePrefab, content);
        line.name = "Line";

        Vector2 startPosition = startNode.anchoredPosition;
        Vector2 endPosition = endNode.anchoredPosition;

        // �� ���̿� ���� ����
        float distance = Vector2.Distance(startPosition, endPosition);
        float angle = Mathf.Atan2(endPosition.y - startPosition.y, endPosition.x - startPosition.x) * Mathf.Rad2Deg;

        line.sizeDelta = new Vector2(distance, 2f); // �� �β�
        line.anchoredPosition = (startPosition + endPosition) / 2; // ���� �߽�
        line.localRotation = Quaternion.Euler(0, 0, angle); // �� ȸ��
    }





    // �Ʒ��� ���� �����̰� �ۼ��� �ڵ���
    IEnumerator LayoutReset(RectTransform obj)
    {
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(obj);

    }

    public void AdressReset()
    {
        for (int i = adressParent.transform.childCount - 1; i > 0; i--) // 0��°�� �����ϰ� ��������
        {
            Destroy(adressParent.transform.GetChild(i).gameObject);
        }
        // adressList.Clear();
    }

    public void SetUIAdress(UIManager.UI uiType)
    {
        switch (uiType)
        {
            case UIManager.UI.UI_MyPC:
                Address.text = "�� PC";
                break;
            case UIManager.UI.UI_DownLoad:
                Address.text = "�ٿ�ε�";
                break;
            case UIManager.UI.UI_MyDocument:
                Address.text = "�� ����";
                break;
            case UIManager.UI.UI_LocalDisk:
                Address.text = "���� ��ũ";
                break;
            case UIManager.UI.UI_Control:
                Address.text = "������";
                break;
            case UIManager.UI.UI_Help:
                Address.text = "����";
                break;
        }
    }
}
