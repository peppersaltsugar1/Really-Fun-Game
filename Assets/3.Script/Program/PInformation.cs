using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PInformation : MonoBehaviour
{
    private bool isProgramBeingAdded = false;
    public string ProgramName;
    public string Explanation;
    public string PowerExplanation;
    public int AddCoin = 0;
    public int HPHeal = 0;
    public int AttackPower = 0;
    public float AttackSpeed = 0;
    public float MoveSpeed = 0;
    public float BulletSpeed = 0;
    public bool DeleteIsPossible = true;

    // Image Setting
    public SpriteRenderer spriteRenderer;
    public string spriteSheetName;  // 이미지 이름
    public int spriteIndex; // 이미지 내 번호

    // Start is called before the first frame update
    void Start()
    {
        SetSprite(spriteSheetName, spriteIndex);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnCollisionEnter2D(Collision2D collision)
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌한 오브젝트가 Player 인 경우에만 이벤트 처리
        if (!isProgramBeingAdded)
        {
            if (other.CompareTag("Player"))
            {
                isProgramBeingAdded = true;
                if (ProgramManager.Instance != null)
                {
                    ProgramManager.Instance.AddProgramList(this);
                }
                else
                {
                    Debug.LogError("ProgramManager instance not found.");
                }

                Destroy(gameObject);
            }
        }
    }

    public void SetSprite(string spriteSheetName, int spriteIndex)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(spriteSheetName);

        if (sprites != null && spriteIndex >= 0 && spriteIndex < sprites.Length)
        {
           spriteRenderer.sprite = sprites[spriteIndex];
        }
        else
        {
            Debug.LogError("Sprite not found or invalid index.");
        }
    }

}
