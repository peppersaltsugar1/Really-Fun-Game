using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Program : MonoBehaviour
{

    // ProgramInterface
    public string ProgramName; // 프로그램 이름
    public string Explanation; // 설명
    protected bool DeleteIsPossible = false; // 삭제 가능한 프로그램인가

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Program Delete Function
    public void DeleteProgram()
    {
        if(DeleteIsPossible)
        {

        }
    }
}
