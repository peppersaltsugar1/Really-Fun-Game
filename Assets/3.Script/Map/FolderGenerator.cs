using System;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Metadata;

public class FolderGenerator : MonoBehaviour
{

    [Header("MapCount")]
    public int GenerateMapCount;
    private int MaxFolderCount;

    [Header("Folder Prefab Lists")]
    public List<GameObject> StartFolderPrefabs1; // ������ ��Ż 1�� ��ŸƮ��
    public List<GameObject> StartFolderPrefabs2; // ������ ��Ż 2�� ��ŸƮ��
    public List<GameObject> StartFolderPrefabs3; // ������ ��Ż 3�� ��ŸƮ��
    public List<GameObject> NormalFolderPrefabs1; // ���ʿ� ��Ż�� 1��, �����ʿ� ��Ż�� 1���� ���� ����Ʈ
    public List<GameObject> NormalFolderPrefabs2; // ���ʿ� ��Ż�� 1��, �����ʿ� ��Ż�� 2���� ���� ����Ʈ
    public List<GameObject> NormalFolderPrefabs3; // ���ʿ� ��Ż�� 1��, �����ʿ� ��Ż�� 3���� ���� ����Ʈ
    public List<GameObject> DownloadFolderPrefabs;
    public List<GameObject> ShopFolderPrefabs;
    public List<GameObject> BossFolderPrefabs;
    public List<GameObject> SpecialFolderPrefabs;
    public List<GameObject> EndFolderPrefabs;

    private List<FolderNode> spawnedFolders = new List<FolderNode>();
    private FolderNode rootFolder;

    [Header("Spacing Settings")]
    public float HorizontalSpacing;
    public float VerticalSpacing;

    public List<TreeNodeData> TreeTemplete;

    public void GenerateMap()
    {
        if (GenerateMapCount < 6)
        {
            Debug.LogError("MaxFolderCount must be at least 6 to ensure proper map generation.");
            return;
        }

        // 2(�ٿ�ε�, ����) + 1(������) + 2(�����) = 5���� ���� ���Ѿ� ��.(���� ���� ���ԵǾ� ����)
        MaxFolderCount = GenerateMapCount - 5;

        // ���� ���� Ʈ�� �ڷ� ������ ����
        TreeTemplete = GenerateTreeData(MaxFolderCount);

        // ���� ���� ����
        rootFolder = CreateStartMap();

        // �߰� �κ� ���� ����
        if (rootFolder != null)
        {
            TreeNodeData rootNode = TreeTemplete.Find(node => node.ParentId == null);
            CreateMiddleMaps(rootFolder, rootNode, Vector3.zero);
        }

        // Step 4: Connect Portals
        ConnectPortals(rootFolder);

        //Debug.Log("Map generation complete.");
    }


    // Ʈ���� ���� ������ ���� ���� ������Ű�� �Լ�
    private FolderNode CreateStartMap()
    {
        // 1. ��Ʈ ��� �ĺ�
        TreeNodeData rootNode = TreeTemplete.Find(node => node.ParentId == null);
        if (rootNode == null)
        {
            Debug.LogError("Root node not found in TreeTemplete.");
            return null;
        }

        // 2. �ڽ� ������ �´� ���� �� ������ ����
        int childCount = rootNode.ChildCount;
        GameObject startPrefab = null;

        switch (childCount)
        {
            case 1:
                startPrefab = StartFolderPrefabs1[UnityEngine.Random.Range(0, StartFolderPrefabs1.Count)];
                break;
            case 2:
                startPrefab = StartFolderPrefabs2[UnityEngine.Random.Range(0, StartFolderPrefabs2.Count)];
                break;
            case 3:
                startPrefab = StartFolderPrefabs3[UnityEngine.Random.Range(0, StartFolderPrefabs3.Count)];
                break;
            default:
                Debug.LogError($"No start prefab found for child count: {childCount}");
                break;
        }

        if (startPrefab == null)
        {
            Debug.LogError("startPrefab is null.");
            return null;
        }

        // 3. ���� �� ����
        GameObject startMap = Instantiate(startPrefab, Vector3.zero, Quaternion.identity);
        startMap.transform.SetParent(this.transform);

        // 4. FolderNode ��ũ��Ʈ ã��
        FolderNode folderNode = startMap.GetComponent<FolderNode>();

        if (folderNode == null)
        {
            Debug.LogError("The selected start map prefab does not contain a FolderNode component.");
            return null;
        }

        // FolderNode�� ���������� ã�� ���
        Debug.Log("FolderNode component found successfully.");

        return folderNode;
    }

