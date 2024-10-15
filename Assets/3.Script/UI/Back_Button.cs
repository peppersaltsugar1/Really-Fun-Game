using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Back_Button : MonoBehaviour
{
    UIManager uiManager;
    MapGenerator mapGenerator;
    // Start is called before the first frame update
    void Start()
    {
        uiManager = UIManager.Instance;
        mapGenerator = FindObjectOfType<MapGenerator>();
    }

    // Update is called once per frame
   

    public void BackButton()
    {
        if (uiManager.addressList.Count > 1)
        {
            Map targetMap = uiManager.addressList[uiManager.addressList.Count - 2];
            int targetIndex = mapGenerator.mapList.IndexOf(targetMap);
            uiManager.LocalDisckUISet(targetIndex);
        }
    }
}
