using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_4_LocalDisk : MonoBehaviour
{
    private static UI_4_LocalDisk instance = null;

    // UI Window
    public GameObject UI_W_LocalDisk = null;

    //주소관련
    // public List<Map> adressList = new();
    [SerializeField]
    GameObject adressParent;
    [SerializeField]
    Adress_Button adressButton;
    public Text Address;

    // 신규 
    [Header("UI References")]
    public RectTransform content;       // 트리 UI의 Content 그룹
    public GameObject FoldPrefab;       // 노말 폴더 노드 프리팹
    public GameObject DownloadPrefab;   // 다운로드 폴더 노드 프리팹
    public GameObject ShopPrefab;       // 상점 폴더 노드 프리팹
    public GameObject BossPrefab;       // 보스 폴더 노드 프리팹
    public RectTransform linePrefab;    // 선으로 사용할 프리팹
    public float xOffset = 200f;        // X축 간격
    public float yOffset = 100f;        // Y축 간격

    private Dictionary<int, int> depthNodeCount; // 각 Depth의 노드 개수를 저장

    private Dictionary<int, List<FolderGenerator.TreeNodeData>> depthNodes; // Depth별 노드 저장
    private Dictionary<int, RectTransform> nodeUIMap; // 노드 ID와 UI RectTransform 매핑


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

        // 기존 UI 초기화
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // 초기화
        depthNodes = new Dictionary<int, List<FolderGenerator.TreeNodeData>>();
        nodeUIMap = new Dictionary<int, RectTransform>();

        // 깊이 우선 탐색을 통해 Depth별 노드 수집
        var rootNode = nodes.Find(node => node.ParentId == null);
        if (rootNode == null)
        {
            Debug.LogError("Root node not found.");
            return;
        }

        CollectNodesByDepth(rootNode, nodes, 0);

        // Depth별 노드 UI 생성
        GenerateUIFromDepthNodes(nodes);
    }

    // 깊이 우선 탐색: Depth별 노드를 수집
    private void CollectNodesByDepth(FolderGenerator.TreeNodeData node, List<FolderGenerator.TreeNodeData> nodes, int depth)
    {
        if (!depthNodes.ContainsKey(depth))
        {
            depthNodes[depth] = new List<FolderGenerator.TreeNodeData>();
        }

        depthNodes[depth].Add(node);

        // 자식 노드 탐색
        foreach (var childId in node.Children)
        {
            var childNode = nodes.Find(n => n.Id == childId);
            if (childNode != null)
            {
                CollectNodesByDepth(childNode, nodes, depth + 1);
            }
        }
    }

    // Depth별 노드를 UI로 생성
    private void GenerateUIFromDepthNodes(List<FolderGenerator.TreeNodeData> nodes)
    {
        float ySpacing = 100f; // Y축 간격
        float xSpacing = 200f; // X축 간격

        foreach (var depth in depthNodes.Keys)
        {
            List<FolderGenerator.TreeNodeData> currentDepthNodes = depthNodes[depth];

            // Y축 위치를 균등하게 배치하기 위한 시작점
            float yStart = (currentDepthNodes.Count - 1) * ySpacing / 2;

            for (int i = 0; i < currentDepthNodes.Count; i++)
            {
                var node = currentDepthNodes[i];

                // 현재 노드 UI 생성
                GameObject newNodeUI;

                // 노드 타입에 따라 프리팹 설정
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

                // X축 위치: Depth에 따라 오른쪽으로 이동
                float xPos = depth * xSpacing;

                // Y축 위치: 중앙 기준으로 위아래로 균등하게 분산
                float yPos = yStart - i * ySpacing;

                rectTransform.anchoredPosition = new Vector2(xPos, yPos);

                // 노드 정보 설정
                Text nodeText = newNodeUI.GetComponentInChildren<Text>();
                if (nodeText != null)
                {
                    nodeText.text = $"ID: {node.Id}";
                }

                // 노드 UI 저장
                nodeUIMap[node.Id] = rectTransform;

                // 부모와 연결되는 선 그리기
                if (node.ParentId != null)
                {
                    RectTransform parentUI = nodeUIMap[node.ParentId.Value];
                    DrawLine(parentUI, rectTransform);
                }
            }
        }
    }

    // 부모와 자식 사이 선 그리기
    private void DrawLine(RectTransform startNode, RectTransform endNode)
    {
        RectTransform line = Instantiate(linePrefab, content);
        line.name = "Line";

        Vector2 startPosition = startNode.anchoredPosition;
        Vector2 endPosition = endNode.anchoredPosition;

        float distance = Vector2.Distance(startPosition, endPosition);
        float angle = Mathf.Atan2(endPosition.y - startPosition.y, endPosition.x - startPosition.x) * Mathf.Rad2Deg;

        line.sizeDelta = new Vector2(distance, 2f); // 선 두께
        line.anchoredPosition = (startPosition + endPosition) / 2; // 선의 중심
        line.localRotation = Quaternion.Euler(0, 0, angle); // 선 회전
    }



    // 아래는 기존 동근이가 작성한 코드임
    IEnumerator LayoutReset(RectTransform obj)
    {
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(obj);

    }

    public void AdressReset()
    {
        for (int i = adressParent.transform.childCount - 1; i > 0; i--) // 0번째는 제외하고 역순으로
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
                Address.text = "내 PC";
                break;
            case UIManager.UI.UI_DownLoad:
                Address.text = "다운로드";
                break;
            case UIManager.UI.UI_MyDocument:
                Address.text = "내 문서";
                break;
            case UIManager.UI.UI_LocalDisk:
                Address.text = "로컬 디스크";
                break;
            case UIManager.UI.UI_Control:
                Address.text = "제어판";
                break;
            case UIManager.UI.UI_Help:
                Address.text = "도움말";
                break;
        }
    }
}