    // Ʈ���� ���� ������ �߰�, ����, ���� ���� ������Ű�� �Լ�
    private void CreateMiddleMaps(FolderNode parentFolder, TreeNodeData parentNode, Vector3 parentPosition)
    {
        // 1. �ڽ� ��� Ž��
        foreach (int childId in parentNode.Children)
        {
            // �ڽ� ��� ������ ��������
            TreeNodeData childNode = TreeTemplete.Find(node => node.Id == childId);
            if (childNode == null)
            {
                Debug.LogError($"Child node with ID {childId} not found in TreeTemplete.");
                continue;
            }

            // �ڽ��� �ڽ� ��� �� Ȯ��
            int grandChildCount = childNode.ChildCount;

            // 2. ������ ����
            GameObject middlePrefab = GetPrefabForFolderType(childNode.Type, grandChildCount);
            if (middlePrefab == null)
            {
                Debug.LogError($"No prefab found for FolderType {childNode.Type}.");
                continue;
            }

            // 3. ��ġ ���
            Vector3 childPosition = parentPosition + new Vector3(HorizontalSpacing, 0, 0);

            // 4. �� ����
            GameObject middleMap = Instantiate(middlePrefab, childPosition, Quaternion.identity);
            middleMap.transform.SetParent(this.transform);

            // 5. FolderNode ����
            FolderNode folderNode = middleMap.GetComponent<FolderNode>();
            if (folderNode == null)
            {
                Debug.LogError("The selected map prefab does not contain a FolderNode component.");
                continue;
            }

            folderNode.Children = new List<FolderNode>();

            // �θ�� �ڽ� ���� ����
            parentFolder.AddChild(folderNode);

            // 6. ��� ȣ�� (���� ���� ����)
            CreateMiddleMaps(folderNode, childNode, childPosition);
        }
    }

    public FolderNode GetRootNode() { return rootFolder; }

    private FolderNode InstantiateNode(FolderNode node, Vector2 position, FolderNode parent, GameObject customPrefab = null)
    {
        // ���õ� ������ ���
        GameObject prefab = customPrefab ?? GetPrefabForFolderType(node.Type);
        GameObject folderObject = Instantiate(prefab, position, Quaternion.identity);
        FolderNode instantiatedNode = folderObject.GetComponent<FolderNode>();

        instantiatedNode.Type = node.Type;
        instantiatedNode.FolderName = node.FolderName;

        if (parent != null)
        {
            parent.AddChild(instantiatedNode);
        }

        spawnedFolders.Add(instantiatedNode);

        float childOffset = -VerticalSpacing;
        foreach (var child in node.Children)
        {
            Vector2 childPosition = position + new Vector2(HorizontalSpacing, childOffset);
            InstantiateNode(child, childPosition, instantiatedNode);
            childOffset -= VerticalSpacing;
        }

        return instantiatedNode;
    }

    private GameObject GetPrefabForFolderType(FolderNode.FolderType type, int childCount = 0)
    {
        switch (type)
        {
            case FolderNode.FolderType.Middle:
                if (childCount == 1)
                    return NormalFolderPrefabs1[UnityEngine.Random.Range(0, NormalFolderPrefabs1.Count)];
                else if (childCount == 2)
                    return NormalFolderPrefabs2[UnityEngine.Random.Range(0, NormalFolderPrefabs2.Count)];
                else if (childCount == 3)
                    return NormalFolderPrefabs3[UnityEngine.Random.Range(0, NormalFolderPrefabs3.Count)];
                else
                    return null;
            case FolderNode.FolderType.Download:
                return DownloadFolderPrefabs[UnityEngine.Random.Range(0, DownloadFolderPrefabs.Count)];

            case FolderNode.FolderType.Shop:
                return ShopFolderPrefabs[UnityEngine.Random.Range(0, ShopFolderPrefabs.Count)];

            case FolderNode.FolderType.Boss:
                return BossFolderPrefabs[UnityEngine.Random.Range(0, BossFolderPrefabs.Count)];

            case FolderNode.FolderType.End:
                return EndFolderPrefabs[UnityEngine.Random.Range(0, EndFolderPrefabs.Count)];

            case FolderNode.FolderType.RandomSpecial:
                if (SpecialFolderPrefabs.Count == 0) // ����Ʈ ����� ��� ����ó��
                {
                    Debug.LogError("SpecialFolderPrefabs list is empty!");
                    return null;
                }

                // ���� ���� �� �ߺ� ������ ���� ����Ʈ���� ����
                int randomIndex = UnityEngine.Random.Range(0, SpecialFolderPrefabs.Count);
                GameObject specialPrefab = SpecialFolderPrefabs[randomIndex];
                SpecialFolderPrefabs.RemoveAt(randomIndex);
                return specialPrefab;

            default:
                return null;
        }
    }

