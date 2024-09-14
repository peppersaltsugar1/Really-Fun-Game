using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Program : MonoBehaviour
{
    public enum program { AttackPower, AttackSpeed, BulletSpeed, DeleteMonster, HPHeal, MovingSpeed };
    public static int ProgramCount = 6;
    // ProgramInterface
    public string ProgramName; // ���α׷� �̸�
    public string Explanation; // ����
    public string Explanation2; // ����
    public int SpriteNum;   // �̹��� �ѹ�
    protected bool DeleteIsPossible = false; // ���� ������ ���α׷��ΰ�

    // Image Loader
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    protected void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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

    // �̹����� �����ϴ� �ż���
    public void SetSprite(string spriteSheetName, int spriteIndex)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(spriteSheetName);

        if (sprites != null && spriteIndex < sprites.Length)
        {
            Sprite newSprite = sprites[spriteIndex];

            spriteRenderer.sprite = newSprite;
            Debug.Log("Sprite assigned: " + newSprite.name);
        }
        else
        {
            Debug.LogWarning("Sprite not found or index out of range: " + spriteSheetName + "_" + spriteIndex);
        }
    }
}
