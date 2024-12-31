using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_4_LocalDisk : MonoBehaviour
{
    private static UI_4_LocalDisk instance = null;

    // UI Window
    public GameObject UI_W_LocalDisk = null;

    // Manager
    private FolderManager folderManager;
    public Player player;

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
    public RectTransform linePrefab;    // ������ ����� ������


    [Header("Base Folder Prefab")]
    public GameObject FoldPrefab;       // �븻 ���� ��� ������
    public GameObject FoldHiddenPrefab;       // �븻 ���� ��� ������
    public GameObject BossPrefab;       // ���� ���� ��� ������

    [Header("Shop, Download Prefab")]
    public GameObject DownloadPrefab;   // �ٿ�ε� ���� ��� ������
    public GameObject ShopPrefab;       // ���� ���� ��� ������

    [Header("Random Special Prefab")]
    public GameObject ChargeRoomPrefab; // ���� ���� ��� ������
    public GameObject GuardRoomPrefab;  // ���� ���� ��� ������
    public GameObject JuvaCafePrefab;   // ���� ���� ��� ������
    public GameObject TrashRoomPrefab;  // ���� ���� ��� ������


    public float ySpacing = 100f; // Y�� ����
    public float xSpacing = 200f; // X�� ����

    private Dictionary<int, List<FolderNode>> depthNodes;   // Depth�� ��� ����
    private Dictionary<int, RectTransform> nodeUIMap;       // ��� ID�� UI RectTransform ����
    private Dictionary<int, List<GameObject>> linesMap;     // �� ��忡 ����� ���� ����


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
        folderManager = FolderManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenUI()
    {
        Debug.Log("OpenUI");
        if (UI_W_LocalDisk != null)
        {
            UI_W_LocalDisk.SetActive(true);
            UpdateNodeUIStates();
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

    public void GenerateTreeUI(FolderNode rootFolder)
    {
        if (rootFolder == null)
        {
            Debug.LogError("Root folder is null. Cannot generate UI.");
            return;
        }

        // ���� UI �ʱ�ȭ
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // �ʱ�ȭ
        depthNodes = new Dictionary<int, List<FolderNode>>();
        nodeUIMap = new Dictionary<int, RectTransform>();
        linesMap = new Dictionary<int, List<GameObject>>();

        // Depth���� ��� ���� (���� �켱 Ž��)
        CollectNodesByDepth(rootFolder, 0);

        // Depth�� ��� UI ����
        GenerateUIFromDepthNodes();
    }

    private void CollectNodesByDepth(FolderNode node, int depth)
    {
        if (!depthNodes.ContainsKey(depth))
        {
            depthNodes[depth] = new List<FolderNode>();
        }

        // ���� Depth�� ��� �߰�
        depthNodes[depth].Add(node);

        // �ڽ� ��� ��ȸ
        foreach (var child in node.Children)
        {
            CollectNodesByDepth(child, depth + 1);
        }
    }

    private void GenerateUIFromDepthNodes()
    {
        foreach (var depth in depthNodes.Keys)
        {
            List<FolderNode> currentDepthNodes = depthNodes[depth];

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
                    case FolderNode.FolderType.RandomSpecial:
                        string name = node.CurrentFolder.name;
                        if (name == "Charge_room(Clone)")
                            newNodeUI = Instantiate(ChargeRoomPrefab, content);
                        else if (name == "Guard_room(Clone)")
                            newNodeUI = Instantiate(GuardRoomPrefab, content);
                        else if (name == "Juva_cafe(Clone)")
                            newNodeUI = Instantiate(JuvaCafePrefab, content);
                        else if (name == "Trash_room(Clone)")
                            newNodeUI = Instantiate(TrashRoomPrefab, content);
                        else
                        {
                            Debug.LogError("Could not found Room Type");
                            return;
                        }
                        break;
                    default:
                        newNodeUI = Instantiate(FoldPrefab, content);
                        break;
                }

                RectTransform rectTransform = newNodeUI.GetComponent<RectTransform>();
                rectTransform.name = $"Node_{node.FolderName}";

                // X�� ��ġ: Depth�� ���� ���������� �̵�
                float xPos = depth * xSpacing;

                // Y�� ��ġ: �߾� �������� ���Ʒ��� �յ��ϰ� �л�
                float yPos = yStart - i * ySpacing;

                rectTransform.anchoredPosition = new Vector2(xPos, yPos);

                // ��� ���� ����
                Text nodeText = newNodeUI.GetComponentInChildren<Text>();
                if (nodeText != null)
                {
                    nodeText.text = node.FolderName;
                }

                // ��� UI ����
                nodeUIMap[node.GetInstanceID()] = rectTransform;

                // �θ�� ����Ǵ� �� �׸���
                if (node.Parent != null && nodeUIMap.ContainsKey(node.Parent.GetInstanceID()))
                {
                    RectTransform parentUI = nodeUIMap[node.Parent.GetInstanceID()];
                    DrawLine(node.Parent, node, parentUI, rectTransform);
                }

                // ��ư �̺�Ʈ 
                Button button = newNodeUI.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.AddListener(() => OnNodeButtonClicked(node));
                }
            }
        }
    }

    private void DrawLine(FolderNode startNode, FolderNode endNode, RectTransform startNodeUI, RectTransform endNodeUI)
    {
        // ������ ���� �� �׷� �ֻ�ܿ� ��ġ��Ŵ.
        RectTransform line = Instantiate(linePrefab, content);
        line.transform.SetSiblingIndex(0);
        line.name = "Line";

        Vector2 startPosition = startNodeUI.anchoredPosition;
        Vector2 endPosition = endNodeUI.anchoredPosition;

        float distance = Vector2.Distance(startPosition, endPosition);
        float angle = Mathf.Atan2(endPosition.y - startPosition.y, endPosition.x - startPosition.x) * Mathf.Rad2Deg;

        line.sizeDelta = new Vector2(distance, 2f); // �� �β�
        line.anchoredPosition = (startPosition + endPosition) / 2; // ���� �߽�
        line.localRotation = Quaternion.Euler(0, 0, angle); // �� ȸ��
        line.gameObject.SetActive(false);

        // ���� linesMap�� ����
        int startNodeID = startNode.GetInstanceID();
        int endNodeID = endNode.GetInstanceID();

        if (!linesMap.ContainsKey(startNodeID))
        {
            linesMap[startNodeID] = new List<GameObject>();
        }
        linesMap[startNodeID].Add(line.gameObject);

        if (!linesMap.ContainsKey(endNodeID))
        {
            linesMap[endNodeID] = new List<GameObject>();
        }
        linesMap[endNodeID].Add(line.gameObject);
    }

    private void OnNodeButtonClicked(FolderNode node)
    {
        if (node == null)
        {
            Debug.LogError("Folder is null.");
            return;
        }

        // Ŭ���� ���� Ȯ��
        if (!node.IsCleared)
        {
            Debug.Log($"Folder {node.FolderName} is not cleared.");
            return;
        }

        // �̵� ���� ����
        Debug.Log($"Moving to Folder {node.FolderName}");
        folderManager.MoveToFolder(node);

        Transform teleportPoint = node.transform.Find("TeleportPoint");
        if (teleportPoint != null && player != null)
        {
            player.transform.position = teleportPoint.position;
            Debug.Log($"Player moved to {node.FolderName} at {teleportPoint.position}");
        }
        else
        {
            Debug.LogWarning("TeleportPoint not found or Player is null.");
        }
        
        // ���� ��Ż�� ��� Ȱ��ȭ
        folderManager.ResetCurrentPortal();
    }



    public void UpdateNodeUIStates()
    {
        Debug.Log("UpdateNodeUIStates");
        foreach (var nodePair in nodeUIMap)
        {
            int nodeID = nodePair.Key;
            RectTransform nodeUI = nodePair.Value;

            // FolderNode�� �������� ���� ID�� ������� ã��
            FolderNode node = depthNodes.Values
                .SelectMany(list => list)
                .FirstOrDefault(n => n.GetInstanceID() == nodeID);

            if (node == null)
            {
                Debug.LogError($"Node with ID {nodeID} not found.");
                continue;
            }

            GameObject nodeGameObject = nodeUI.gameObject; 
            Image imageComponent = nodeGameObject.GetComponent<Image>();

            if (imageComponent == null)
            {
                Debug.LogError("Image Component is not found");
                return;
            }
            // Ž������ ���� ��� UI ��Ȱ��ȭ
            if (node.isDetectionDone == false)
            {
                nodeGameObject.SetActive(false);
            }
            // �߰� O + Ŭ���� X : ������ ��Ӱ� ó��
            else if (node.isDetectionDone == true && node.IsCleared == false)
            {
                Debug.Log("�߰� O + Ŭ���� X");
                nodeGameObject.SetActive(true);
                imageComponent.color = new Color(0.3f, 0.3f, 0.3f, 0.6f);
            }
            // �߰� O + Ŭ���� O : ���� ����, �� Ȱ��ȭ
            else if (node.isDetectionDone == true && node.IsCleared == true)
            {
                Debug.Log("�߰� O + Ŭ���� O");
                nodeGameObject.SetActive(true);
                imageComponent.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                if (linesMap.ContainsKey(nodeID))
                {
                    foreach (GameObject line in linesMap[nodeID])
                    {
                        line.SetActive(true);
                    }
                }
            }
            else
            {
                Debug.LogError("�̻��� ���°� ���ߵ�");
            }
            
        }
    }


    // �Ʒ��� ���� �����̰� �ۼ��� �ڵ���

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