    // ������ ��Ż�� �������ִ� �Լ�
    private void ConnectPortals(FolderNode node)
    {
        if (node == null) return;

        if (node.Portals == null)
        {
            Debug.LogError($"Node {node.name} has null Portals");
            return;
        }

        if (node.Children == null)
        {
            Debug.LogError($"Node {node.name} has null Children");
            return;
        }

        for (int i = 0; i < node.Children.Count; i++)
        {
            if (node.Portals[i] == null)
            {
                Debug.LogError($"Node {node.name} has null Portal at index {i}");
                continue;
            }

            if (node.Children[i] == null)
            {
                Debug.LogError($"Node {node.name} has null Child at index {i}");
                continue;
            }

            // ���� �� -> �ڽ� �� ��Ż �̾��ֱ�
            node.Portals[i].SetConnectedFolder(node.Children[i], 0);

            // �ڽ� �� -> ���� �� ��Ż �̾��ֱ�
            node.Children[i].Left_Portal.SetConnectedFolder(node, i);
        }

        // ��������� �ڽ� ����� ��Ż ����
        foreach (FolderNode child in node.Children)
        {
            ConnectPortals(child);
        }
    }


    // Ʈ�� ����ü ������ ����ִ� Ŭ����
    public class TreeNodeData
    {
        public int Id { get; set; }               // ���� ID
        public int? ParentId { get; set; }        // �θ� ID (��Ʈ ���� null)
        public int ChildCount { get; set; }       // �ڽ� ����
        public List<int> Children { get; set; }   // �ڽ� ��� ID ���
        public FolderNode.FolderType Type { get; set; } // ��� Ÿ��
        public bool IsLeaf { get; set; }          // ���� ��� ����
        public int Depth { get; set; }            // Ʈ�� ����

        public TreeNodeData(int id, int? parentId, FolderNode.FolderType type, int depth)
        {
            Id = id;
            ParentId = parentId;
            Type = type;
            Depth = depth;
            Children = new List<int>();
            ChildCount = 0;
            IsLeaf = true; // �ʱ�ȭ �� �⺻������ ���� ���� ����
        }
    }

