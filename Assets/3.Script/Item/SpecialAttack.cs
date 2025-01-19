using System.Collections;
using UnityEngine;

public class SpecialAttack : MonoBehaviour
{
    #region Manager

    private static SpecialAttack instance;
    private ItemManager itemManager;

    #endregion

    #region Variables Declaration

    [Header("Base Element")]
    [SerializeField] private float Damage; // ������
    [SerializeField] private bool isSpecialAttackMode = false; // Ư�� ���� ��� ����
    private float lastClickTime; // ���� Ŭ�� Ȯ�ο�

    [Header("Attack Area")]
    [SerializeField] private float maxSideLength = 10f; // �巡�� �� �ִ� ���� ����
    [SerializeField] private float squareSize = 5f; // ���� Ŭ�� �� ������ ���簢�� ũ��
    [SerializeField] private float damageDelay; // �������� ��������� ������
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float doubleClickThreshold = 0.3f; // ���� Ŭ������ �����ϴ� �ð� ����

    private Vector2 dragStartPosition; // �巡�� ���� ��ġ
    private bool isDragging = false; // �巡�� �� ����
    private Rect currentAttackArea; // ���� ���� ����

    #endregion

    #region Default Function

    public static SpecialAttack Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject specialAttackObject = new GameObject("SpecialAttack");
                instance = specialAttackObject.AddComponent<SpecialAttack>();
                DontDestroyOnLoad(specialAttackObject);
            }
            return instance;
        }
    }


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer�� �Ҵ���� �ʾҽ��ϴ�.");
        }
        else
        {
            lineRenderer.positionCount = 5; // �簢�� ����� �׸��� ���� ������ 4�� + ���������� ����
            lineRenderer.enabled = false; // �ʱ⿡�� ��Ȱ��ȭ
        }

        itemManager = ItemManager.Instance;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (isSpecialAttackMode) // ��� �����̸� �ƹ��͵� ���� ����
                return;

            if (itemManager.ForcedDeletionUse())
            {
                isSpecialAttackMode = true;
            }
        }

        if (isSpecialAttackMode)
        {
            HandleMouseInput();
        }
    }

    #endregion

    #region Line Renderer Updator

    void HandleMouseInput()
    {
        // Ŭ�� ����: �巡�� �Ǵ� ���� Ŭ�� �ĺ�
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isDragging = false; // �巡�� �ʱ�ȭ
            lastClickTime = Time.time; // ������ Ŭ�� �ð� ����
        }

        // �巡�� �� ���� ����
        if (Input.GetMouseButton(0))
        {
            float dragDuration = Time.time - lastClickTime;

            // �巡�� �ð��� 0.1�ʺ��� ��ٸ� �巡�� ���·� ����
            if (dragDuration > 0.1f)
            {
                lineRenderer.enabled = true;

                isDragging = true;
                Vector2 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 size = new Vector2(
                    Mathf.Min(Mathf.Abs(currentMousePosition.x - dragStartPosition.x), maxSideLength),
                    Mathf.Min(Mathf.Abs(currentMousePosition.y - dragStartPosition.y), maxSideLength)
                );
                Vector2 center = dragStartPosition + size / 2 * new Vector2(
                    Mathf.Sign(currentMousePosition.x - dragStartPosition.x),
                    Mathf.Sign(currentMousePosition.y - dragStartPosition.y)
                );

                currentAttackArea = new Rect(center - size / 2, size);
                UpdateLineRenderer(currentAttackArea);
            }
        }

        // Ŭ�� �Ǵ� �巡�� ����
        if (Input.GetMouseButtonUp(0))
        {
            float clickDuration = Time.time - lastClickTime;
            lastClickTime = Time.time;

            // ���� Ŭ�� ó��
            if (clickDuration <= doubleClickThreshold && !isDragging)
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                AttackSquare(mousePosition);
                isSpecialAttackMode = false;
            }
            // �巡�� ���� ó��
            else if (isDragging)
            {
                isDragging = false;
                StartCoroutine(ApplyDamageAfterDelay(currentAttackArea));
                isSpecialAttackMode = false;
            }
        }
    }

    void AttackSquare(Vector2 center)
    {
        Rect attackArea = new Rect(center - Vector2.one * (squareSize / 2), Vector2.one * squareSize);
        UpdateLineRenderer(attackArea); // ���� Ŭ�� �ÿ��� ���� �������� ���� ǥ��
        lineRenderer.enabled = true; // ���� ������ Ȱ��ȭ

        Debug.Log("���� Ŭ�� �ν�");
        StartCoroutine(ApplyDamageAfterDelay(attackArea));

        isSpecialAttackMode = false; // ����� ���� ��� ����
    }

    void UpdateLineRenderer(Rect area)
    {
        Vector3[] corners = new Vector3[5]; // ������ 4�� + ���������� ����

        corners[0] = new Vector3(area.xMin, area.yMin, 0); // Bottom-left
        corners[1] = new Vector3(area.xMin, area.yMax, 0); // Top-left
        corners[2] = new Vector3(area.xMax, area.yMax, 0); // Top-right
        corners[3] = new Vector3(area.xMax, area.yMin, 0); // Bottom-right
        corners[4] = corners[0]; // �ٽ� Bottom-left�� ����

        lineRenderer.SetPositions(corners);
    }

    #endregion

    #region Area Detection and Damaged Monster

    IEnumerator ApplyDamageAfterDelay(Rect attackArea)
    {
        yield return new WaitForSeconds(damageDelay);
        lineRenderer.enabled = false;

        // Put animation effects under this line
        // Animation Line

        Collider2D[] hitMonsters = Physics2D.OverlapAreaAll(
            new Vector2(attackArea.xMin, attackArea.yMin),
            new Vector2(attackArea.xMax, attackArea.yMax)
        );

        foreach (var monster in hitMonsters)
        {
            if (monster.CompareTag("Monster"))
            {
                monster.gameObject.GetComponent<MonsterBase>().Damaged(Damage);
            }
        }
    }

    #endregion

    #region Getter

    public bool GetAttackMode(){ return isSpecialAttackMode; }

    #endregion
}
