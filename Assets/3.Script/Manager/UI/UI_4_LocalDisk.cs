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

    private Dictionary<int, List<FolderGenerator.TreeNodeData>> depthNodes; // Depth�� ��� ����
    private Dictionary<int, RectTransform> nodeUIMap; // ��� ID�� UI RectTransform ����


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

        // �ʱ�ȭ
        depthNodes = new Dictionary<int, List<FolderGenerator.TreeNodeData>>();
        nodeUIMap = new Dictionary<int, RectTransform>();

        // ���� �켱 Ž���� ���� Depth�� ��� ����
        var rootNode = nodes.Find(node => node.ParentId == null);
        if (rootNode == null)
        {
            Debug.LogError("Root node not found.");
            return;
        }

        CollectNodesByDepth(rootNode, nodes, 0);

        // Depth�� ��� UI ����
        GenerateUIFromDepthNodes(nodes);
    }

    // ���� �켱 Ž��: Depth�� ��带 ����
    private void CollectNodesByDepth(FolderGenerator.TreeNodeData node, List<FolderGenerator.TreeNodeData> nodes, int depth)
    {
        if (!depthNodes.ContainsKey(depth))
        {
            depthNodes[depth] = new List<FolderGenerator.TreeNodeData>();
        }

        depthNodes[depth].Add(node);

        // �ڽ� ��� Ž��
        foreach (var childId in node.Children)
        {
            var childNode = nodes.Find(n => n.Id == childId);
            if (childNode != null)
            {
                CollectNodesByDepth(childNode, nodes, depth + 1);
            }
        }
    }

    // Depth�� ��带 UI�� ����
    private void GenerateUIFromDepthNodes(List<FolderGenerator.TreeNodeData> nodes)
    {
        float ySpacing = 100f; // Y�� ����
        float xSpacing = 200f; // X�� ����

        foreach (var depth in depthNodes.Keys)
        {
            List<FolderGenerator.TreeNodeData> currentDepthNodes = depthNodes[depth];

            // Y�� ��ġ�� �յ��ϰ� ��ġ�ϱ� ���� ������
            float yStart = (currentDepthNodes.Count - 1) * ySpacing / 2;

            for (int i = 0; i < currentDepthNodes.Count; i++)
            {
                var node = currentDepthNodes[i];

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

                // X�� ��ġ: Depth�� ���� ���������� �̵�
                float xPos = depth * xSpacing;

                // Y�� ��ġ: �߾� �������� ���Ʒ��� �յ��ϰ� �л�
                float yPos = yStart - i * ySpacing;

                rectTransform.anchoredPosition = new Vector2(xPos, yPos);

                // ��� ���� ����
                Text nodeText = newNodeUI.GetComponentInChildren<Text>();
                if (nodeText != null)
                {
                    nodeText.text = $"ID: {node.Id}";
                }

                // ��� UI ����
                nodeUIMap[node.Id] = rectTransform;

                // �θ�� ����Ǵ� �� �׸���
                if (node.ParentId != null)
                {
                    RectTransform parentUI = nodeUIMap[node.ParentId.Value];
                    DrawLine(parentUI, rectTransform);
                }
            }
        }
    }

    // �θ�� �ڽ� ���� �� �׸���
    private void DrawLine(RectTransform startNode, RectTransform endNode)
    {
        RectTransform line = Instantiate(linePrefab, content);
        line.name = "Line";

        Vector2 startPosition = startNode.anchoredPosition;
        Vector2 endPosition = endNode.anchoredPosition;

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