    private List<TreeNodeData> GenerateTreeData(int nodeCount)
    {
        System.Random random = new System.Random();

        var nodes = new List<TreeNodeData>();
        int nextId = 1;
        int leafCount = 0;

        // 1. ��Ʈ ��� ���� �� �ʱ�ȭ
        var root = new TreeNodeData(nextId++, null, FolderNode.FolderType.Start, 0);
        nodes.Add(root);
        // Debug.Log($"Root Node Created - ID: {root.Id}, ChildCount: {root.ChildCount}");

        var validParents = new List<TreeNodeData> { root };

        // 2.Ʈ�� ���� ����
        while (nextId <= nodeCount || leafCount < 5)
        {
            // �θ� ��� ����
            if (validParents.Count == 0)
            {
                Debug.LogWarning("No valid parents available. Tree generation terminated early.");
                break;
            }

            // �θ� ��� ����
            var parent = validParents[random.Next(validParents.Count)];

            //int maxChildren = Mathf.Min(3, nodeCount - nextId);
            int maxChildren = Mathf.Min(3 - parent.ChildCount, nodeCount - nextId);

            int childCount = random.Next(1, maxChildren + 1);

            // Debug.Log($"Parent Node ID: {parent.Id}, Adding {childCount} Children");

            for (int i = 0; i < childCount && nextId <= nodeCount; i++)
            {
                if (parent.ChildCount >= 3) break; // �ڽ��� �ִ� ������ �ʰ��ϸ� �ߴ�

                // �ڽ� ��� ����
                var child = new TreeNodeData(nextId++, parent.Id, FolderNode.FolderType.Middle , parent.Depth + 1);
                parent.Children.Add(child.Id);
                parent.ChildCount++;
                parent.IsLeaf = false; // �θ� �Ǿ����Ƿ� ���� ��尡 �ƴ�

                nodes.Add(child);
                validParents.Add(child);

                // Debug.Log($"Child Node Created - ID: {child.Id}, Parent ID: {parent.Id}");
            }

            // �θ� �ִ� �ڽ��� �����ٸ� ��ȿ �θ� ����Ʈ���� ����
            if (parent.ChildCount >= 3)
            {
                validParents.Remove(parent);
                // Debug.Log($"Parent Node ID: {parent.Id} removed from validParents (Max Children Reached)");
            }

            // ���� ��� ���� ����
            if (nextId > nodeCount && leafCount < 5)
            {
                foreach (var parentNode in validParents)
                {
                    var extraLeaf = new TreeNodeData(nextId++, parentNode.Id, FolderNode.FolderType.Hidden, parentNode.Depth + 1);
                    parentNode.Children.Add(extraLeaf.Id);
                    parentNode.ChildCount++;
                    nodes.Add(extraLeaf);
                    leafCount++;

                    // Debug.Log($"Extra Leaf Created - ID: {extraLeaf.Id}, Parent ID: {parentNode.Id}");

                    if (leafCount >= 5) break;
                }
            }
        }

        // 3. ��� Ÿ�� �缳��
        TreeNodeData bossNode = null;
        int maxDepth = -1; // ���� ���� ���� ��带 ã�� ���� ����
        List<TreeNodeData> leafNodes = new List<TreeNodeData>();

        foreach (var node in nodes)
        {
            if (node.Children.Count == 0 && node.ParentId != null) // ���� ���
            {
                leafNodes.Add(node);
                node.Type = FolderNode.FolderType.End;

                // ���� ���� ���� ��� ã��
                if (node.Depth > maxDepth)
                {
                    maxDepth = node.Depth;
                    bossNode = node;
                }
            }
            else if (node.ParentId != null) // �߰� ���
            {
                node.Type = FolderNode.FolderType.Middle;
            }
        }

        // 4. ���� ���� ���� ��� Ÿ���� Boss�� ����
        if (bossNode != null)
        {
            bossNode.Type = FolderNode.FolderType.Boss;
            leafNodes.Remove(bossNode);
            // Debug.Log($"Boss Node Assigned - ID: {bossNode.Id}, Depth: {bossNode.Depth}");
        }

        // 5. Ư�� Ÿ�� �� ����
        if (leafNodes.Count >= 4)
        {
            // �ٿ�ε� 1��
            var downloadNode = leafNodes[random.Next(leafNodes.Count)];
            downloadNode.Type = FolderNode.FolderType.Download;
            leafNodes.Remove(downloadNode);
            // Debug.Log($"Download Node Assigned - ID: {downloadNode.Id}");

            // ���� 1��
            var storeNode = leafNodes[random.Next(leafNodes.Count)];
            storeNode.Type = FolderNode.FolderType.Shop;
            leafNodes.Remove(storeNode);
            // Debug.Log($"Store Node Assigned - ID: {storeNode.Id}");

            // ����� 2��
            for (int i = 0; i < 2; i++)
            {
                var specialNode = leafNodes[random.Next(leafNodes.Count)];
                specialNode.Type = FolderNode.FolderType.RandomSpecial;
                leafNodes.Remove(specialNode);
                // Debug.Log($"Special Node Assigned - ID: {specialNode.Id}");
            }
        }

        // 6. �����: ���� Ʈ�� ���� ���
        //foreach (var node in nodes)
        //{
        //    Debug.Log($"Node ID: {node.Id}, Parent ID: {node.ParentId}, ChildCount: {node.ChildCount}, Depth: {node.Depth}, Type: {node.Type}");
        //}

        return nodes;
    }


}
