using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Back_Button : MonoBehaviour
{
    UIManager uiManager;
    // Start is called before the first frame update
    void Start()
    {
        uiManager = UIManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BackButton()
    {
        if (uiManager.addressList.Count > 1)
        {
            uiManager.LocalDisckUISet(uiManager.addressList.Count-2);
        }
    }
}
