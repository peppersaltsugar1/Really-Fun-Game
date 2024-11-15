using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_4_LocalDisk : MonoBehaviour
{
    private static UI_4_LocalDisk instance = null;

    // UI Window
    public GameObject UI_W_LocalDisk = null;

    // Detail
    [SerializeField]
    GameObject localDiskContent;
    [SerializeField]
    MapGenerator mapGenerator;
    //��Ż Ui����
    [SerializeField]
    GameObject UIPortal;
    [SerializeField]
    List<Sprite> portalUiList = new();
    [SerializeField]
    List<Sprite> closePortalUiList = new();
    [SerializeField]
    List<GameObject> ItemImageList = new();

    //�ּҰ���
    public List<Map> adressList = new();
    [SerializeField]
    GameObject adressParent;
    [SerializeField]
    Adress_Button adressButton;
    //
    public Text Address;
    // Manager

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

    // ================ Map Section ================
    public void RoomUISet()
    {
        //UI�� �����ִ¿�� Ȯ���� ����
        for (int i = localDiskContent.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = localDiskContent.transform.GetChild(i);

            if (child != null)
            {
                Destroy(child.gameObject); // �ڽ� ��ü ����
            }
        }
        //�� index�ʱ�ȭ
        int mapIndex = 0;
        //���� ���� ���° index���� Ȯ��
        for (int i = 0; i < mapGenerator.mapList.Count; i++)
        {
            Map map = mapGenerator.mapList[i];

            // ���� Ȱ��ȭ�� ������ Ȯ��
            if (map.transform.gameObject.activeSelf)
            {
                mapIndex = i;
                continue; // Ȱ��ȭ�� ���� �ε��� ��ȯ
            }
        }
        /*LocalDisk_UI.GetComponent<LocalDiskUI>().currentLocakDiskMapIndex = mapIndex;*/
        //��list���� �������� ������
        LocalDisckUISet(mapIndex);
    }

    public void LocalDisckUISet(int mapIndex)
    {
        AdressSet(mapIndex);
        for (int i = localDiskContent.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = localDiskContent.transform.GetChild(i);

            if (child != null)
            {
                Destroy(child.gameObject); // �ڽ� ��ü ����
            }
        }
        GameObject currentMap = mapGenerator.mapList[mapIndex].transform.gameObject;

        if (mapIndex == 0)
        {
            //0�������϶� �������� ��Ż�� �����ͼ� ui����
            foreach (Transform child in currentMap.transform)
            {
                Portal curretnportal = child.GetComponent<Portal>();
                if (curretnportal != null)
                {
                    GameObject portalUI = Instantiate(UIPortal);
                    portalUI.transform.SetParent(localDiskContent.transform);
                    portalUI.transform.SetAsLastSibling();
                    UI_W_LocalDisk.GetComponent<LocalDiskUI>().telMap = currentMap;
                    Text mapName = portalUI.GetComponentInChildren<Text>();
                    Image[] images = portalUI.GetComponentsInChildren<Image>(true);
                    Image portalImage = images[1];

                    if (curretnportal.connectPortal != null)
                    {
                        Map connectedMap = curretnportal.connectPortal.transform.parent.GetComponent<Map>();
                        // �� �̸��� �����մϴ�.
                        mapName.text = connectedMap.mapName;
                        // LocalDiskUIPortalPanel�� connectMap�� �����մϴ�.
                        portalUI.GetComponent<LocalDiskUIPortalPanel>().connectMap =
                            mapGenerator.mapList[mapGenerator.mapList.IndexOf(connectedMap)];

                        if (connectedMap != null)
                        {
                            // connectedMap�� Ŭ���� �Ǿ����� ���ο� ���� ��������Ʈ ����
                            if (connectedMap.isClear)
                            {
                                switch (connectedMap.Type)
                                {
                                    case Map.MapType.Middle:
                                        portalImage.sprite = portalUiList[0];
                                        continue;

                                    case Map.MapType.Boss:
                                        portalImage.sprite = portalUiList[1];
                                        continue;

                                    case Map.MapType.Download:
                                        portalImage.sprite = portalUiList[2];
                                        continue;
                                    case Map.MapType.Shop:
                                        portalImage.sprite = portalUiList[3];
                                        continue;
                                    case Map.MapType.RandomSpecial:
                                        switch (connectedMap.mapName)
                                        {
                                            case "������":
                                                portalImage.sprite = portalUiList[4];
                                                continue;
                                            case "���� �ɼ�":
                                                portalImage.sprite = portalUiList[5];
                                                continue;
                                            case "JuvaCafe":
                                                portalImage.sprite = portalUiList[6];
                                                continue;
                                            case "Window ��ȭ��":
                                                portalImage.sprite = portalUiList[7];
                                                continue;
                                        }
                                        continue;
                                }
                            }
                            else
                            {
                                Debug.Log("����");
                                switch (connectedMap.Type)
                                {
                                    case Map.MapType.Middle:
                                        portalImage.sprite = closePortalUiList[0];
                                        continue;
                                    case Map.MapType.Boss:
                                        portalImage.sprite = closePortalUiList[1];
                                        continue;
                                    case Map.MapType.Download:
                                        if (curretnportal.isLock)
                                        {
                                            portalImage.sprite = closePortalUiList[3];
                                        }
                                        else
                                        {
                                            portalImage.sprite = closePortalUiList[2];
                                        }
                                        continue;
                                    case Map.MapType.Shop:
                                        if (curretnportal.isLock)
                                        {
                                            portalImage.sprite = closePortalUiList[4];
                                        }
                                        else
                                        {
                                            portalImage.sprite = closePortalUiList[5];
                                        }
                                        continue;
                                    case Map.MapType.RandomSpecial:
                                        switch (connectedMap.mapName)
                                        {
                                            case "������":
                                                portalImage.sprite = closePortalUiList[6];
                                                continue;
                                            case "���� �ɼ�":
                                                portalImage.sprite = closePortalUiList[7];
                                                continue;
                                            case "JuvaCafe":
                                                portalImage.sprite = closePortalUiList[8];
                                                continue;
                                            case "Window ��ȭ��":
                                                portalImage.sprite = closePortalUiList[9];
                                                continue;
                                        }
                                        continue;
                                }
                            }
                        }
                    }
                }

            }
            //0��° ���϶� �������� �������� �����ͼ� ui����
            foreach (Transform child in currentMap.transform)
            {
                Item fildItem = child.GetComponent<Item>();
                if (fildItem != null)
                {
                    switch (fildItem.itemType)
                    {
                        case Item.ItemType.Coin1:
                            switch (fildItem.itemScore)
                            {
                                case 1:
                                    GameObject oneCoinUI = Instantiate(ItemImageList[0]);
                                    oneCoinUI.transform.SetParent(localDiskContent.transform);
                                    oneCoinUI.transform.SetAsLastSibling();
                                    continue;
                                case 5:
                                    GameObject fiveCoinUI = Instantiate(ItemImageList[1]);
                                    fiveCoinUI.transform.SetParent(localDiskContent.transform);
                                    fiveCoinUI.transform.SetAsLastSibling();
                                    continue;
                                case 10:
                                    GameObject tenCoinUI = Instantiate(ItemImageList[2]);
                                    tenCoinUI.transform.SetParent(localDiskContent.transform);
                                    tenCoinUI.transform.SetAsLastSibling();
                                    continue;
                                case 15:
                                    GameObject fifteenCoinUI = Instantiate(ItemImageList[3]);
                                    fifteenCoinUI.transform.SetParent(localDiskContent.transform);
                                    fifteenCoinUI.transform.SetAsLastSibling();
                                    continue;
                            }
                            continue;
                        case Item.ItemType.Heal:
                            GameObject healUI = Instantiate(ItemImageList[0]);
                            healUI.transform.SetParent(localDiskContent.transform);
                            healUI.transform.SetAsLastSibling();
                            continue;
                        case Item.ItemType.TemHp:
                            GameObject ItemUI = Instantiate(ItemImageList[0]);
                            ItemUI.transform.SetParent(localDiskContent.transform);
                            ItemUI.transform.SetAsLastSibling();
                            continue;
                        case Item.ItemType.Shiled:
                            GameObject shiledUI = Instantiate(ItemImageList[0]);
                            shiledUI.transform.SetParent(localDiskContent.transform);
                            shiledUI.transform.SetAsLastSibling();
                            continue;
                        case Item.ItemType.Spark:
                            GameObject sparkUI = Instantiate(ItemImageList[0]);
                            sparkUI.transform.SetParent(localDiskContent.transform);
                            sparkUI.transform.SetAsLastSibling();
                            continue;
                    }


                }

            }
        }
        else
        {
            List<GameObject> currentPortalList = new();
            foreach (Transform child in currentMap.transform)
            {
                Portal curretnportal = child.GetComponent<Portal>();
                if (curretnportal != null)
                {
                    GameObject portalUI = Instantiate(UIPortal);
                    portalUI.transform.SetParent(localDiskContent.transform);
                    portalUI.transform.SetAsLastSibling();
                    UI_W_LocalDisk.GetComponent<LocalDiskUI>().telMap = currentMap;
                    currentPortalList.Add(portalUI);
                    Text mapName = portalUI.GetComponentInChildren<Text>();
                    Image[] images = portalUI.GetComponentsInChildren<Image>(true);
                    Image portalImage = images[1];

                    if (curretnportal.connectPortal != null)
                    {
                        Map connectedMap = curretnportal.connectPortal.transform.parent.GetComponent<Map>();
                        mapName.text = connectedMap.mapName;
                        portalUI.GetComponent<LocalDiskUIPortalPanel>().connectMap =
                            mapGenerator.mapList[mapGenerator.mapList.IndexOf(connectedMap)];
                        if (connectedMap != null)
                        {
                            // connectedMap�� Ŭ���� �Ǿ����� ���ο� ���� ��������Ʈ ����
                            if (connectedMap.isClear)
                            {
                                switch (connectedMap.Type)
                                {
                                    case Map.MapType.Middle:
                                        portalImage.sprite = portalUiList[0];
                                        continue;

                                    case Map.MapType.Boss:
                                        portalImage.sprite = portalUiList[1];
                                        continue;

                                    case Map.MapType.Download:
                                        portalImage.sprite = portalUiList[2];
                                        continue;
                                    case Map.MapType.Shop:
                                        portalImage.sprite = portalUiList[3];
                                        continue;
                                    case Map.MapType.RandomSpecial:
                                        switch (connectedMap.mapName)
                                        {
                                            case "������":
                                                portalImage.sprite = portalUiList[4];
                                                continue;
                                            case "���� �ɼ�":
                                                portalImage.sprite = portalUiList[5];
                                                continue;
                                            case "JuvaCafe":
                                                portalImage.sprite = portalUiList[6];
                                                continue;
                                            case "Window ��ȭ��":
                                                portalImage.sprite = portalUiList[7];
                                                continue;
                                        }
                                        continue;
                                }
                            }
                            else
                            {
                                switch (connectedMap.Type)
                                {
                                    case Map.MapType.Middle:
                                        portalImage.sprite = closePortalUiList[0];
                                        continue;
                                    case Map.MapType.Boss:
                                        portalImage.sprite = closePortalUiList[1];
                                        continue;
                                    case Map.MapType.Download:
                                        if (curretnportal.isLock)
                                        {
                                            portalImage.sprite = closePortalUiList[3];
                                        }
                                        else
                                        {
                                            portalImage.sprite = closePortalUiList[2];
                                        }
                                        continue;
                                    case Map.MapType.Shop:
                                        if (curretnportal.isLock)
                                        {
                                            portalImage.sprite = closePortalUiList[4];
                                        }
                                        else
                                        {
                                            portalImage.sprite = closePortalUiList[5];
                                        }
                                        continue;
                                    case Map.MapType.RandomSpecial:
                                        switch (connectedMap.mapName)
                                        {
                                            case "������":
                                                portalImage.sprite = closePortalUiList[6];
                                                continue;
                                            case "���� �ɼ�":
                                                portalImage.sprite = closePortalUiList[7];
                                                continue;
                                            case "JuvaCafe":
                                                portalImage.sprite = closePortalUiList[8];
                                                continue;
                                            case "Window ��ȭ��":
                                                portalImage.sprite = closePortalUiList[9];
                                                continue;
                                        }
                                        continue;
                                }
                            }

                        }
                    }
                }
            }
            Destroy(currentPortalList[0].gameObject);
            currentPortalList.RemoveAt(0);
            foreach (Transform child in currentMap.transform)
            {
                Item fildItem = child.GetComponent<Item>();
                if (fildItem != null)
                {
                    switch (fildItem.itemType)
                    {
                        case Item.ItemType.Coin1:
                            switch (fildItem.itemScore)
                            {
                                case 1:
                                    GameObject oneCoinUI = Instantiate(ItemImageList[0]);
                                    oneCoinUI.transform.SetParent(localDiskContent.transform);
                                    oneCoinUI.transform.SetAsLastSibling();
                                    continue;
                                case 5:
                                    GameObject fiveCoinUI = Instantiate(ItemImageList[1]);
                                    fiveCoinUI.transform.SetParent(localDiskContent.transform);
                                    fiveCoinUI.transform.SetAsLastSibling();
                                    continue;
                                case 10:
                                    GameObject tenCoinUI = Instantiate(ItemImageList[2]);
                                    tenCoinUI.transform.SetParent(localDiskContent.transform);
                                    tenCoinUI.transform.SetAsLastSibling();
                                    continue;
                                case 15:
                                    GameObject fifteenCoinUI = Instantiate(ItemImageList[3]);
                                    fifteenCoinUI.transform.SetParent(localDiskContent.transform);
                                    fifteenCoinUI.transform.SetAsLastSibling();
                                    continue;
                            }
                            continue;
                        case Item.ItemType.Heal:
                            GameObject healUI = Instantiate(ItemImageList[0]);
                            healUI.transform.SetParent(localDiskContent.transform);
                            healUI.transform.SetAsLastSibling();
                            continue;
                        case Item.ItemType.TemHp:
                            GameObject ItemUI = Instantiate(ItemImageList[0]);
                            ItemUI.transform.SetParent(localDiskContent.transform);
                            ItemUI.transform.SetAsLastSibling();
                            continue;
                        case Item.ItemType.Shiled:
                            GameObject shiledUI = Instantiate(ItemImageList[0]);
                            shiledUI.transform.SetParent(localDiskContent.transform);
                            shiledUI.transform.SetAsLastSibling();
                            continue;
                        case Item.ItemType.Spark:
                            GameObject sparkUI = Instantiate(ItemImageList[0]);
                            sparkUI.transform.SetParent(localDiskContent.transform);
                            sparkUI.transform.SetAsLastSibling();
                            continue;
                    }


                }

            }
        }
        /*AddressSet(mapIndex);*/
    }

    public void AdressSet(int mapIndex)
    {
        for (int i = adressParent.transform.childCount - 1; i > 0; i--) // 0��°�� �����ϰ� ��������
        {
            Destroy(adressParent.transform.GetChild(i).gameObject);
        }
        adressList.Clear();
        List<Map> temList = new();
        Map currentMap = mapGenerator.mapList[mapIndex];
        //�ݺ����Ѿ���
        if (currentMap != null)
        {

            while (true)
            {

                if (currentMap.Type == Map.MapType.Start)
                {
                    break;
                }
                //���� ���� �����ͼ� ����ȸ��� ����Ʈ�� �߰�
                int siblingIndex = currentMap.transform.GetSiblingIndex(); // �� ��° �ڽ����� ��������
                if (siblingIndex != 0)
                {
                    //Portal connectMapPortal = mapGenerator.map.GetChild(siblingIndex).GetComponent<Portal>();
                    Transform nowMap = mapGenerator.map.transform.GetChild(siblingIndex);
                    Portal currentMapPortal = nowMap.GetComponentInChildren<Portal>();
                    Portal connectPortal = currentMapPortal.connectPortal;
                    Map connectMap = connectPortal.currentMap.GetComponent<Map>();
                    temList.Add(currentMap);
                    currentMap = connectMap;
                }

            }

        }

        temList.Add(mapGenerator.mapList[0]);

        for (int i = temList.Count - 1; i >= 0; i--)
        {
            adressList.Add(temList[i]);
        }


        for (int i = 0; i < adressList.Count; i++)
        {
            Adress_Button adressObj = Instantiate(adressButton, adressParent.transform);
            Text adressText = adressObj.GetComponentInChildren<Text>();
            adressText.text += adressList[i].mapName + " > ";
        }
        Text firstChildText = adressParent.transform.GetChild(0).GetComponentInChildren<Text>();
        firstChildText.text = "C:\\";
        // ������ �ڽ��� Text ����
        int lastIndex = adressParent.transform.childCount - 1;
        Text lastChildText = adressParent.transform.GetChild(lastIndex).GetComponentInChildren<Text>();
        Debug.Log(lastChildText.text);
        lastChildText.text = lastChildText.text.Replace(">", "");
        /*adressText.text = adressText.text.TrimEnd('>');*/
        Canvas.ForceUpdateCanvases();
        StartCoroutine(LayoutReset(adressParent.GetComponent<RectTransform>()));
    }

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
        adressList.Clear();
    }

    public void SetUIAdress(UIManager.UI uiType)
    {
        switch(uiType)
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
