using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LocalDiskUIPortalPanel : MonoBehaviour,IPointerClickHandler
{
    float clickTime = 0;
    public Map connectMap;
    public Map currentMap;
    MapGenerator mapGenerator;
    UIManager uiManager;


    // Start is called before the first frame update
    void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
        uiManager = UIManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void OnMouseDoubleClick()
    {
       int index = mapGenerator.mapList.IndexOf(connectMap);
       uiManager.LocalDisckUISet(index);
    }   

    public void OnPointerClick(PointerEventData eventData)
    {
        if ((Time.time - clickTime) < 0.3f)
        {
            if (connectMap.isClear)
            {
                OnMouseDoubleClick();
                clickTime = -1;
            }
        }
        else
        {
            clickTime = Time.time;
        }
    }
}
