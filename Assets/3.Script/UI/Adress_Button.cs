using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Adress_Button : MonoBehaviour
{
    MapGenerator mapGenerator;
    UIManager uiManager;

    // Start is called before the first frame update
    void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
        uiManager = UIManager.Instance;
    }

    public void AdressButton()
    {
        Debug.Log("버튼눌림");
        int index = transform.GetSiblingIndex()-1;
        int targetIndex = mapGenerator.mapList.IndexOf(uiManager.adressList[index]);
        uiManager.LocalDisckUISet(targetIndex);

    }
}
